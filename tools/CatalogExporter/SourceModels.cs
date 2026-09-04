// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Deserialization model for WinUIGallery/SampleSupport/Data/ControlInfoData.json.
/// This mirrors WinUIGallery.Models.ControlInfoData plus the fields that already exist in
/// ControlInfoDataSchema.json/ControlInfoData.json but are not (yet) consumed by the running
/// app (Tags, RelatedControls), and the optional "Catalog" override block added for this
/// exporter (see ControlInfoCatalogOverride).
/// </summary>
internal sealed class ControlInfoRoot
{
    public List<ControlInfoGroup> Groups { get; set; } = [];
}

internal sealed class ControlInfoGroup
{
    public string UniqueId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsSpecialSection { get; set; }
    public List<ControlInfoItem> Items { get; set; } = [];
}

internal sealed class ControlInfoItem
{
    public string UniqueId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] BaseClasses { get; set; } = [];
    public string[] Tags { get; set; } = [];
    public string ApiNamespace { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsNew { get; set; }
    public bool IsUpdated { get; set; }
    public bool IsPreview { get; set; }
    public List<ControlInfoDocLink> Docs { get; set; } = [];

    /// <summary>
    /// UniqueIds of other items in this same file that are conceptually related. Already present
    /// in ControlInfoData.json/ControlInfoDataSchema.json but not consumed by the app today.
    /// </summary>
    public string[] RelatedControls { get; set; } = [];

    /// <summary>
    /// Optional, catalog-only overrides. Everything here is additive/derivable metadata that is
    /// not needed by the running gallery app itself, so it is only ever read by the exporter.
    /// </summary>
    public ControlInfoCatalogOverride? Catalog { get; set; }
}

internal sealed class ControlInfoDocLink
{
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}

/// <summary>
/// Explicit, opt-in override mechanism for the exported catalog manifest. Every field is
/// optional and additive: omitting "Catalog" entirely (the overwhelming majority of items)
/// produces a fully-populated catalog entry derived from the existing fields above plus the
/// on-disk sample folder. Use this block only for the rare item that needs to diverge from what
/// can be safely derived automatically.
/// </summary>
internal sealed class ControlInfoCatalogOverride
{
    /// <summary>
    /// When true, this item is left out of the generated catalog manifest entirely (for example,
    /// a page that is a developer utility rather than a genuine, documentable sample).
    /// </summary>
    public bool Exclude { get; set; }

    /// <summary>
    /// Additional search aliases that are useful for an external catalog/agent but too broad or
    /// noisy for the in-app search experience (which already uses Tags).
    /// </summary>
    public string[] Aliases { get; set; } = [];

    /// <summary>
    /// Extra related-sample references that point outside this repository, formatted as
    /// "{owner}/{repo}#{uniqueId}". Used to extend (not replace) RelatedControls, which can only
    /// reference other items in this same file.
    /// </summary>
    public string[] RelatedSamples { get; set; } = [];
}
