// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage;

namespace WinUIGallery.Helpers;

public static partial class NavigationOrientationHelper
{
    private static bool _isLeftMode = true;
    private static ApplicationData appData = ApplicationData.GetDefault();
    public static bool IsLeftMode()
    {
        if (NativeMethods.IsAppPackaged)
        {
            var valueFromSettings = appData.LocalSettings.Values[SettingsKeys.IsLeftMode];
            if (valueFromSettings == null)
            {
                appData.LocalSettings.Values[SettingsKeys.IsLeftMode] = true;
                valueFromSettings = true;
            }
            return (bool)valueFromSettings;
        }
        else
        {
            return _isLeftMode;
        }
    }

    public static void IsLeftModeForElement(bool isLeftMode)
    {
        UpdateNavigationViewForElement(isLeftMode);
        if (NativeMethods.IsAppPackaged)
        {
            appData.LocalSettings.Values[SettingsKeys.IsLeftMode] = isLeftMode;
        }
        else
        {
            _isLeftMode = isLeftMode;
        }
    }

    public static void UpdateNavigationViewForElement(bool isLeftMode)
    {
        NavigationView _navView = App.MainWindow.NavigationView;
        if (isLeftMode)
        {
            _navView.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
        }
        else
        {
            _navView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
        }
    }
}
