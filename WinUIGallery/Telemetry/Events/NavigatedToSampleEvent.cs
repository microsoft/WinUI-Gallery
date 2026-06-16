// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Diagnostics.Tracing;

namespace WinUIGallery.Telemetry.Events;

[EventData]
internal class NavigatedToSampleEvent : EventBase
{
    public override PartA_PrivTags PartA_PrivTags => PrivTags.ProductAndServiceUsage;

    public string SampleId { get; }

    public string SampleTitle { get; }

    public DateTime StartTime { get; }

    private NavigatedToSampleEvent(string sampleId, string sampleTitle, DateTime startTime)
    {
        SampleId = sampleId;
        SampleTitle = sampleTitle;
        StartTime = startTime;
    }

    public override void ReplaceSensitiveStrings(Func<string?, string?> replaceSensitiveStrings)
    {
        // No sensitive strings to replace.
    }

    public static void Log(string sampleId, string sampleTitle)
    {
        TelemetryFactory.Get<ITelemetry>().Log(
            "NavigatedToSample_Event",
            LogLevel.Critical,
            new NavigatedToSampleEvent(sampleId, sampleTitle, DateTime.Now));
    }
}
