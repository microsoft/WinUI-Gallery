// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// The parsed contents of a SampleDefinition .txt bundle: the prose header shown above the
/// scenario plus the XAML and/or C# rendered in its code viewer. Any section may be absent.
/// </summary>
internal sealed record SampleBundle(string? Header, string? Xaml, string? CSharp);

/// <summary>
/// Parses SampleDefinition .txt bundles.
///
/// This deliberately mirrors ControlExample.ParseSampleCodeSections in
/// WinUIGallery/Controls/ControlExample.xaml.cs, which is the source of truth for the format.
/// The exporter has to agree with it exactly, because the catalog's promise is "this is the code
/// the gallery shows for this scenario" - if the two parsers diverge, the catalog silently
/// publishes something the app never renders. Keep them in sync.
/// </summary>
internal static class SampleBundleParser
{
    /// <summary>Section lines are "--- name"; the trailing space is part of the marker.</summary>
    private const string SectionMarker = "--- ";

    public static SampleBundle Parse(string content)
    {
        string? header = null;
        string? xaml = null;
        string? csharp = null;
        string? currentSection = null;
        List<string> currentLines = [];

        foreach (string rawLine in content.Split('\n'))
        {
            string trimmed = rawLine.TrimEnd('\r');
            if (trimmed.StartsWith(SectionMarker, StringComparison.Ordinal))
            {
                SaveSection(currentSection, currentLines, ref header, ref xaml, ref csharp);
                currentSection = trimmed[SectionMarker.Length..].Trim();
                currentLines = [];
            }
            else
            {
                currentLines.Add(trimmed);
            }
        }

        SaveSection(currentSection, currentLines, ref header, ref xaml, ref csharp);

        return new SampleBundle(header, xaml, csharp);
    }

    private static void SaveSection(
        string? sectionName,
        List<string> lines,
        ref string? header,
        ref string? xaml,
        ref string? csharp)
    {
        if (sectionName is null)
        {
            return;
        }

        // Unrecognized section names are ignored rather than reported: the app ignores them too,
        // so they cannot affect what a user sees.
        string content = string.Join('\n', lines).Trim();
        if (sectionName.Equals("header", StringComparison.OrdinalIgnoreCase))
        {
            header = content;
        }
        else if (sectionName.Equals("xaml", StringComparison.OrdinalIgnoreCase))
        {
            xaml = content;
        }
        else if (sectionName.Equals("c#", StringComparison.OrdinalIgnoreCase))
        {
            csharp = content;
        }
    }
}
