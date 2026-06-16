// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Diagnostics.Tracing;

namespace WinUIGallery.Telemetry.Events;

[EventData]
internal class ProtocolActivatedEvent : EventBase
{
    public override PartA_PrivTags PartA_PrivTags => PrivTags.ProductAndServiceUsage;

    public string Uri { get; private set; }

    private ProtocolActivatedEvent(string uri)
    {
        Uri = uri;
    }

    public override void ReplaceSensitiveStrings(Func<string?, string?> replaceSensitiveStrings)
    {
        Uri = replaceSensitiveStrings(Uri) ?? string.Empty;
    }

    public static void Log(string uri)
    {
        TelemetryFactory.Get<ITelemetry>().Log(
            "ProtocolActivated_Event",
            LogLevel.Critical,
            new ProtocolActivatedEvent(uri));
    }
}
