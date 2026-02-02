// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Samples.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class AppWindowTitleBarPage : Page
{
    private AppWindowTitleBarWindow? window;
    public AppWindowTitleBarPage()
    {
        InitializeComponent();
    }

    private void ShowWindowButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        window = new AppWindowTitleBarWindow(
            Background.Color,
            Foreground.Color,
            ButtonBackground.Color,
            ButtonForeground.Color,
            ButtonHoverBackground.Color,
            ButtonHoverForeground.Color,
            ButtonInactiveBackground.Color,
            ButtonInactiveForeground.Color,
            InactiveBackground.Color,
            InactiveForeground.Color,
            ButtonPressedBackground.Color,
            ButtonPressedForeground.Color);
        window.Activate();
    }

    private void Background_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.BackgroundColor = Background.Color;
        }
    }

    private void Foreground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ForegroundColor = Foreground.Color;
        }
    }

    private void ButtonBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonBackgroundColor = ButtonBackground.Color;
        }
    }

    private void ButtonForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonForegroundColor = ButtonForeground.Color;
        }
    }

    private void ButtonHoverBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonHoverBackgroundColor = ButtonHoverBackground.Color;
        }
    }

    private void ButtonHoverForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonHoverForegroundColor = ButtonHoverForeground.Color;
        }
    }

    private void ButtonInactiveBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonInactiveBackgroundColor = ButtonInactiveBackground.Color;
        }
    }

    private void ButtonInactiveForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonInactiveForegroundColor = ButtonInactiveForeground.Color;
        }
    }

    private void InactiveBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.InactiveBackgroundColor = InactiveBackground.Color;
        }
    }

    private void InactiveForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.InactiveForegroundColor = InactiveForeground.Color;
        }
    }

    private void ButtonPressedBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonPressedBackgroundColor = ButtonPressedBackground.Color;
        }
    }

    private void ButtonPressedForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window is not null)
        {
            window.AppWindow.TitleBar.ButtonPressedForegroundColor = ButtonPressedForeground.Color;
        }
    }
}
