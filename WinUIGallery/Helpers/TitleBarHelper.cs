// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace WinUIGallery.Helpers;

internal partial class TitleBarHelper
{
    // workaround as AppWindow TitleBar doesn't update caption button colors correctly when changed while app is running
    // https://task.ms/44172495
    public static void ApplySystemThemeToCaptionButtons(Window window, ElementTheme currentTheme)
    {
        if (window.AppWindow != null)
        {
            var foregroundColor = currentTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
            window.AppWindow.TitleBar.ButtonForegroundColor = foregroundColor;
            window.AppWindow.TitleBar.ButtonHoverForegroundColor = foregroundColor;

            var backgroundHoverColor = currentTheme == ElementTheme.Dark ? Color.FromArgb(24, 255, 255, 255) : Color.FromArgb(24, 0, 0, 0);
            window.AppWindow.TitleBar.ButtonHoverBackgroundColor = backgroundHoverColor;
        }
    }
}
