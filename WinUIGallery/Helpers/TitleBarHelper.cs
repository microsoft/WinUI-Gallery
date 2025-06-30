// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace WinUIGallery.Helpers;


internal class TitleBarHelper
{
    // workaround as Appwindow titlebar doesn't update caption button colors correctly when changed while app is running
    // https://task.ms/44172495
    public static Windows.UI.Color ApplySystemThemeToCaptionButtons(Window window)
    {
        var frame = (Application.Current as WinUIGallery.App).GetRootFrame() as FrameworkElement;
        Windows.UI.Color color;
        if (frame.ActualTheme == ElementTheme.Dark)
        {
            color = Colors.White;
        }
        else
        {
            color = Colors.Black;
        }
        SetCaptionButtonColors(window, color);
        return color;
    }

    public static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
    {
        var res = Application.Current.Resources;
        res["WindowCaptionForeground"] = color;
        window.AppWindow.TitleBar.ButtonForegroundColor = color;
    }

    public static void SetCaptionButtonBackgroundColors(Window window, Windows.UI.Color? color)
    {
        var titleBar = window.AppWindow.TitleBar;
        titleBar.ButtonBackgroundColor = color;
    }

    public static void SetForegroundColor(Window window, Windows.UI.Color? color)
    {
        var titleBar = window.AppWindow.TitleBar;
        titleBar.ForegroundColor = color;
    }

    public static void SetBackgroundColor(Window window, Windows.UI.Color? color)
    {
        var titleBar = window.AppWindow.TitleBar;
        titleBar.BackgroundColor = color;
    }
}
