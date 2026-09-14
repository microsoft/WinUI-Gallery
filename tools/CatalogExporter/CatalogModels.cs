// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Root document for catalog/windows-samples.json.
///
/// The shape is not this repository's invention: it is the published "WinUI sample index" contract
/// defined by microsoft/winappCli (see <see cref="Schema"/>), which already has a working consumer
/// and a second publisher (microsoft-ui-reactor). Emitting that contract directly is what lets a
/// tool read this file without writing a WinUI-Gallery-specific parser, and is also why the code
/// is inline here rather than in a sibling file: the contract is built around a consumer making a
/// single HTTP request.
///
/// The contract allows additional properties, so gallery-specific provenance it has no slot for is
/// carried under a <c>gallery</c> object on each control and sample. It is grouped rather than
/// scattered so a reader can tell at a glance which fields are the shared contract and which are
/// ours.
/// </summary>
internal sealed class SampleIndex
{
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "https://raw.githubusercontent.com/microsoft/winappCli/main/docs/winui-sample-index.schema.json";

    /// <summary>Contract version. Version 1 is the only value the schema accepts.</summary>
    public int SchemaVersion { get; set; } = 1;

    /// <summary>
    /// Identifies the publisher to consumers. "gallery" is the value winappCli already uses for
    /// this repository, so it is fixed rather than derived from the repository name.
    /// </summary>
    public string Source { get; set; } = "gallery";

    /// <summary>
    /// Gallery-specific provenance. Deliberately carries no timestamp: the file is committed and
    /// CI re-runs the generator to check it is current, so a generation time would make every run
    /// differ and turn that check into constant churn.
    /// </summary>
    public IndexGeneratorInfo Generator { get; set; } = new();

    public int ControlCount { get; set; }

    public List<IndexControl> Controls { get; set; } = [];
}

internal sealed class IndexGeneratorInfo
{
    public string Tool { get; set; } = "tools/CatalogExporter";
    public string Command { get; set; } = "dotnet run --project tools/CatalogExporter -- generate";
    public string Repository { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = string.Empty;
    public string License { get; set; } = "MIT";
}

/// <summary>One gallery sample page, expressed as a control in the shared contract.</summary>
internal sealed class IndexControl
{
    /// <summary>
    /// Lowercased <see cref="IndexControlGallery.UniqueId"/>. The contract scopes ids to a source
    /// rather than globally, and the consumer builds per-sample ids as "{id}-{n}", so a short
    /// URL-safe token is used here instead of an "owner/repo#Name" form.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>One-line summary (ControlInfoData Subtitle).</summary>
    public string? Description { get; set; }

    /// <summary>Long-form prose (ControlInfoData Description).</summary>
    public string? Details { get; set; }

    public string? ApiNamespace { get; set; }

    /// <summary>Display names of related controls, per the contract — not source-qualified ids.</summary>
    public List<string>? RelatedControls { get; set; }

    /// <summary>
    /// Namespace declarations shared by every sample below. A sample needing a different set
    /// carries its own; the contract treats this as the default for the ones that do not.
    /// </summary>
    public List<string>? XmlnsImports { get; set; }

    /// <summary>
    /// Namespaces the control's C# samples assume are imported. The consumer prepends these as
    /// "using X;" lines so a snippet compiles on its own, which is why they are not repeated
    /// inside each sample's code.
    /// </summary>
    public List<string>? Usings { get; set; }

    /// <summary>Supplementary, derived search terms.</summary>
    public List<string>? Keywords { get; set; }

    /// <summary>
    /// Search terms written by the sample's own author in ControlInfoData.json. Kept separate from
    /// <see cref="Keywords"/> because the consumer weighs first-hand terms more heavily.
    /// </summary>
    public List<string>? CuratedKeywords { get; set; }

    public List<IndexDocLink>? Docs { get; set; }

    public IndexControlGallery Gallery { get; set; } = new();

    /// <summary>
    /// The control's samples in page order — the order a visitor sees them in the gallery.
    /// Ordering is significant: the consumer numbers samples positionally, so appending a
    /// ControlExample is safe while reordering renumbers the ones after it.
    /// </summary>
    public List<IndexSample> Samples { get; set; } = [];
}

/// <summary>Gallery-specific provenance that the shared contract has no field for.</summary>
internal sealed class IndexControlGallery
{
    /// <summary>Original WinUI Gallery UniqueId from ControlInfoData.json.</summary>
    public string UniqueId { get; set; } = string.Empty;

    public IndexGroupRef Group { get; set; } = new();

    /// <summary>Repository-relative path to the sample page.</summary>
    public string Page { get; set; } = string.Empty;

    public string? CodeBehind { get; set; }

    public List<string>? BaseClasses { get; set; }

    /// <summary>"New", "Updated" and/or "Preview", as shown on the gallery's home page.</summary>
    public List<string>? Badges { get; set; }

    /// <summary>Source-qualified ids of related samples, for callers that need to resolve links.</summary>
    public List<string>? RelatedSamples { get; set; }
}

internal sealed class IndexGroupRef
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

internal sealed class IndexDocLink
{
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}

/// <summary>
/// One scenario from a page, expressed as a sample in the shared contract. The contract requires
/// at least one of <see cref="Xaml"/> or <see cref="Code"/> to be present and non-empty, so a
/// scenario with neither is left out of the index entirely.
/// </summary>
internal sealed class IndexSample
{
    public string? Header { get; set; }

    /// <summary>
    /// XAML as the gallery renders it on load, with $(Token) substitutions already applied.
    /// Omitted when the snippet is not a well-formed XML fragment, because the consumer validates
    /// this and silently discards whatever fails — so publishing it would advertise code that
    /// never actually arrives.
    /// </summary>
    public string? Xaml { get; set; }

    public string? Code { get; set; }

    /// <summary>Set to "csharp" whenever <see cref="Code"/> is present; the only value version 1 accepts.</summary>
    public string? Language { get; set; }

    public List<string>? XmlnsImports { get; set; }

    public IndexSampleGallery Gallery { get; set; } = new();
}

internal sealed class IndexSampleGallery
{
    /// <summary>
    /// Snippet file name. Unlike the sample's position, this survives insertion and reordering, so
    /// it is the stable way to refer to one scenario across regenerations.
    /// </summary>
    public string Snippet { get; set; } = string.Empty;

    /// <summary>Repository-relative path to the bundle this content was parsed from.</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>Name derived from the snippet file name, used when the bundle declares no header.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// True when the snippet's XAML was left out because it is not a well-formed fragment. Kept in
    /// the index so the omission is visible to a reader rather than looking like a sample that
    /// simply has no XAML.
    /// </summary>
    public bool? XamlOmittedAsMalformed { get; set; }
}
