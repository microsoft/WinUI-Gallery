// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Diagnostics.Tracing;

namespace AIDevGallery.Telemetry.Events;

[EventData]
internal class NavigatedToSampleEvent : EventBase
{
    public override PartA_PrivTags PartA_PrivTags => PrivTags.ProductAndServiceUsage;

    public string Name { get; }

    public DateTime StartTime { get; }

    private NavigatedToSampleEvent(string name, DateTime startTime)
    {
        Name = name;
        StartTime = startTime;
    }

    public override void ReplaceSensitiveStrings(Func<string?, string?> replaceSensitiveStrings)
    {
        // No sensitive strings to replace.
    }

    public static void Log(string name)
    {
        TelemetryFactory.Get<ITelemetry>().Log("NavigatedToSample_Event", LogLevel.Critical, new NavigatedToSampleEvent(name, DateTime.Now));
    }
}