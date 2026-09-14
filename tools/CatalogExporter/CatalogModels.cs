// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Root document for catalog/windows-samples.json. See catalog/windows-samples.schema.json for
/// the formal JSON Schema contract and catalog/README.md for how source metadata maps here.
/// </summary>
internal sealed class CatalogManifest
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "./windows-samples.schema.json";
    public int SchemaVersion { get; set; } = 1;
    public CatalogGeneratorInfo Generator { get; set; } = new();
    public CatalogRepository Repository { get; set; } = new();
    public CatalogDefaults Defaults { get; set; } = new();
    public int SampleCount { get; set; }
    public List<CatalogSample> Samples { get; set; } = [];
}

internal sealed class CatalogGeneratorInfo
{
    public string Tool { get; set; } = "tools/CatalogExporter";
    public string Command { get; set; } = "dotnet run --project tools/CatalogExporter -- generate";
}

/// <summary>
/// Repository-level provenance. Intentionally does not pin a commit SHA: the manifest describes
/// the state of the repository's default branch and is regenerated whenever samples change,
/// rather than being tied to an ever-changing hash.
/// </summary>
internal sealed class CatalogRepository
{
    public string Id { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
}

/// <summary>
/// Facts shared by every sample in this repository so they are not repeated on each entry.
/// </summary>
internal sealed class CatalogDefaults
{
    public string Language { get; set; } = "C#";
    public string Framework { get; set; } = "WinUI 3";
    public string Platform { get; set; } = "Windows App SDK";
    public string License { get; set; } = "MIT";
    public string Kind { get; set; } = "embedded-gallery-page";
}

internal sealed class CatalogSample
{
    /// <summary>Source-qualified, collision-safe id: "{owner}/{repo}#{uniqueId}".</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Original WinUI Gallery UniqueId from ControlInfoData.json.</summary>
    public string UniqueId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public CatalogGroupRef Group { get; set; } = new();
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string? ApiNamespace { get; set; }
    public List<string>? BaseClasses { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Aliases { get; set; }
    public List<string>? RelatedSamples { get; set; }
    public List<CatalogDocLink>? Docs { get; set; }
    public List<string>? Badges { get; set; }
    public CatalogSource Source { get; set; } = new();
    public List<CatalogScenario>? Scenarios { get; set; }
}

internal sealed class CatalogGroupRef
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

internal sealed class CatalogDocLink
{
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}

/// <summary>Repository-relative source locations for this sample (embedded gallery page).</summary>
internal sealed class CatalogSource
{
    public string Root { get; set; } = string.Empty;
    public string Page { get; set; } = string.Empty;
    public string? CodeBehind { get; set; }
    public List<string>? Snippets { get; set; }
}

/// <summary>
/// A single interactive scenario within the sample page, derived from a
/// controls:ControlExample element's SampleDefinition attribute.
/// </summary>
internal sealed class CatalogScenario
{
    /// <summary>
    /// Stable, source-qualified id: "{owner}/{repo}#{uniqueId}/{snippetFileNameWithoutExtension}".
    /// Derived from the snippet file name rather than the scenario's position in the page, so
    /// inserting or reordering scenarios never renumbers the others. This is also the join key
    /// into catalog/windows-samples.code.json.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>Prose from the bundle's "--- header" section, as shown above the scenario.</summary>
    public string? Description { get; set; }

    public string Snippet { get; set; } = string.Empty;
}

/// <summary>
/// Root document for catalog/windows-samples.code.json - the scenario source that
/// catalog/windows-samples.json deliberately does not inline.
///
/// The split exists because the two files have different audiences: the manifest is metadata that
/// gets imported into the federated windows-samples catalog (which carries no code), while this
/// file exists for consumers that want the code without cloning the repository or making one
/// request per snippet. Both are produced by a single generator pass so they cannot drift.
/// </summary>
internal sealed class CatalogCodeManifest
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "./windows-samples.code.schema.json";
    public int SchemaVersion { get; set; } = 1;
    public CatalogGeneratorInfo Generator { get; set; } = new();

    /// <summary>Relative path to the manifest whose scenario ids this file is keyed by.</summary>
    public string Manifest { get; set; } = "./windows-samples.json";

    public int ScenarioCount { get; set; }
    public List<CatalogScenarioCode> Scenarios { get; set; } = [];
}

/// <summary>The XAML and/or C# for one scenario, keyed by the manifest's scenario id.</summary>
internal sealed class CatalogScenarioCode
{
    public string Id { get; set; } = string.Empty;

    /// <summary>Repository-relative path to the bundle this content was parsed from.</summary>
    public string Source { get; set; } = string.Empty;

    public string? Xaml { get; set; }
    public string? Code { get; set; }
}
