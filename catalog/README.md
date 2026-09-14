# WinUI Gallery catalog manifest

`catalog/windows-samples.json` is a generated, machine-readable inventory of every embedded
sample in this repository, and `catalog/windows-samples.code.json` carries the source shown for
each of their scenarios. They exist so a future federated
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

## Why the code lives in a second file

`catalog/windows-samples.code.json` holds the XAML and C# for each scenario, keyed by the
manifest's scenario ids. The split is deliberate, because the two files have different audiences:

- The **manifest** is metadata, and it is what gets imported into the federated
  `windows-samples` catalog - which carries paths, not source. Inlining ~400 KB of code there
  would be dead weight that the importer has to strip.
- The **code file** exists for consumers that want the snippets without cloning the repository:
  one request for all of them, instead of one request per `.txt` file.

Both files are produced by a **single generator pass**, so they cannot drift apart, and a test
asserts that every code entry resolves to a scenario in the manifest.

## Regenerating and checking the manifest

```powershell
# Regenerate both catalog files from the current source data
dotnet run --project tools/CatalogExporter -- generate

# Verify the committed files are still up to date (used in CI/tests); does not write anything
dotnet run --project tools/CatalogExporter -- check
```

Both commands auto-detect the repository root (by walking up to `WinUIGallery.slnx`); pass
`--repo-root <path>` to override.

`tests/WinUIGallery.CatalogExporter.Tests` also asserts the committed files match a fresh
`generate` output, so a stale or hand-edited catalog file fails `dotnet test` (and therefore CI)
rather than silently drifting from `ControlInfoData.json`. The same suite checks that each JSON
Schema declares exactly the properties the exporter emits - both schemas set
`"additionalProperties": false`, so an undeclared field would make every generated file invalid
for consumers.

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
| `samples[].scenarios[].id` | `"{sample id}/{snippet file name without extension}"` - derived from the file name rather than the scenario's position, so inserting or reordering scenarios never renumbers the others. This is the join key into `windows-samples.code.json` |
| `samples[].scenarios[].name`, `.snippet` | One entry per `controls:ControlExample` element whose `SampleDefinition="..."` attribute names a snippet file that exists next to the page |
| `samples[].scenarios[].description` | The snippet's `--- header` section, as shown above the scenario in the app |

`catalog/windows-samples.code.json` is described by
[`catalog/windows-samples.code.schema.json`](windows-samples.code.schema.json) and is versioned
independently:

| Code file field | Derived from |
| --- | --- |
| `scenarios[].id` | Matches a `samples[].scenarios[].id` in the manifest |
| `scenarios[].source` | Repository-relative path to the snippet bundle the content was parsed from |
| `scenarios[].xaml`, `.code` | The snippet's `--- xaml` and `--- c#` sections, verbatim |

### Snippet bundles, and staying faithful to what the app renders

A `SampleDefinition` snippet is a small sectioned text file:

```text
--- header
Built-in styles applied to Button.
--- xaml
<Button Style="{StaticResource AccentButtonStyle}" Content="Accent style button"/>
```

The exporter parses these with `SampleBundleParser`, which deliberately mirrors
`ControlExample.ParseSampleCodeSections` in
[`WinUIGallery/Controls/ControlExample.xaml.cs`](../WinUIGallery/Controls/ControlExample.xaml.cs) -
the source of truth for the format. The catalog's promise is "this is the code the gallery shows
for this scenario", so if the two parsers diverge the catalog silently publishes something users
never see. `SampleBundleParserTests` pins the rules that are easiest to get subtly wrong (the
marker is `"--- "` including the trailing space; section content is trimmed; unknown sections are
ignored). **Keep the two parsers in sync.**

### `$(Token)` placeholders

A page can pair a snippet with `ControlExampleSubstitution` entries that bind a token to one of the interactive option controls, so the code updates as the reader changes a slider or a dropdown. Published verbatim, a token like `$(Spacing)` would leave the snippet invalid and impossible to paste into a project.

The exporter therefore resolves each token to the value its control starts with, which is exactly what the gallery renders when the page first loads. Resolution is deliberately conservative and only reads what the markup actually states: a literal `Value`, an initial attribute on the bound control, or the item a selector explicitly marks as selected. A token is left exactly as written whenever its value depends on running code — a converter function such as `BoolToLowerString(x.IsOn)`, a control that declares no initial value and relies on a framework default, or an `IsEnabled` condition that cannot be settled statically. Publishing a value the gallery does not show would be worse than publishing none, so an unresolved token is the intended fallback rather than a failure.

A substitution whose `IsEnabled` is false resolves to the empty string, matching `ControlExampleSubstitution.ValueAsString`. Literal values keep their surrounding whitespace, because some snippets rely on a value such as `` IsSticky="True" `` to supply its own separating spaces.

`SubstitutionResolverTests` pins these rules, and most of its cases assert that an ambiguous binding is skipped rather than guessed.

### Scenarios without code

A scenario appears in the manifest but contributes no entry to the code file when either:

- its page sets `SourceCodeVisibility="Collapsed"`, so the gallery deliberately shows no code; or
- the page swaps `ControlExample.XamlSource` at runtime, so there is no single snippet that
  represents the scenario.

`RealRepository_CodelessScenariosAreTheKnownSet` pins the current set (two scenarios) so this gap stays visible and shrinking it is a deliberate, reviewed change.

Every other scenario supplies its code through a `SampleDefinition` snippet bundle. That is enforced, not merely conventional: `RealRepository_NoSampleUsesInlineControlExampleCode` fails if any page reintroduces inline `<ControlExample.Xaml>` or `<ControlExample.CSharp>` markup. Inline code renders correctly in the gallery but is invisible to the exporter, so it would otherwise go missing from the catalog without any visible symptom.

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
reference that doesn't resolve to an included entry, a `SampleDefinition` snippet that doesn't
exist on disk, or two scenarios resolving to the same id - fails validation
(`CatalogValidationException`) rather than being silently skipped or guessed at. All 120 current
`ControlInfoData.json` items satisfy these rules.

### What's intentionally left out

Optional/unknown fields are omitted rather than filled with guesses (for example, `description`,
`apiNamespace`, `badges`, `relatedSamples`, and `scenarios` are all omitted when the source data
has nothing to report). The manifest does not pin a commit SHA - it reflects `repository.defaultBranch`
and is regenerated whenever samples change.
