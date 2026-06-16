// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Diagnostics.Tracing;

namespace WinUIGallery.Telemetry.Events;

[EventData]
internal class SearchSampleEvent : EventBase
{
    public override PartA_PrivTags PartA_PrivTags => PrivTags.ProductAndServiceUsage;

    public string Query { get; private set; }

    public bool HasMatch { get; }

    public string? SelectedSampleId { get; }

    private SearchSampleEvent(string query, bool hasMatch, string? selectedSampleId)
    {
        Query = query;
        HasMatch = hasMatch;
        SelectedSampleId = selectedSampleId;
    }

    public override void ReplaceSensitiveStrings(Func<string?, string?> replaceSensitiveStrings)
    {
        // Free-form user input — scrub for known sensitive substrings.
        Query = replaceSensitiveStrings(Query) ?? string.Empty;
    }

    public static void Log(string query, bool hasMatch, string? selectedSampleId = null)
    {
        TelemetryFactory.Get<ITelemetry>().Log(
            "SearchSample_Event",
            LogLevel.Critical,
            new SearchSampleEvent(query, hasMatch, selectedSampleId));
    }
}
