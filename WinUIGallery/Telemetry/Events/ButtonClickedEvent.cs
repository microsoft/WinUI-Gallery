// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Diagnostics.Tracing;

namespace WinUIGallery.Telemetry.Events;

[EventData]
internal class ButtonClickedEvent : EventBase
{
    public override PartA_PrivTags PartA_PrivTags => PrivTags.ProductAndServiceUsage;

    public string ButtonName { get; }

    private ButtonClickedEvent(string buttonName)
    {
        ButtonName = buttonName;
    }

    public override void ReplaceSensitiveStrings(Func<string?, string?> replaceSensitiveStrings)
    {
        // No sensitive strings to replace.
    }

    public static void Log(string buttonName)
    {
        TelemetryFactory.Get<ITelemetry>().Log("ButtonClicked_Event", LogLevel.Critical, new ButtonClickedEvent(buttonName));
    }
}
