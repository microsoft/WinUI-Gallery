// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using WinUIGallery.Telemetry.Events;

namespace WinUIGallery.Telemetry;

/// <summary>
/// To create an instance call <see cref="TelemetryFactory.Get{T}"/>.
/// </summary>
internal sealed class Telemetry : ITelemetry
{
    private const string ProviderName = "Microsoft.Windows.WinUIGallery";

    /// <summary>
    /// Exception Thrown Event Name.
    /// </summary>
    private const string ExceptionThrownEventName = "ExceptionThrown";

    private static readonly Guid DefaultRelatedActivityId = Guid.Empty;

    /// <summary>
    /// Can only have one EventSource alive per process, so just create one statically.
    /// </summary>
    private static readonly EventSource TelemetryEventSourceInstance = new TelemetryEventSource(ProviderName);

    /// <summary>
    /// Logs telemetry locally, but shouldn't upload it. Similar to an ETW event.
    /// </summary>
    private static readonly EventSourceOptions LocalOption = new() { Level = EventLevel.Verbose };

    /// <summary>
    /// Logs error telemetry locally, but shouldn't upload it. Similar to an ETW event.
    /// </summary>
    private static readonly EventSourceOptions LocalErrorOption = new() { Level = EventLevel.Error };

    private static readonly EventSourceOptions InfoOption = new() { Keywords = TelemetryEventSource.TelemetryKeyword };

    private static readonly EventSourceOptions InfoErrorOption = new() { Level = EventLevel.Error, Keywords = TelemetryEventSource.TelemetryKeyword };

    private static readonly EventSourceOptions MeasureOption = new() { Keywords = TelemetryEventSource.MeasuresKeyword };

    private static readonly EventSourceOptions MeasureErrorOption = new() { Level = EventLevel.Error, Keywords = TelemetryEventSource.MeasuresKeyword };

    private static readonly EventSourceOptions CriticalDataOption = new() { Keywords = TelemetryEventSource.CriticalDataKeyword };

    private static readonly EventSourceOptions CriticalDataErrorOption = new() { Level = EventLevel.Error, Keywords = TelemetryEventSource.CriticalDataKeyword };

    /// <summary>
    /// ActivityId so we can correlate all events in the same run.
    /// </summary>
    private static Guid activityId = Guid.NewGuid();

    /// <summary>
    /// List of strings we should try removing for sensitivity reasons.
    /// </summary>
    private readonly List<KeyValuePair<string, string>> sensitiveStrings = [];

    internal Telemetry()
    {
    }

    /// <summary>
    /// Gets a value indicating whether telemetry is on.
    /// For future use if we add a registry key or some other setting to check if telemetry is turned on.
    /// </summary>
    public bool IsTelemetryOn { get; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether diagnostic telemetry is on.
    /// </summary>
    public bool IsDiagnosticTelemetryOn { get; set; }

    /// <summary>
    /// Add a string that we should try stripping out of some of our telemetry for sensitivity reasons.
    /// </summary>
    public void AddSensitiveString(string name, string replaceWith)
    {
        // Make sure the name isn't blank, hasn't already been added, and is greater than three characters.
        if (!string.IsNullOrWhiteSpace(name) && name.Length > 3 && !this.sensitiveStrings.Exists(item => name.Equals(item.Key, StringComparison.Ordinal)))
        {
            this.sensitiveStrings.Add(new KeyValuePair<string, string>(name, replaceWith ?? string.Empty));
        }
    }

    /// <summary>
    /// Logs an exception at Critical level.
    /// </summary>
    public void LogException(string action, Exception e, Guid? relatedActivityId = null)
    {
        var innerMessage = this.ReplaceSensitiveStrings(e.InnerException?.Message);
        StringBuilder innerStackTrace = new();
        Exception? innerException = e.InnerException;
        while (innerException != null)
        {
            innerStackTrace.Append(innerException.StackTrace);

            // Separating by 2 new lines to distinguish between different exceptions.
            innerStackTrace.AppendLine();
            innerStackTrace.AppendLine();
            innerException = innerException.InnerException;
        }

        this.LogInternal(
            ExceptionThrownEventName,
            LogLevel.Critical,
            new
            {
                action,
                name = e.GetType().Name,
                stackTrace = e.StackTrace,
                innerName = e.InnerException?.GetType().Name,
                innerMessage,
                innerStackTrace = innerStackTrace.ToString(),
                message = this.ReplaceSensitiveStrings(e.Message),
                PartA_PrivTags = PartA_PrivTags.ProductAndServicePerformance,
            },
            relatedActivityId,
            isError: true);
    }

    /// <summary>
    /// Logs an event with no additional data at Critical level.
    /// </summary>
    public void LogCritical(string eventName, bool isError = false, Guid? relatedActivityId = null)
    {
        this.LogInternal(eventName, LogLevel.Critical, new EmptyEvent(PartA_PrivTags.ProductAndServiceUsage), relatedActivityId, isError);
    }

    /// <summary>
    /// Logs an informational event.
    /// </summary>
    public void Log<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase
    {
        data.ReplaceSensitiveStrings(this.ReplaceSensitiveStrings);
        this.LogInternal(eventName, level, data, relatedActivityId, isError: false);
    }

    /// <summary>
    /// Logs an error event.
    /// </summary>
    public void LogError<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase
    {
        data.ReplaceSensitiveStrings(this.ReplaceSensitiveStrings);
        this.LogInternal(eventName, level, data, relatedActivityId, isError: true);
    }

    private void LogInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId, bool isError)
    {
        this.WriteTelemetryEvent(eventName, level, relatedActivityId ?? DefaultRelatedActivityId, isError, data);
    }

