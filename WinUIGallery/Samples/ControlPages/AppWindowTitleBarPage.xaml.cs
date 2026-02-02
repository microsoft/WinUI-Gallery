// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Windows.UI;
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
        ShowWindowButton.IsEnabled = false;
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
        window.Closed += Window_Closed;
    }

    private void Window_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs args)
    {
        ShowWindowButton.IsEnabled = true;
    }

    private void Background_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && Background.Color != default)
        {
            window.AppWindow.TitleBar.BackgroundColor = Background.Color;
        }
    }

    private void Foreground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && Foreground.Color != default)
        {
            window.AppWindow.TitleBar.ForegroundColor = Foreground.Color;
        }
    }

    private void ButtonBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonBackground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonBackgroundColor = ButtonBackground.Color;
        }
    }

    private void ButtonForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonForeground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonForegroundColor = ButtonForeground.Color;
        }
    }

    private void ButtonHoverBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonHoverBackground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonHoverBackgroundColor = ButtonHoverBackground.Color;
        }
    }

    private void ButtonHoverForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonHoverForeground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonHoverForegroundColor = ButtonHoverForeground.Color;
        }
    }

    private void ButtonInactiveBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonInactiveBackground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonInactiveBackgroundColor = ButtonInactiveBackground.Color;
        }
    }

    private void ButtonInactiveForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonInactiveForeground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonInactiveForegroundColor = ButtonInactiveForeground.Color;
        }
    }

    private void InactiveBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && InactiveBackground.Color != default)
        {
            window.AppWindow.TitleBar.InactiveBackgroundColor = InactiveBackground.Color;
        }
    }

    private void InactiveForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && InactiveForeground.Color != default)
        {
            window.AppWindow.TitleBar.InactiveForegroundColor = InactiveForeground.Color;
        }
    }

    private void ButtonPressedBackground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonPressedBackground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonPressedBackgroundColor = ButtonPressedBackground.Color;
        }
    }

    private void ButtonPressedForeground_ColorChanged(Controls.ColorSelector obj)
    {
        if (window != null && ButtonPressedForeground.Color != default)
        {
            window.AppWindow.TitleBar.ButtonPressedForegroundColor = ButtonPressedForeground.Color;
        }
    }

    private string ColorToArgbString(Color color)
    {
        return $"{color.A}, {color.R}, {color.G}, {color.B}";
    }
}
