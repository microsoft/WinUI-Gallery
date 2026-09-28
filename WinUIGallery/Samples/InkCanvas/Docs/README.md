<!-- Copyright (c) Microsoft Corporation. All rights reserved. -->
<!-- Licensed under the MIT License. -->

# InkCanvas snippets

Copy-and-paste examples for `Microsoft.UI.Xaml.Controls.InkCanvas`. Each page has a XAML fragment, the `using` directives and the code-behind members, plus Light and Dark screenshots. The ink surface is white in both themes.

| Example | Shows |
|---|---|
| [Choose which devices can ink](InputDeviceTypes.md) | `InputDeviceTypes` flags: add or remove pen, mouse and touch |
| [Pen presets](DrawingAttributes.md) | Ballpoint, pencil (`CreateForPencil`), highlighter and a calligraphy nib (`PenTipTransform`) |
| [Save ink as a GIF with embedded ISF](SaveLoadIsf.md) | `SaveAsync(..., GifWithEmbeddedIsf)`, `LoadAsync` and a preview of the saved file |
| [Copy ink between canvases](Clipboard.md) | `CopySelectedToClipboard`, `CanPasteFromClipboard`, `PasteFromClipboard` |
| [Create strokes in code](StrokeBuilder.md) | `InkStrokeBuilder.CreateStroke` and `AddStroke` for shapes and a signature line |

## Using a snippet

1. Paste the XAML inside your page's root `Grid` (or any panel).
2. Add the `using` directives to the top of the code-behind file.
3. Paste the members into the page class and call `InitializeInking();` right after `InitializeComponent();`.

Every snippet was built and run as written, next to the default usings of a new **Blank Page (WinUI 3)**, on **Windows App SDK 2.4.1-experimental**. `InkCanvas` is experimental in that release, so suppress `CS8305` in your project (`<NoWarn>$(NoWarn);CS8305</NoWarn>`).

`Microsoft.UI.Xaml.Controls` and `Windows.UI.Input.Inking` both declare `InkPresenter`, `InkStrokeContainer` and a few other `Ink*` types. The snippets import the WinUI namespace and alias the `Windows.UI.Input.Inking` types they use (for example `using InkStroke = Windows.UI.Input.Inking.InkStroke;`), which avoids ambiguity errors.
