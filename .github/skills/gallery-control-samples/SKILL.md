---
name: gallery-control-samples
description: How to design and build control sample pages in WinUI Gallery. Use when adding or changing a control page, a ControlExample, a sample snippet (.txt), or a ControlInfoData.json entry. Covers page structure, keeping description text to a minimum, placing options and output in the side panels, snippet format, and validation.
---

# Building control samples

A control page teaches a control through a short series of focused examples. The control should speak for itself. Text, options, and output support it and stay out of its way.

## Page structure

- Start with the simplest useful example, then add one example per concept (configuration, states, input, events, data binding, customization). Each `ControlExample` teaches one idea.
- Keep sample data small and neutral. Avoid business scenarios unless the control needs one.
- The control's overview belongs in its `Description` in `WinUIGallery/SampleSupport/Data/ControlInfoData.json`, not on the page.
- Put the control in the right group in `ControlInfoData.json` and keep items within a group in alphabetical order by title.

## Keep description text to a minimum

- Don't add prose `TextBlock`s that explain the API inside or above an example. Most gallery examples have none.
- Let the example header carry the idea. Write it as a short, specific phrase, for example "Create a bar chart and sort its categories".
- Explain non-obvious code with a brief comment in the snippet's C# or XAML, where developers copy it.
- Use one short sentence only when the example can't be understood without it, for example a behavior that isn't visible. Never repeat what the header, the UI, or the docs already say.

## Example, Options, and Output

`ControlExample` has three areas. Put each piece in the right one:

| Area | Contains |
| --- | --- |
| `Example` (main content) | Only the control being demonstrated, plus minimal layout it needs. |
| `ControlExample.Options` (right panel) | Anything the user changes or triggers to affect the example: toggles, combo boxes, sliders, and action buttons such as "Add item" or "Reset". |
| `ControlExample.Output` (column labeled "Output:") | Results and state produced by the example: event messages, selected values, counts. |

- Don't put action buttons or result text in the example area next to the control.
- Keep Output text short. The column is at most 320 pixels wide, so prefer "Latest: 165 ms" over a full list of values. Set `AutomationProperties.LiveSetting="Polite"` on output that changes.
- Top-align options. A lone control in `Options` stretches and centers vertically, so set `VerticalAlignment="Top"` on it, or wrap several options in a `StackPanel`.
- Give options a `Header` (or an `AutomationProperties.Name`) so they make sense on their own.

```xml
<controls:ControlExample SampleDefinition="MyControl\BasicExample.txt">
    <local:MyControl x:Name="Sample" IsCompact="{x:Bind CompactToggle.IsOn, Mode=OneWay}" />
    <controls:ControlExample.Output>
        <TextBlock AutomationProperties.LiveSetting="Polite" Text="{x:Bind Status, Mode=OneWay}" />
    </controls:ControlExample.Output>
    <controls:ControlExample.Options>
        <ToggleSwitch
            x:Name="CompactToggle"
            VerticalAlignment="Top"
            Header="Compact" />
    </controls:ControlExample.Options>
    <controls:ControlExample.Substitutions>
        <controls:ControlExampleSubstitution Key="IsCompact" Value="{x:Bind CompactToggle.IsOn, Mode=OneWay}" />
    </controls:ControlExample.Substitutions>
</controls:ControlExample>
```

## Sample snippets

Snippets live next to the page in `WinUIGallery/Samples/{UniqueId}/` and are referenced with `SampleDefinition`:

```text
--- header
Create a basic example
--- xaml
<local:MyControl IsCompact="$(IsCompact)" />
--- c#
// Only the code a developer needs to reproduce the example.
```

- Show what a developer would copy: the control and the code behind it. Leave out gallery plumbing such as Output text, AutomationIds, and option controls.
- Make the XAML self-contained. Declare any non-default XML namespace on the snippet's root element, for example `xmlns:charts="using:Microsoft.UI.Xaml.Controls.Charts"`.
- Use `$(Key)` substitutions so the snippet reflects the current option values.
- Keep the snippet in sync with the page. If the page behavior changes, update the snippet.

## Accessibility

- Give the demonstrated control an `AutomationProperties.Name`, and a `HelpText` when it shows data that isn't otherwise readable, such as a chart.
- Every option and action button needs an accessible name.
- Don't rely on color alone to convey meaning.

## Validate

1. Regenerate the catalog and run its tests:

   ```powershell
   dotnet run --project tools/CatalogExporter -- generate
   dotnet test tests\WinUIGallery.CatalogExporter.Tests
   ```

2. Build and run the app. Open the page and check the result visually: options top-aligned in the right panel, output in the Output column, every action works more than once, and the page looks right in light and dark themes.