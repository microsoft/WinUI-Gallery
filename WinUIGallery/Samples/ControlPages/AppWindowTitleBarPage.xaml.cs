// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.UI;
using WinUIGallery.Pages;
using WinUIGallery.Samples.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class AppWindowTitleBarPage : Page
{
    private AppWindowTitleBarWindow? window;
    private AppWindowTitleBarExtendWindow? extendWindow;
    private AppWindowTitleBarThemeWindow? themeHeightWindow;
    private IReadOnlyList<TitleBarTheme> titleBarThemes { get; set; } = new List<TitleBarTheme>(Enum.GetValues<TitleBarTheme>());
    private TitleBarTheme selectedTheme = TitleBarTheme.UseDefaultAppMode;
    private IReadOnlyList<TitleBarHeightOption> titleBarHeightOptions { get; set; } = new List<TitleBarHeightOption>(Enum.GetValues<TitleBarHeightOption>());
    private TitleBarHeightOption selectedHeight = TitleBarHeightOption.Standard;

    public AppWindowTitleBarPage()
    {
        InitializeComponent();
    }
    private void Hyperlink_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
    {
        App.MainWindow.Navigate(typeof(ItemPage), "TitleBar");
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
        if (window != null)
            window.Closed -= Window_Closed;
        window = null;
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

    private string ColorToArgbString(Color color) => $"{color.A}, {color.R}, {color.G}, {color.B}";

    private string BoolToLowerString(bool? value) => value.ToString().ToLower();

    private void ShowExtendButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShowExtendButton.IsEnabled = false;
        extendWindow = new AppWindowTitleBarExtendWindow(
            ExtendContentCheckBox.IsChecked ?? false,
            (TitleBarHeightOption)HeightComboBox.SelectedItem);
        extendWindow.Activate();
        extendWindow.Closed += ExtendWindow_Closed;
    }

    private void ExtendWindow_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs args)
    {
        ShowExtendButton.IsEnabled = true;
        if (extendWindow != null)
            extendWindow.Closed -= ExtendWindow_Closed;
        extendWindow = null;
    }

    private void ExtendContentCheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (extendWindow != null)
        {
            extendWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        }
    }

    private void ExtendContentCheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (extendWindow != null)
        {
            extendWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = false;
        }
    }

    private void HeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (extendWindow != null && extendWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar)
        {
            extendWindow.AppWindow.TitleBar.PreferredHeightOption = ((TitleBarHeightOption)HeightComboBox.SelectedItem);
        }
    }

    private void ShowThemeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ShowThemeHeightButton.IsEnabled = false;
        themeHeightWindow = new AppWindowTitleBarThemeWindow((TitleBarTheme)ThemeComboBox.SelectedItem);
        themeHeightWindow.Activate();
        themeHeightWindow.Closed += ThemeHeightWindow_Closed;
    }

    private void ThemeHeightWindow_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs args)
    {
        ShowThemeHeightButton.IsEnabled = true;
        if (themeHeightWindow != null)
            themeHeightWindow.Closed -= ThemeHeightWindow_Closed;
        themeHeightWindow = null;
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (themeHeightWindow != null)
        {
            themeHeightWindow.AppWindow.TitleBar.PreferredTheme = ((TitleBarTheme)ThemeComboBox.SelectedItem);
        }
    }
}
