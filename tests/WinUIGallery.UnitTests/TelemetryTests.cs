// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinUIGallery.Helpers;
using WinUIGallery.Models;
using WinUIGallery.Pages;
using WinUIGallery.Telemetry;

namespace WinUIGallery.UnitTests;

[TestClass]
public class TelemetryTests
{
    [TestMethod]
    public void PageViewResolverResolvesCatalogSample()
    {
        string? result = PageViewResolver.ResolveSampleId(
            typeof(ItemPage),
            "Button",
            CreateGroups());

        Assert.IsNotNull(result);
        Assert.AreEqual("Button", result);
    }

    [TestMethod]
    public void PageViewResolverRejectsInvalidNavigation()
    {
        List<ControlInfoDataGroup> groups = CreateGroups();

        Assert.IsNull(PageViewResolver.ResolveSampleId(typeof(SectionPage), "Fundamentals", groups));
        Assert.IsNull(PageViewResolver.ResolveSampleId(typeof(ItemPage), "Unknown", groups));
        Assert.IsNull(PageViewResolver.ResolveSampleId(typeof(ItemPage), null, groups));
        Assert.IsNull(PageViewResolver.ResolveSampleId(typeof(ItemPage), 42, groups));

        groups.Add(new ControlInfoDataGroup
        {
            UniqueId = "Other",
            Items =
            {
                new ControlInfoDataItem { UniqueId = "Button" }
            }
        });
        Assert.IsNull(PageViewResolver.ResolveSampleId(typeof(ItemPage), "Button", groups));
    }

    [TestMethod]
    public void TelemetrySettingDefaultsToEnabledOutsideSensitiveRegionsAndPersistsChanges()
    {
        MemorySettingsProvider provider = new();
        SettingsHelper settings = new(provider, "USA");

        Assert.IsTrue(settings.IsTelemetryEnabled);
        Assert.IsTrue(settings.IsTelemetryAllowed);

        settings.IsTelemetryEnabled = false;
        Assert.IsFalse(new SettingsHelper(provider, "USA").IsTelemetryEnabled);

        settings.IsTelemetryEnabled = true;
        Assert.IsTrue(new SettingsHelper(provider, "USA").IsTelemetryEnabled);
    }

    [TestMethod]
    public void TelemetryRequiresConsentInSensitiveRegions()
    {
        string[] sensitiveRegions =
        [
            "AUT", "BEL", "BGR", "BRA", "CAN", "HRV", "CYP", "CZE", "DNK", "EST",
            "FIN", "FRA", "DEU", "GRC", "HUN", "ISL", "IRL", "ITA", "KOR", "LVA",
            "LIE", "LTU", "LUX", "MLT", "NLD", "NOR", "POL", "PRT", "ROU", "SVK",
            "SVN", "ESP", "SWE", "CHE", "GBR"
        ];

        foreach (string region in sensitiveRegions)
        {
            SettingsHelper settings = new(new MemorySettingsProvider(), region);

            Assert.IsTrue(settings.IsPrivacySensitiveRegion, region);
            Assert.IsFalse(settings.IsTelemetryEnabled, region);
            Assert.IsTrue(settings.IsTelemetryConsentRequired, region);
            Assert.IsFalse(settings.IsTelemetryAllowed, region);
        }
    }

    [TestMethod]
    public void TelemetryConsentTakesEffectImmediately()
    {
        MemorySettingsProvider provider = new();
        SettingsHelper settings = new(provider, "DEU");

        settings.IsTelemetryEnabled = true;
        Assert.IsFalse(settings.IsTelemetryAllowed);

        settings.IsTelemetryConsentDismissed = true;
        Assert.IsTrue(settings.IsTelemetryAllowed);

        settings.IsTelemetryEnabled = false;
        Assert.IsFalse(settings.IsTelemetryAllowed);
    }

    [TestMethod]
    public void PageViewTelemetryHonorsSetting()
    {
        WithTemporaryPath((journalPath, now) =>
        {
            using CapturingEventListener listener = new("Microsoft.Windows.WinUIGallery");
            TelemetryService disabledService = CreateService(journalPath, now, enabled: false);
            disabledService.LogPageView("Button");
            Assert.AreEqual(0, listener.EventCount);

            TelemetryService enabledService = CreateService(journalPath, now, enabled: true);
            enabledService.LogPageView("Button");

            Assert.AreEqual(1, listener.EventCount);
            Assert.AreEqual("Button", listener.Payload["PageId"]);
            Assert.AreEqual("1.2.3.4", listener.Payload["AppVersion"]);
            Assert.AreEqual(System.Diagnostics.Tracing.EventKeywords.None, listener.Keywords);
        });
    }

