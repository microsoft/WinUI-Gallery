// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Diagnostics.Tracing;

namespace AIDevGallery.Telemetry;

[EventData]
internal class EmptyEvent : EventBase
{
    public override PartA_PrivTags PartA_PrivTags { get; }

    public EmptyEvent(PartA_PrivTags tags)
    {
        PartA_PrivTags = tags;
    }

    public override void ReplaceSensitiveStrings(Func<string?, string?> replaceSensitiveStrings)
    {
        // No sensitive string
    }
}