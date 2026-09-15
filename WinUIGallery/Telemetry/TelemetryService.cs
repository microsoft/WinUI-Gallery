// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
#if TELEMETRY
using System.Reflection;
#endif
using System.Text.Json;
using System.Text.Json.Serialization;
using WinUIGallery.Helpers;

namespace WinUIGallery.Telemetry;

internal sealed class TelemetryService
{
    private const string ProviderName = "Microsoft.Windows.WinUIGallery";
    private const string JournalFileName = "pending-crash-telemetry.json";
    private const EventKeywords MeasuresKeyword = (EventKeywords)0x0000400000000000;
    private const ulong ProductAndServiceUsagePrivacyTag = 0x0000000002000000;
    private static readonly TimeSpan MaximumCrashRecordAge = TimeSpan.FromDays(7);
    private static readonly EventSource TelemetryEventSource = CreateEventSource();
    private static readonly Lazy<TelemetryService> DefaultInstance = new(CreateDefault);

    private readonly string journalPath;
    private readonly Func<bool> isTelemetryEnabled;
    private readonly Func<DateTimeOffset> utcNow;
    private readonly Func<string> appVersion;
    private readonly Func<CrashTelemetryRecord, bool> crashWriter;

    internal TelemetryService(
        string journalPath,
        Func<bool> isTelemetryEnabled,
        Func<DateTimeOffset> utcNow,
        Func<string> appVersion,
        Func<CrashTelemetryRecord, bool>? crashWriter = null)
    {
        this.journalPath = journalPath;
        this.isTelemetryEnabled = isTelemetryEnabled;
        this.utcNow = utcNow;
        this.appVersion = appVersion;
        this.crashWriter = crashWriter ?? TryWriteCrashEvent;
    }

    public static TelemetryService Current => DefaultInstance.Value;

    public void LogPageView(string pageId)
    {
        ArgumentException.ThrowIfNullOrEmpty(pageId);

        if (!isTelemetryEnabled())
        {
            return;
        }

#if TELEMETRY
        EventSourceOptions options = new()
        {
            Keywords = MeasuresKeyword
        };

        PageViewTelemetryPayload payload = new(
            pageId,
            appVersion(),
            TelemetryBuildConfiguration.PrivacyProductId,
            ProductAndServiceUsagePrivacyTag);
#else
        EventSourceOptions options = new()
        {
            Level = EventLevel.Verbose
        };

        PageViewTelemetryPayload payload = new(pageId, appVersion(), 0, 0);
#endif

        try
        {
            TelemetryEventSource.Write("PageView_Event", options, payload);
        }
        catch (EventSourceException exception)
        {
            Debug.WriteLine($"Failed to write page-view telemetry: {exception}");
        }
    }

    public void RecordCrash(Exception exception, string? activeSampleId)
    {
        if (!isTelemetryEnabled())
        {
            ClearPendingCrash();
            return;
        }

        CrashTelemetryRecord crash = CrashTelemetryRecord.Create(
            exception,
            activeSampleId,
            appVersion(),
            utcNow());

        SavePendingCrash(crash);
        if (crashWriter(crash) || !isTelemetryEnabled())
        {
            ClearPendingCrash();
        }
    }

    public void TrySendPendingCrash()
    {
        CrashTelemetryRecord? crash = LoadPendingCrash();
        if (crash is null)
        {
            return;
        }

        if (!isTelemetryEnabled())
        {
            ClearPendingCrash();
            return;
        }

        if (crashWriter(crash) || !isTelemetryEnabled())
        {
            ClearPendingCrash();
        }
    }

    public void ClearPendingCrash()
    {
        try
        {
            File.Delete(journalPath);
            File.Delete(journalPath + ".tmp");
        }
        catch (IOException exception)
        {
            Debug.WriteLine($"Failed to clear pending crash telemetry: {exception}");
        }
        catch (UnauthorizedAccessException exception)
        {
            Debug.WriteLine($"Failed to clear pending crash telemetry: {exception}");
        }
    }

