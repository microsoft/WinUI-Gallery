# WinUI Gallery catalog manifest

`catalog/windows-samples.json` is a generated, machine-readable inventory of every embedded
sample in this repository. It exists so a future federated
[microsoft/windows-samples](https://github.com/microsoft/windows-samples) catalog (and other
agents/tools) can discover WinUI Gallery's samples without scraping the app's XAML.

## Why this shape, and not one `sample.yml` per control

`windows-samples` normally has one `Samples/<Name>/sample.yml` per **standalone** project. WinUI
Gallery is different: its ~120 samples are **pages embedded in a single app**
(`WinUIGallery/Samples/<UniqueId>/`), and the authoritative inventory of them already exists as
[`WinUIGallery/SampleSupport/Data/ControlInfoData.json`](../WinUIGallery/SampleSupport/Data/ControlInfoData.json)
(consumed by the app itself and by `WinUIGallery.SourceGenerator` at build time).

Adding ~120 hand-maintained `sample.yml` files would create a second, easily-stale source of
truth. Instead:

- **`ControlInfoData.json` stays the single source of truth.** It already carries most of what a
  catalog needs: `Title`, `Subtitle`/`Description`, `Docs`, `Tags`, and `RelatedControls`.
- **`tools/CatalogExporter`** deterministically derives everything else (ids, group, source file
  paths, snippet files, interactive scenarios) from that JSON file plus the on-disk sample
  folders, and validates the result.
- **`catalog/windows-samples.json`** is the generated output: one JSON document, safe to import
  wholesale into `windows-samples`' own `catalog/samples.json` aggregation step.
- Repository-wide constants (license, language, framework, platform, "embedded gallery page"
  kind) live once in the manifest's top-level `defaults`, instead of being repeated on every one
  of the ~120 entries.

## Regenerating and checking the manifest

```powershell
# Regenerate catalog/windows-samples.json from the current source data
dotnet run --project tools/CatalogExporter -- generate

# Verify the committed file is still up to date (used in CI/tests); does not write anything
dotnet run --project tools/CatalogExporter -- check
```

Both commands auto-detect the repository root (by walking up to `WinUIGallery.slnx`); pass
`--repo-root <path>` to override.

`tests/WinUIGallery.CatalogExporter.Tests` also asserts the committed manifest matches a fresh
`generate` output, so a stale or hand-edited `catalog/windows-samples.json` fails `dotnet test`
(and therefore CI) rather than silently drifting from `ControlInfoData.json`.

## Contract

The manifest's shape is formally described by
[`catalog/windows-samples.schema.json`](windows-samples.schema.json) (JSON Schema draft-07). At a
glance:

| Manifest field | Derived from |
| --- | --- |
| `repository`, `defaults` | Fixed for this repository (see `CatalogGenerationOptions` / `CatalogDefaults` in `tools/CatalogExporter`) |
| `samples[].id` | `"{owner}/{repo}#{UniqueId}"` - source-qualified so ids can't collide once aggregated across repositories |
| `samples[].uniqueId`, `.title`, `.summary`, `.description`, `.apiNamespace`, `.baseClasses`, `.tags`, `.docs` | `UniqueId`, `Title`, `Subtitle`, `Description`, `ApiNamespace`, `BaseClasses`, `Tags`, `Docs` on the matching `ControlInfoData.json` item |
| `samples[].group` | The enclosing `ControlInfoData.json` group's `UniqueId`/`Title` |
| `samples[].badges` | Derived from `IsNew` / `IsUpdated` / `IsPreview` |
| `samples[].relatedSamples` | `RelatedControls` (resolved to this repository's ids), plus any `Catalog.RelatedSamples` override |
| `samples[].source` | The `WinUIGallery/Samples/<UniqueId>/` folder: main `*Page.xaml`, `*Page.xaml.cs`, and every `*.txt` snippet, verified to exist with exact-case file names |
| `samples[].scenarios` | One entry per `controls:ControlExample` element whose `SampleDefinition="..."` attribute names a snippet file that exists next to the page |

### The optional `Catalog` override block

`ControlInfoData.json` items may include an optional `Catalog` object
(see `ControlInfoDataSchema.json`) for the rare case where the exporter can't safely derive
something on its own:

```jsonc
"Catalog": {
  "Exclude": false,           // drop this item from the manifest entirely
  "Aliases": [ "cta" ],       // catalog-only search keywords, additive to Tags
  "RelatedSamples": [ "owner/other-repo#sample-id" ] // cross-repository related samples
}
```

This block is read only by `tools/CatalogExporter` - the running gallery app never looks at it -
and is expected to stay rare. `Button` and `ScratchPad` in `ControlInfoData.json` each set
`Catalog.Aliases` as a small, working example of the mechanism.

### Inclusion/exclusion rules

An item from `ControlInfoData.json` becomes a catalog entry when, and only when:

1. It has a non-empty `UniqueId` that is unique across the file.
2. A `WinUIGallery/Samples/<UniqueId>/` folder exists with a case-exact `<UniqueId>Page.xaml` file
   in it.
3. It does not set `Catalog.Exclude: true`.

Anything else - a missing folder/page, a duplicate id, a `RelatedControls`/`Catalog.RelatedSamples`
reference that doesn't resolve to an included entry, or a `SampleDefinition` snippet that doesn't
exist on disk - fails validation (`CatalogValidationException`) rather than being silently
skipped or guessed at. All 120 current `ControlInfoData.json` items satisfy these rules.

### What's intentionally left out

Optional/unknown fields are omitted rather than filled with guesses (for example, `description`,
`apiNamespace`, `badges`, `relatedSamples`, and `scenarios` are all omitted when the source data
has nothing to report). The manifest does not pin a commit SHA - it reflects `repository.defaultBranch`
and is regenerated whenever samples change.