    [TestMethod]
    public void CrashRecordSanitizesStackTraceAndOmitsMessage()
    {
        InvalidOperationException exception = CaptureException();

        CrashTelemetryRecord crash = CrashTelemetryRecord.Create(
            exception,
            "Button",
            "1.2.3.4",
            new DateTimeOffset(2026, 9, 15, 12, 0, 0, TimeSpan.Zero));

        Assert.AreEqual("Button", crash.SampleId);
        Assert.AreEqual(typeof(InvalidOperationException).FullName, crash.ExceptionType);
        Assert.AreEqual(exception.HResult, crash.HResult);
        StringAssert.Contains(crash.StackTrace, nameof(ThrowCrashForTest));
        Assert.IsFalse(crash.StackTrace.Contains("TelemetryTests.cs", StringComparison.Ordinal));
        Assert.IsFalse(crash.StackTrace.Contains(exception.Message, StringComparison.Ordinal));
    }

    [TestMethod]
    public void CrashRecordKeepsWinUIAndWindowsAppSdkFrames()
    {
        string stackTrace = string.Join(
            Environment.NewLine,
            @"   at WinUIGallery.Pages.ItemPage.Load() in E:\src\ItemPage.cs:line 10",
            @"   at Microsoft.UI.Xaml.Controls.Frame.Navigate() in E:\src\Frame.cs:line 20",
            @"   at Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent()",
            @"   at Windows.Foundation.Metadata.ApiInformation.IsTypePresent(String typeName)",
            @"   at WinRT.ExceptionHelpers.ThrowExceptionForHR(Int32 hr)",
            @"   at ABI.Microsoft.UI.Xaml.IApplicationStaticsMethods.Start()",
            @"   at ABI.Windows.Foundation.IAsyncActionMethods.GetResults()",
            @"   at Contoso.Library.Run() in C:\Users\someone\Contoso.cs:line 30");

        string sanitized = CrashTelemetryRecord.SanitizeStackTrace(stackTrace);

        StringAssert.Contains(sanitized, "WinUIGallery.Pages.ItemPage.Load()");
        StringAssert.Contains(sanitized, "Microsoft.UI.Xaml.Controls.Frame.Navigate()");
        StringAssert.Contains(sanitized, "Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent()");
        StringAssert.Contains(sanitized, "Windows.Foundation.Metadata.ApiInformation.IsTypePresent");
        StringAssert.Contains(sanitized, "WinRT.ExceptionHelpers.ThrowExceptionForHR");
        StringAssert.Contains(sanitized, "ABI.Microsoft.UI.Xaml.IApplicationStaticsMethods.Start()");
        StringAssert.Contains(sanitized, "ABI.Windows.Foundation.IAsyncActionMethods.GetResults()");
        Assert.IsFalse(sanitized.Contains("Contoso.Library", StringComparison.Ordinal));
        Assert.IsFalse(sanitized.Contains(@"E:\src", StringComparison.Ordinal));
        Assert.IsFalse(sanitized.Contains(@"C:\Users", StringComparison.Ordinal));
    }

    [TestMethod]
    public void CrashTelemetryWritesSanitizedPayload()
    {
        WithTemporaryPath((journalPath, now) =>
        {
            using CapturingEventListener listener = new("Microsoft.Windows.WinUIGallery");
            TelemetryService service = CreateService(journalPath, now, enabled: true);
            InvalidOperationException exception = CaptureException();

            service.RecordCrash(exception, "Button");

            Assert.AreEqual(1, listener.EventCount);
            Assert.AreEqual("Button", listener.Payload["SampleId"]);
            Assert.AreEqual(typeof(InvalidOperationException).FullName, listener.Payload["ExceptionType"]);
            Assert.AreEqual(exception.HResult, listener.Payload["HResult"]);
            Assert.IsFalse(listener.Payload.Values.Contains(exception.Message));
            Assert.IsFalse(File.Exists(journalPath));
        });
    }

    [TestMethod]
    public void CrashTelemetryRetriesPendingRecord()
    {
        WithTemporaryPath((journalPath, now) =>
        {
            Queue<bool> results = new([false, true]);
            List<CrashTelemetryRecord> crashes = [];
            TelemetryService service = CreateService(
                journalPath,
                now,
                enabled: true,
                crash =>
                {
                    crashes.Add(crash);
                    return results.Dequeue();
                });

            service.RecordCrash(CaptureException(), "Button");
            Assert.IsTrue(File.Exists(journalPath));

            service.TrySendPendingCrash();

            Assert.AreEqual(2, crashes.Count);
            Assert.AreEqual("Button", crashes[1].SampleId);
            Assert.IsFalse(File.Exists(journalPath));
        });
    }

