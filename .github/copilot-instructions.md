# Copilot Instructions for WinUI Gallery

## Build and Test

**Solution:** Open `WinUIGallery.sln` in Visual Studio 2022+. Set `WinUIGallery` as the startup project.

```powershell
# Restore and build
dotnet restore WinUIGallery.sln
msbuild WinUIGallery.sln /p:Configuration=Debug /p:Platform=x64

# Run unit tests
dotnet test tests\WinUIGallery.UnitTests\WinUIGallery.UnitTests.csproj

# Run a single unit test
dotnet test tests\WinUIGallery.UnitTests\WinUIGallery.UnitTests.csproj --filter "FullyQualifiedName~TestMethodName"

# UI tests require the app to be running; use Appium + WinAppDriver
dotnet test tests\WinUIGallery.UITests\WinUIGallery.UITests.csproj
```

Build configurations: Debug, Release, Preview, Stable, Store, Sideload, Debug-Unpackaged. Platforms: x86, x64, ARM64.

## Architecture

This is a **WinUI 3 / Windows App SDK** gallery app that demonstrates controls, styles, and design patterns. It is a companion to the Fluent Design guidelines.

### Project structure

- **WinUIGallery** — Main app (XAML + C#, .NET 10, `net10.0-windows10.0.22621.0`)
- **WinUIGallery.SourceGenerator** — Incremental source generator that reads `ControlInfoData.json` at compile time and emits `SamplesNavigationPageMappings.cs` to map control IDs → page types
- **tests/WinUIGallery.UnitTests** — MSTest v3 unit tests (runs in WinUI context)
- **tests/WinUIGallery.UITests** — MSTest + Appium UI automation tests with Axe.Windows accessibility checks

### Data flow

1. `Samples/Data/ControlInfoData.json` defines all control entries grouped by category (Fundamentals, Design, Accessibility, etc.). Each item has `UniqueId`, `Title`, `Description`, `Docs`, `IsNew`/`IsUpdated`/`IsPreview` flags.
2. The **source generator** reads this JSON at build time and generates a mapping from `UniqueId` → XAML page type (e.g., `"Button"` → `typeof(ButtonPage)`).
3. At runtime, `ControlInfoDataSource` loads and deserializes the JSON, then uses the generated mappings to wire up navigation.
4. `App.xaml.cs` initializes data sources, populates NavigationView, and handles protocol activation for deep-linking to specific controls.

### Control pages

Each control demo lives in `Samples/ControlPages/` as a `[ControlName]Page.xaml` + `.xaml.cs` pair. Pages use the `ControlExample` custom control to show interactive demos:

```xml
<controls:ControlExample HeaderText="A basic button">
    <controls:ControlExample.Example>
        <Button Content="Click me" Click="Button_Click" />
    </controls:ControlExample.Example>
    <controls:ControlExample.Options>
        <CheckBox Content="Toggle something" />
    </controls:ControlExample.Options>
    <controls:ControlExample.Xaml>
        <x:String>$(XamlSnippet)</x:String>
    </controls:ControlExample.Xaml>
    <controls:ControlExample.Substitutions>
        <controls:ControlExampleSubstitution Key="XamlSnippet" Value="{x:Bind ...}" />
    </controls:ControlExample.Substitutions>
</controls:ControlExample>
```

`ControlExample` exposes: `Example`, `Output`, `Options`, `Xaml`/`XamlSource`, `CSharp`/`CSharpSource`, and `Substitutions` for dynamic `$(Key)` replacements in displayed code.

### Sample code files

`Samples/SampleCode/` contains `.txt` files with raw XAML/C# snippets displayed via `XamlSource`/`CSharpSource`. Naming: `{ControlName}Sample{N}_{xaml|cs|csharp}.txt`.

## Adding a New Control Page

1. Add the control entry to `Samples/Data/ControlInfoData.json` with a unique `UniqueId` matching the page class name (without `Page` suffix).
2. Create `Samples/ControlPages/[ControlName]Page.xaml` and `.xaml.cs` using the `ControlExample` pattern above.
3. The source generator auto-maps the `UniqueId` to the page type — no manual registration needed.
4. Add any external code snippets to `Samples/SampleCode/` as `.txt` files.

## Accessibility

All new UI must be accessible. The project enforces this through automated Axe.Windows scans that run against every control page in the UI test suite.

- **Set `AutomationProperties.Name`** on all interactive controls (buttons, text boxes, sliders, toggles) so screen readers can announce them.
- **Use `AutomationProperties.HeadingLevel`** (`Level2`, `Level3`) on section headers for semantic navigation.
- **Hide decorative elements** from the accessibility tree with `AutomationProperties.AccessibilityView="Raw"`.
- **Support keyboard navigation** — ensure all interactive elements are reachable via Tab and operable via Enter/Space.
- **Meet WCAG color contrast requirements** — the gallery includes a color contrast checker page as a reference.
- New control pages are automatically picked up by `AxeScanAllTests` which navigates to every page and asserts zero accessibility violations.

## Documentation Reference

When looking up API references, control usage, or platform guidance, use the Microsoft Learn MCP server at `https://learn.microsoft.com/en-us/training/support/mcp`. This covers all Windows developer documentation. This project is **WinUI 3 / Windows App SDK** — always prefer WinUI 3 docs over WPF, MAUI, or UWP equivalents.

Key reference repositories:

- **[microsoft/microsoft-ui-xaml](https://github.com/microsoft/microsoft-ui-xaml)** — WinUI 3 source code (controls, theming, input handling)
- **[microsoft/WindowsAppSDK](https://github.com/microsoft/WindowsAppSDK)** — Windows App SDK (app lifecycle, windowing, deployment)
- **[microsoft/WindowsAppSDK-Samples](https://github.com/microsoft/WindowsAppSDK-Samples)** — Official samples demonstrating Windows App SDK features
- **[microsoft/ai-dev-gallery](https://github.com/microsoft/ai-dev-gallery)** — AI Dev Gallery, a WinUI 3 app showcasing on-device AI models and APIs

## Conventions

- **File-scoped namespaces** are enforced (`csharp_style_namespace_declarations = file_scoped`, severity: warning).
- **File headers** are required: `// Copyright (c) Microsoft Corporation. All rights reserved.` / `// Licensed under the MIT License.`
- **No `var` for built-in types** — use explicit types (`csharp_style_var_for_built_in_types = false`).
- **No `this.` qualifier** — access members directly without `this.`.
- **Allman brace style** — opening braces on their own line (`csharp_new_line_before_open_brace = all`).
- **4-space indentation**, CRLF line endings, no final newline.
- **Pattern matching** preferred over `as`/`is` with null checks.
- **Accessibility modifiers** required on all non-interface members.
- **PascalCase** for types, methods, and properties. Interfaces prefixed with `I`.
- **JSON serialization** uses `System.Text.Json` with source generators (`[JsonSerializable]`), not Newtonsoft.
- **XAML namespaces**: `xmlns:controls="using:WinUIGallery.Controls"` for custom controls.
- Pages inherit from `ItemsPageBase` (which implements `INotifyPropertyChanged`) when they need data binding to control info items.