    private static TelemetryService CreateDefault()
    {
        ApplicationData applicationData = NativeMethods.IsAppPackaged
            ? ApplicationData.GetDefault()
            : ApplicationData.GetForUnpackaged(ProcessInfoHelper.Publisher, ProcessInfoHelper.ProductName);
        string journalPath = Path.Combine(applicationData.LocalFolder.Path, JournalFileName);

        return new TelemetryService(
            journalPath,
            () => SettingsHelper.Current.IsTelemetryAllowed,
            () => DateTimeOffset.UtcNow,
            () => ProcessInfoHelper.Version);
    }

    private static EventSource CreateEventSource()
    {
#if TELEMETRY
        return new EventSource(
            ProviderName,
            EventSourceSettings.EtwSelfDescribingEventFormat,
            ["ETW_GROUP", $"{{{TelemetryBuildConfiguration.ProviderGroupGuid}}}"]);
#else
        return new EventSource(ProviderName, EventSourceSettings.EtwSelfDescribingEventFormat);
#endif
    }

    private bool TryWriteCrashEvent(CrashTelemetryRecord crash)
    {
#if TELEMETRY
        EventSourceOptions options = new()
        {
            Keywords = MeasuresKeyword
        };

        CrashTelemetryPayload payload = new(
            crash,
            TelemetryBuildConfiguration.PrivacyProductId,
            TelemetryBuildConfiguration.CrashPrivacyTags);
#else
        EventSourceOptions options = new()
        {
            Level = EventLevel.Verbose
        };

        CrashTelemetryPayload payload = new(crash, 0, 0);
#endif

        if (!TelemetryEventSource.IsEnabled(options.Level, options.Keywords))
        {
            return false;
        }

        try
        {
            TelemetryEventSource.Write("UnhandledException_Event", options, payload);
            return true;
        }
        catch (EventSourceException exception)
        {
            Debug.WriteLine($"Failed to write crash telemetry: {exception}");
            return false;
        }
    }

    private bool SavePendingCrash(CrashTelemetryRecord crash)
    {
        string temporaryPath = journalPath + ".tmp";

        try
        {
            string json = JsonSerializer.Serialize(crash, TelemetryJsonContext.Default.CrashTelemetryRecord);
            Directory.CreateDirectory(Path.GetDirectoryName(journalPath)!);
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, journalPath, overwrite: true);
            return true;
        }
        catch (IOException exception)
        {
            Debug.WriteLine($"Failed to save pending crash telemetry: {exception}");
        }
        catch (UnauthorizedAccessException exception)
        {
            Debug.WriteLine($"Failed to save pending crash telemetry: {exception}");
        }

        return false;
    }

    private CrashTelemetryRecord? LoadPendingCrash()
    {
        try
        {
            if (!File.Exists(journalPath))
            {
                return null;
            }

            string json = File.ReadAllText(journalPath);
            CrashTelemetryRecord? crash = JsonSerializer.Deserialize(
                json,
                TelemetryJsonContext.Default.CrashTelemetryRecord);

            if (crash is null ||
                crash.CrashTimeUtc == default ||
                utcNow() - crash.CrashTimeUtc > MaximumCrashRecordAge)
            {
                ClearPendingCrash();
                return null;
            }

            return crash;
        }
        catch (IOException exception)
        {
            Debug.WriteLine($"Failed to load pending crash telemetry: {exception}");
        }
        catch (UnauthorizedAccessException exception)
        {
            Debug.WriteLine($"Failed to load pending crash telemetry: {exception}");
        }
        catch (JsonException exception)
        {
            Debug.WriteLine($"Failed to load pending crash telemetry: {exception}");
            ClearPendingCrash();
        }

        return null;
    }

