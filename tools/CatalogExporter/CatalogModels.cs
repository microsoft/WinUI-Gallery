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
    public string SchemaVersion { get; set; } = "1.0.0";
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
    public string Name { get; set; } = string.Empty;
    public string Snippet { get; set; } = string.Empty;
}