    private string? ReplaceSensitiveStrings(string? message)
    {
        if (message != null)
        {
            foreach (KeyValuePair<string, string> pair in this.sensitiveStrings)
            {
                var sb = new StringBuilder();
                var i = 0;
                while (true)
                {
                    var foundPosition = message.IndexOf(pair.Key, i, StringComparison.OrdinalIgnoreCase);
                    if (foundPosition < 0)
                    {
                        sb.Append(message, i, message.Length - i);
                        message = sb.ToString();
                        break;
                    }

                    sb.Append(message, i, foundPosition - i);
                    sb.Append(pair.Value);
                    i = foundPosition + pair.Key.Length;
                }
            }
        }

        return message;
    }

    /// <summary>
    /// Writes the telemetry event info using the TraceLogging API.
    /// </summary>
    private void WriteTelemetryEvent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, Guid relatedActivityId, bool isError, T data)
    {
        EventSourceOptions telemetryOptions;
        if (this.IsTelemetryOn)
        {
            if (!IsDiagnosticTelemetryOn)
            {
                if (!isError && (level == LogLevel.Measure || level == LogLevel.Info))
                {
                    level = LogLevel.Local;
                }
            }

            switch (level)
            {
                case LogLevel.Critical:
                    telemetryOptions = isError ? Telemetry.CriticalDataErrorOption : Telemetry.CriticalDataOption;
                    break;
                case LogLevel.Measure:
                    telemetryOptions = isError ? Telemetry.MeasureErrorOption : Telemetry.MeasureOption;
                    break;
                case LogLevel.Info:
                    telemetryOptions = isError ? Telemetry.InfoErrorOption : Telemetry.InfoOption;
                    break;
                case LogLevel.Local:
                default:
                    telemetryOptions = isError ? Telemetry.LocalErrorOption : Telemetry.LocalOption;
                    break;
            }
        }
        else
        {
            // The telemetry is not turned on, downgrade to local telemetry.
            telemetryOptions = isError ? Telemetry.LocalErrorOption : Telemetry.LocalOption;
        }
#pragma warning disable IL2026
        TelemetryEventSourceInstance.Write(eventName, ref telemetryOptions, ref activityId, ref relatedActivityId, ref data);
#pragma warning restore IL2026
    }

    internal void AddWellKnownSensitiveStrings()
    {
        try
        {
            // This should convert "c:\users\johndoe" to "<UserDirectory>".
            var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            this.AddSensitiveString(userDirectory.ToString(), "<UserDirectory>");

            // Include both these names, since they should cover the logged on user, and the user who is running the tools built on top of these API's.
            this.AddSensitiveString(Environment.UserName, "<UserName>");
            var currentUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last();
            this.AddSensitiveString(currentUserName, "<CurrentUserName>");
        }
        catch (Exception e)
        {
            // Catch and log exception.
            this.LogException("AddSensitiveStrings", e);
        }
    }
}
