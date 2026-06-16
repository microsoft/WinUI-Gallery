// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Diagnostics.Tracing;

namespace WinUIGallery.Telemetry.Events;

[EventData]
internal class NavigatedToPageEvent : EventBase
{
    public override PartA_PrivTags PartA_PrivTags => PrivTags.ProductAndServiceUsage;

    public string PageName { get; }

    private NavigatedToPageEvent(string pageName)
    {
        PageName = pageName;
    }

    public override void ReplaceSensitiveStrings(Func<string?, string?> replaceSensitiveStrings)
    {
        // No sensitive strings to replace.
    }

    public static void Log(string pageName)
    {
        TelemetryFactory.Get<ITelemetry>().Log("NavigatedToPage_Event", LogLevel.Critical, new NavigatedToPageEvent(pageName));
    }
}