#if TELEMETRY
    private static class TelemetryBuildConfiguration
    {
        public static string ProviderGroupGuid { get; } = GetMetadata("TelemetryProviderGroupGuid");

        public static ushort PrivacyProductId { get; } = ushort.Parse(
            GetMetadata("TelemetryPrivacyProductId"),
            System.Globalization.CultureInfo.InvariantCulture);

        public static ulong CrashPrivacyTags { get; } = ulong.Parse(
            GetMetadata("TelemetryCrashPrivacyTags"),
            System.Globalization.CultureInfo.InvariantCulture);

        private static string GetMetadata(string key)
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .Single(attribute => attribute.Key == key)
                .Value!;
        }
    }
#endif

    [EventData]
    private sealed class PageViewTelemetryPayload
    {
        public PageViewTelemetryPayload(
            string pageId,
            string appVersion,
            ushort privacyProductId,
            ulong privacyTag)
        {
            PageId = pageId;
            AppVersion = appVersion;
            PartA_PrivacyProduct = privacyProductId;
            PartA_PrivTags = privacyTag;
        }

        public string PageId { get; }

        public string AppVersion { get; }

        public ushort PartA_PrivacyProduct { get; }

        public ulong PartA_PrivTags { get; }
    }

    [EventData]
    private sealed class CrashTelemetryPayload
    {
        public CrashTelemetryPayload(
            CrashTelemetryRecord crash,
            ushort privacyProductId,
            ulong privacyTags)
        {
            CrashTimeUtc = crash.CrashTimeUtc;
            SampleId = crash.SampleId;
            ExceptionType = crash.ExceptionType;
            HResult = crash.HResult;
            StackTrace = crash.StackTrace;
            AppVersion = crash.AppVersion;
            PartA_PrivacyProduct = privacyProductId;
            PartA_PrivTags = privacyTags;
        }

        public DateTime CrashTimeUtc { get; }

        public string SampleId { get; }

        public string ExceptionType { get; }

        public int HResult { get; }

        public string StackTrace { get; }

        public string AppVersion { get; }

        public ushort PartA_PrivacyProduct { get; }

        public ulong PartA_PrivTags { get; }
    }
}

internal sealed class CrashTelemetryRecord
{
    private const int MaximumStackFrames = 32;
    private const int MaximumStackTraceLength = 8192;

    public DateTime CrashTimeUtc { get; set; }

    public string SampleId { get; set; } = string.Empty;

    public string ExceptionType { get; set; } = string.Empty;

    public int HResult { get; set; }

    public string StackTrace { get; set; } = string.Empty;

    public string AppVersion { get; set; } = string.Empty;

    public static CrashTelemetryRecord Create(
        Exception exception,
        string? sampleId,
        string appVersion,
        DateTimeOffset crashTime)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new CrashTelemetryRecord
        {
            CrashTimeUtc = crashTime.UtcDateTime,
            SampleId = sampleId ?? string.Empty,
            ExceptionType = exception.GetType().FullName ?? exception.GetType().Name,
            HResult = exception.HResult,
            StackTrace = SanitizeStackTrace(exception.StackTrace),
            AppVersion = appVersion
        };
    }

    private static string SanitizeStackTrace(string? stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace))
        {
            return string.Empty;
        }

        IEnumerable<string> frames = stackTrace
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Where(frame => frame.Contains("WinUIGallery.", StringComparison.Ordinal))
            .Take(MaximumStackFrames)
            .Select(RemoveSourceLocation);

        string sanitizedStackTrace = string.Join(Environment.NewLine, frames);
        return sanitizedStackTrace.Length <= MaximumStackTraceLength
            ? sanitizedStackTrace
            : sanitizedStackTrace[..MaximumStackTraceLength];
    }

    private static string RemoveSourceLocation(string frame)
    {
        int sourceLocationIndex = frame.IndexOf(" in ", StringComparison.Ordinal);
        return sourceLocationIndex >= 0 ? frame[..sourceLocationIndex] : frame;
    }
}

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(CrashTelemetryRecord))]
internal partial class TelemetryJsonContext : JsonSerializerContext
{
}
