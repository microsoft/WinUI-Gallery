<!-- Copyright (c) Microsoft Corporation. All rights reserved. -->
<!-- Licensed under the MIT License. -->

# InkToolbar snippets

Copy-and-paste examples for `Microsoft.UI.Xaml.Controls.InkToolbar`. Each page has a XAML fragment, the `using` directives and the code-behind members, plus Light and Dark screenshots. The ink surface is white in both themes.

| Example | Shows |
|---|---|
| [Pick the built-in buttons](InitialControls.md) | `InitialControls` = `All`, `PensOnly`, `AllExceptPens`, `None` + your own button |
| [Custom pen palette](CustomPenPalette.md) | `Palette`, `MinStrokeWidth` / `MaxStrokeWidth`, `SelectedBrushIndex` |
| [Start on a tool and follow changes](ActiveToolChanged.md) | `GetToolButton`, `ActiveTool`, `ActiveToolChanged`, `InkDrawingAttributesChanged` |
| [Dock the toolbar vertically](VerticalToolbar.md) | `Orientation="Vertical"` next to the canvas |
| [Custom toggle for touch writing](CustomToggle.md) | `InkToolbarCustomToggleButton` switching touch input on and off |
| [Build it in code](CodeOnly.md) | `InkToolbar` and `InkCanvas` created and connected in C# |

## Using a snippet

1. Paste the XAML inside your page's root `Grid` (or any panel).
2. Add the `using` directives to the top of the code-behind file.
3. Paste the members into the page class and call `InitializeInking();` right after `InitializeComponent();`.

Every snippet was built and run as written, next to the default usings of a new **Blank Page (WinUI 3)**, on **Windows App SDK 2.4.1-experimental**. `InkToolbar` is experimental in that release, so suppress `CS8305` in your project (`<NoWarn>$(NoWarn);CS8305</NoWarn>`).
