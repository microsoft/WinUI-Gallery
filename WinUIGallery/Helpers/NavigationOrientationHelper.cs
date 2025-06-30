// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage;
using WinUIGallery.Pages;

namespace WinUIGallery.Helpers;

public static class NavigationOrientationHelper
{
    private static bool _isLeftMode = true;
    private static ApplicationData appData = ApplicationData.GetDefault();
    public static bool IsLeftMode()
    {
        if (NativeHelper.IsAppPackaged)
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

    public static void IsLeftModeForElement(bool isLeftMode, UIElement element)
    {
        UpdateNavigationViewForElement(isLeftMode, element);
        if (NativeHelper.IsAppPackaged)
        {
            appData.LocalSettings.Values[SettingsKeys.IsLeftMode] = isLeftMode;
        }
        else
        {
            _isLeftMode = isLeftMode;
        }
    }

    public static void UpdateNavigationViewForElement(bool isLeftMode, UIElement element)
    {
        NavigationView _navView = NavigationRootPage.GetForElement(element).NavigationView;
        if (isLeftMode)
        {
            _navView.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
            Grid.SetRow(_navView, 0);
        }
        else
        {
            _navView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
            Grid.SetRow(_navView, 1);
        }
    }

}