    [TestMethod]
    public void CrashTelemetryDeletesPendingRecordWhenDisabled()
    {
        WithTemporaryPath((journalPath, now) =>
        {
            CreateService(journalPath, now, enabled: true, _ => false)
                .RecordCrash(CaptureException(), "Button");
            Assert.IsTrue(File.Exists(journalPath));

            CreateService(journalPath, now, enabled: false)
                .TrySendPendingCrash();

            Assert.IsFalse(File.Exists(journalPath));
        });
    }

    [TestMethod]
    public void CrashTelemetryExpiresOldRecord()
    {
        WithTemporaryPath((journalPath, now) =>
        {
            CreateService(journalPath, now.AddDays(-8), enabled: true, _ => false)
                .RecordCrash(CaptureException(), "Button");
            Assert.IsTrue(File.Exists(journalPath));

            int retryCount = 0;
            CreateService(
                journalPath,
                now,
                enabled: true,
                _ =>
                {
                    retryCount++;
                    return true;
                })
                .TrySendPendingCrash();

            Assert.AreEqual(0, retryCount);
            Assert.IsFalse(File.Exists(journalPath));
        });
    }

    private static TelemetryService CreateService(
        string journalPath,
        DateTimeOffset now,
        bool enabled,
        Func<CrashTelemetryRecord, bool>? crashWriter = null)
    {
        return new TelemetryService(
            journalPath,
            () => enabled,
            () => now,
            () => "1.2.3.4",
            crashWriter);
    }

    private static List<ControlInfoDataGroup> CreateGroups()
    {
        return
        [
            new ControlInfoDataGroup
            {
                UniqueId = "Fundamentals",
                Items =
                {
                    new ControlInfoDataItem { UniqueId = "Button" }
                }
            }
        ];
    }

    private static InvalidOperationException CaptureException()
    {
        try
        {
            ThrowCrashForTest();
        }
        catch (InvalidOperationException exception)
        {
            return exception;
        }

        throw new AssertFailedException("Expected test exception was not thrown.");
    }

    private static void ThrowCrashForTest()
    {
        throw new InvalidOperationException("Sensitive exception message");
    }

    private static void WithTemporaryPath(Action<string, DateTimeOffset> action)
    {
        string directory = Path.Combine(Path.GetTempPath(), "WinUIGallery.UnitTests", Guid.NewGuid().ToString("N"));
        string journalPath = Path.Combine(directory, "pending-crash-telemetry.json");
        DateTimeOffset now = new(2026, 9, 15, 12, 0, 0, TimeSpan.Zero);

        try
        {
            action(journalPath, now);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    private sealed class MemorySettingsProvider : ISettingsProvider
    {
        private readonly Dictionary<string, object> values = [];

        public bool Contains(string key) => values.ContainsKey(key);

        public object? Get(string key) => values.TryGetValue(key, out object? value) ? value : null;

        public void Set(string key, object value) => values[key] = value;

        public T? Get<T>(string key) => Get(key) is T value ? value : default;

        public void Set<T>(string key, T value) => values[key] = value!;
    }

    private sealed class CapturingEventListener : System.Diagnostics.Tracing.EventListener
    {
        private readonly string providerName;

        public CapturingEventListener(string providerName)
        {
            this.providerName = providerName;

            foreach (System.Diagnostics.Tracing.EventSource source in System.Diagnostics.Tracing.EventSource.GetSources())
            {
                EnableIfMatched(source);
            }
        }

        public int EventCount { get; private set; }

        public Dictionary<string, object?> Payload { get; } = [];

        public System.Diagnostics.Tracing.EventKeywords Keywords { get; private set; }

        protected override void OnEventSourceCreated(System.Diagnostics.Tracing.EventSource eventSource)
        {
            EnableIfMatched(eventSource);
        }

        protected override void OnEventWritten(System.Diagnostics.Tracing.EventWrittenEventArgs eventData)
        {
            EventCount++;
            Keywords = eventData.Keywords;
            if (eventData.PayloadNames is not null && eventData.Payload is not null)
            {
                foreach ((string name, object? value) in eventData.PayloadNames.Zip(eventData.Payload))
                {
                    Payload[name] = value;
                }
            }
        }

        private void EnableIfMatched(System.Diagnostics.Tracing.EventSource source)
        {
            if (source.Name == providerName)
            {
                EnableEvents(source, System.Diagnostics.Tracing.EventLevel.LogAlways);
            }
        }
    }
}
