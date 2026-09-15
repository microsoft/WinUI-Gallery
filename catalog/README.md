# WinUI Gallery sample index

`catalog/windows-samples.json` is a generated, machine-readable index of every embedded sample in this repository, including the XAML and C# the gallery shows for each one. It exists so that tools and agents can discover and reuse WinUI Gallery's samples without scraping the app's XAML.

## The shape is a shared contract, not ours

The file conforms to the **WinUI sample index** contract published by [microsoft/winappCli](https://github.com/microsoft/winappCli) at [`docs/winui-sample-index.schema.json`](https://raw.githubusercontent.com/microsoft/winappCli/main/docs/winui-sample-index.schema.json), which the generated file names in its own `$schema` property.

That contract already has a working consumer and a second publisher ([microsoft-ui-reactor](https://github.com/microsoft/microsoft-ui-reactor) publishes a conforming index today), so emitting it directly means a tool that already reads one source can read this one with no new parser and no gallery-specific special cases. The alternative — inventing a gallery-shaped file and asking every consumer to adapt to it — would have produced the same data behind a second, redundant contract.

Two consequences of the contract are worth knowing before reading the file:

- **Code is inline.** The contract is explicitly designed so a consumer needs *one* HTTP request rather than one per snippet, so each sample carries its `xaml` and `code` directly instead of pointing at a file to fetch separately.
- **Sample order is meaningful.** Consumers number a control's samples positionally. Samples are therefore emitted in page order — the order a visitor sees them — so appending a `ControlExample` is safe while reordering renumbers the ones after it.

Fields the contract has no slot for are grouped under a `gallery` object on each control and sample, rather than scattered among the shared ones, so it stays obvious which is which. The contract permits these extras and consumers ignore them.

## Why this, and not one `sample.yml` per control

`windows-samples` normally has one `Samples/<Name>/sample.yml` per **standalone** project. WinUI Gallery is different: its ~120 samples are **pages embedded in a single app** (`WinUIGallery/Samples/<UniqueId>/`), and the authoritative inventory of them already exists as [`WinUIGallery/SampleSupport/Data/ControlInfoData.json`](../WinUIGallery/SampleSupport/Data/ControlInfoData.json), consumed by the app itself and by `WinUIGallery.SourceGenerator` at build time.

Adding ~120 hand-maintained `sample.yml` files would create a second, easily-stale source of truth. Instead:

- **`ControlInfoData.json` stays the single source of truth.** It already carries most of what an index needs: `Title`, `Subtitle`/`Description`, `Docs`, `Tags`, and `RelatedControls`.
- **`tools/CatalogExporter`** deterministically derives everything else — ids, source paths, scenarios, and the code itself — from that JSON file plus the on-disk sample folders, and validates the result.

## Regenerating and checking the index

```powershell
# Regenerate the index from the current source data
dotnet run --project tools/CatalogExporter -- generate

# Verify the committed file is still up to date (used in CI/tests); does not write anything
dotnet run --project tools/CatalogExporter -- check
```

Both commands auto-detect the repository root by walking up to `WinUIGallery.slnx`; pass `--repo-root <path>` to override.

`tests/WinUIGallery.CatalogExporter.Tests` asserts the committed file matches a fresh `generate`, so a stale or hand-edited index fails `dotnet test` (and therefore CI) rather than silently drifting from `ControlInfoData.json`. `ContractConformanceTests` additionally checks the parts a consumer depends on that valid JSON alone would not catch — that every emitted field name is one the contract defines, that required fields are present, that control ids are unique and URL-safe, and that every published XAML fragment parses.

The index deliberately carries **no generation timestamp**, even though the contract offers `generatedAtUtc`. The file is committed and CI regenerates it to prove it is current; a timestamp would make every run differ and turn that check into constant churn.

## How source data maps onto the contract

| Contract field | Derived from |
| --- | --- |
| `source` | Fixed as `"gallery"` — the identifier the consumer already uses for this repository |
| `controls[].id` | The lowercased `UniqueId`. The contract scopes ids to a source, so `"button"` is unambiguous without repeating the repository in it |
| `controls[].name`, `.description`, `.details`, `.apiNamespace`, `.docs` | `Title`, `Subtitle`, `Description`, `ApiNamespace`, `Docs` on the matching `ControlInfoData.json` item |
| `controls[].relatedControls` | `RelatedControls`, as display names — the contract asks for names here, not ids |
| `controls[].curatedKeywords` | `Tags` plus any `Catalog.Aliases`. Both are written by the sample's own author, and the contract has one slot for author-written terms, which consumers weigh above derived ones |
| `controls[].keywords` | `BaseClasses`, as supplementary derived search terms |
| `controls[].xmlnsImports` | The namespace declarations the control's samples actually use, resolved against its page's own `xmlns` attributes. Hoisted here only when every sample needs the same set; otherwise each sample carries its own. A prefix the page itself does not declare — a snippet using `local:` to mean "your namespace" — is omitted rather than invented, since a guessed URI would look authoritative and not compile |
| `controls[].samples[]` | One entry per `controls:ControlExample` whose `SampleDefinition="..."` names a snippet file that exists next to the page, in page order |
| `controls[].samples[].header` | The snippet's `--- header` section, as shown above the scenario in the app; falls back to a name derived from the snippet file name |
| `controls[].samples[].xaml`, `.code` | The snippet's `--- xaml` and `--- c#` sections, with `$(Token)` placeholders resolved. `xaml` is guaranteed to contain no leftover token; `code` may still carry one that was not safe to remove |
| `controls[].samples[].language` | `"csharp"` whenever `code` is present; the only value contract version 1 accepts |
| `controls[].samples[].gallery.codePlaceholdersPresent` | The `$(Token)` names still present in `code`, in order of first appearance; omitted entirely when there are none. The mirror image of `gallery.xamlPlaceholdersDropped`, not a companion to it: that field names tokens that were *removed*, this one names tokens that are *still there*, so a sample carrying it is not pasteable as published |
| `controls[].gallery`, `samples[].gallery` | Gallery-specific provenance: `uniqueId`, `group`, page and snippet paths, badges, base classes, and source-qualified related-sample ids |

### Snippet bundles, and staying faithful to what the app renders

A `SampleDefinition` snippet is a small sectioned text file:

```text
--- header
Built-in styles applied to Button.
--- xaml
<Button Style="{StaticResource AccentButtonStyle}" Content="Accent style button"/>
```

The exporter parses these with `SampleBundleParser`, which deliberately mirrors `ControlExample.ParseSampleCodeSections` in [`WinUIGallery/Controls/ControlExample.xaml.cs`](../WinUIGallery/Controls/ControlExample.xaml.cs) — the source of truth for the format. The index's promise is "this is the code the gallery shows for this scenario", so if the two parsers diverge it silently publishes something users never see. `SampleBundleParserTests` pins the rules that are easiest to get subtly wrong: the marker is `"--- "` including the trailing space, section content is trimmed, and unknown sections are ignored. **Keep the two parsers in sync.**

Every scenario supplies its code this way. That is enforced, not merely conventional: `RealRepository_NoSampleUsesInlineControlExampleCode` fails if any page reintroduces inline `<ControlExample.Xaml>` or `<ControlExample.CSharp>` markup. Inline code renders correctly in the gallery but is invisible to the exporter, so it would otherwise go missing from the index with no visible symptom.

### `$(Token)` placeholders

A page can pair a snippet with `ControlExampleSubstitution` entries that bind a token to one of the interactive option controls, so the code updates as the reader changes a slider or a dropdown. Published verbatim, a token like `$(Spacing)` would leave the snippet impossible to paste into a project — and a token that stands in for a whole attribute, such as `$(IsEnabled)`, would leave the XAML unparseable and therefore discarded entirely.

The exporter resolves each token to the value its control starts with, which is exactly what the gallery renders when the page first loads. Resolution is deliberately conservative and reads only what the markup actually states: a literal `Value`, an initial attribute on the bound control, the item a selector explicitly marks as selected, or a gate on a boolean whose documented default is false (`IsChecked`, `IsOn`, `IsSticky`, `IsOpen`) that the markup never sets. That last rule is an explicit short list rather than a general "unset means false", because `IsEnabled`, `IsTabStop` and `IsHitTestVisible` all default to *true* and a blanket rule would invert them.

Some tokens cannot be settled from markup alone — a converter function such as `BoolToLowerString(x.IsOn)`, a selector populated from code-behind, or a control that declares no initial value and relies on a framework default the exporter does not know. The exporter never guesses one, because publishing a value the gallery does not show would be worse than publishing none.

XAML instead degrades by deletion. `TokenFallback` removes the whole attribute carrying an unresolvable token, which leaves that property at its own default — in the common case, where the token binds the demo control's own property, that is precisely the value the reader sees when the page first loads. A token standing in for a whole attribute, such as `$(IsEnabled)`, is removed the same way. The fragment stays well-formed and pasteable, and the names that were dropped are listed in `gallery.xamlPlaceholdersDropped` so the loss is visible rather than silent. `CatalogGenerator` re-checks each fragment afterwards and fails the build if a token survived, so a published `xaml` value never contains `$(`; `RealRepository_NoPublishedXamlContainsAPlaceholder` pins that guarantee.

`code` cannot degrade the same way, and that asymmetry is a property of the language rather than unfinished work. Deleting a XAML attribute works because the property then falls back to its own default; C# has no construct whose absence yields a default. The tokens that actually occur bear this out: they appear as identifier fragments (`BadgeNotificationGlyph.$(SelectedGlyph)`), as arguments in a fixed-arity call (`SetBorderAndTitleBar($(HasBorder), $(HasTitleBar))`), and as whole statements (`$(TxtFileType)$(JsonFileType)`). Removing any of those leaves either code that does not compile or a sample with its subject cut out. Inferring a value instead would be worse than publishing none, for the reason the resolver already refuses to guess: a `Slider` whose `Minimum` is above zero silently coerces whatever it is handed, and several of these tokens are bound to controls a constructor initializes, so a markup-derived guess would state a value the gallery never shows.

So C# tokens are published verbatim, and the exporter declares that rather than leaving it to be discovered. `gallery.codePlaceholdersPresent` lists the token names a sample's `code` still contains, and is omitted when there are none. **Read the two placeholder fields as opposites.** `xamlPlaceholdersDropped` is a record of cleanup already done — the tokens are gone and the fragment pastes as published. `codePlaceholdersPresent` is a warning about cleanup that is not possible — the tokens are still in the string and the code will not compile as published. A consumer that conflates them pastes broken C#, which is precisely what the field exists to prevent. Treat `code` as templated whenever it is present, and use the sample's page as the reference for real values: the gallery resolves these tokens at runtime from the live option controls.

A substitution whose `IsEnabled` resolves to false becomes the empty string, matching `ControlExampleSubstitution.ValueAsString`. Literal values keep their surrounding whitespace, because some snippets rely on a value such as `` IsSticky="True" `` to supply its own separating spaces.

`SubstitutionResolverTests` pins these rules, and several of its cases assert that an ambiguous binding is skipped rather than guessed.

### Samples that carry no XAML, and scenarios that are left out

The contract requires a sample to carry XAML or code, and its consumer skips any that has neither. Two situations are handled explicitly.

**A scenario with no code at all is omitted from the index.** This happens when a page sets `SourceCodeVisibility="Collapsed"`, so the gallery deliberately shows no code, or when a page swaps `ControlExample.XamlSource` at runtime and no single snippet represents the scenario.

**A snippet whose XAML is not a well-formed fragment keeps its C# and loses its XAML.** Several snippets are written for the gallery's own code viewer, where a human correctly reads `<Window ...>` as "your existing window". That is not parseable XML, and a consumer parses each fragment and discards whatever fails *without reporting it* — so publishing it would advertise code that never arrives. The exporter omits the XAML instead, prints a warning during `generate`, and marks the sample with `gallery.xamlOmittedAsMalformed` so the omission is visible rather than looking like a sample that simply has no XAML.

`RealRepository_SnippetsWithUnpublishableXamlAreTheKnownSet` pins the current set, so a newly broken snippet surfaces as a test failure instead of quietly disappearing. Every snippet in that set still publishes its C#.

### The optional `Catalog` override block

`ControlInfoData.json` items may include an optional `Catalog` object (see `ControlInfoDataSchema.json`) for the rare case where the exporter can't safely derive something on its own:

```jsonc
"Catalog": {
  "Exclude": false,           // drop this item from the index entirely
  "Aliases": [ "cta" ],       // extra author-written search keywords, additive to Tags
  "RelatedSamples": [ "owner/other-repo#sample-id" ] // cross-repository related samples
}
```

This block is read only by `tools/CatalogExporter` — the running gallery app never looks at it — and is expected to stay rare. `Button` and `ScratchPad` in `ControlInfoData.json` each set `Catalog.Aliases` as a small, working example of the mechanism.

### Inclusion and exclusion rules

An item from `ControlInfoData.json` becomes a control in the index when, and only when:

1. It has a non-empty `UniqueId` that is unique across the file.
2. A `WinUIGallery/Samples/<UniqueId>/` folder exists with a case-exact `<UniqueId>Page.xaml` file in it.
3. It does not set `Catalog.Exclude: true`.

Anything else — a missing folder or page, a duplicate id, a `RelatedControls`/`Catalog.RelatedSamples` reference that doesn't resolve to an included entry, a `SampleDefinition` snippet that doesn't exist on disk, or two `ControlExample` elements pointing at the same snippet — fails validation (`CatalogValidationException`) rather than being silently skipped or guessed at. All 120 current `ControlInfoData.json` items satisfy these rules.

### What's intentionally left out

Optional fields are omitted rather than filled with guesses: `description`, `apiNamespace`, `docs`, `relatedControls` and the rest are absent when the source data has nothing to report. The index does not pin a commit SHA — it reflects the repository's default branch and is regenerated whenever samples change.
