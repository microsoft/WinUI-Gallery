// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Windows.Globalization;

namespace AIDevGallery.Telemetry;

internal static class PrivacyConsentHelpers
{
    private static readonly string[] PrivacySensitiveRegions =
    [
        "AUT",
        "BEL",
        "BGR",
        "BRA",
        "CAN",
        "HRV",
        "CYP",
        "CZE",
        "DNK",
        "EST",
        "FIN",
        "FRA",
        "DEU",
        "GRC",
        "HUN",
        "ISL",
        "IRL",
        "ITA",
        "KOR", // Double Check
        "LVA",
        "LIE",
        "LTU",
        "LUX",
        "MLT",
        "NLD",
        "NOR",
        "POL",
        "PRT",
        "ROU",
        "SVK",
        "SVN",
        "ESP",
        "SWE",
        "CHE",
        "GBR",
    ];

    public static bool IsPrivacySensitiveRegion()
    {
        var geographicRegion = new GeographicRegion();

        return PrivacySensitiveRegions.Contains(geographicRegion.CodeThreeLetter, StringComparer.OrdinalIgnoreCase);
    }
}