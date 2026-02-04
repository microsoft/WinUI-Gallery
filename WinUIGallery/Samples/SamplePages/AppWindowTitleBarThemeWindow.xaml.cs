// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class AppWindowTitleBarThemeWindow : Window
{
    public AppWindowTitleBarThemeWindow(TitleBarTheme titleBarTheme)
    {
        InitializeComponent();

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsResizable = false;

        AppWindow.SetPresenter(presenter);
        AppWindow.Resize(new Windows.Graphics.SizeInt32(600, 400));

        AppWindow.TitleBar.PreferredTheme = titleBarTheme;
    }
}
