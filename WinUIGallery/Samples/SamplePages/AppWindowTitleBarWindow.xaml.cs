// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class AppWindowTitleBarWindow : Window
{
    public AppWindowTitleBarWindow(Color BackgroundColor, Color ForegroundColor, Color ButtonBackgroundColor, Color ButtonForegroundColor,
        Color ButtonHoverBackgroundColor, Color ButtonHoverForegroundColor, Color ButtonInactiveBackgroundColor, Color ButtonInactiveForegroundColor,
        Color InactiveBackgroundColor, Color InactiveForegroundColor, Color ButtonPressedBackgroundColor, Color ButtonPressedForegroundColor)
    {
        InitializeComponent();

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsResizable = false;

        AppWindow.SetPresenter(presenter);
        AppWindow.Resize(new Windows.Graphics.SizeInt32(400, 400));
        AppWindow.TitleBar.BackgroundColor = BackgroundColor;
        AppWindow.TitleBar.ForegroundColor = ForegroundColor;
        AppWindow.TitleBar.ButtonBackgroundColor = ButtonBackgroundColor;
        AppWindow.TitleBar.ButtonForegroundColor = ButtonForegroundColor;
        AppWindow.TitleBar.ButtonHoverBackgroundColor = ButtonHoverBackgroundColor;
        AppWindow.TitleBar.ButtonHoverForegroundColor = ButtonHoverForegroundColor;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = ButtonInactiveBackgroundColor;
        AppWindow.TitleBar.ButtonInactiveForegroundColor = ButtonInactiveForegroundColor;
        AppWindow.TitleBar.InactiveBackgroundColor = InactiveBackgroundColor;
        AppWindow.TitleBar.InactiveForegroundColor = InactiveForegroundColor;
        AppWindow.TitleBar.ButtonPressedBackgroundColor = ButtonPressedBackgroundColor;
        AppWindow.TitleBar.ButtonPressedForegroundColor = ButtonPressedForegroundColor;
    }

    public AppWindowTitleBarWindow()
    {
        InitializeComponent();

        AppWindow.TitleBar.BackgroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ForegroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonBackgroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonForegroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonHoverForegroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonInactiveForegroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.InactiveBackgroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.InactiveForegroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonPressedBackgroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
        AppWindow.TitleBar.ButtonPressedForegroundColor = ColorHelper.FromArgb(0, 0, 0, 0);
    }
}
