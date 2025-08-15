// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Helpers;

public static partial class NavigationOrientationHelper
{
    public static bool IsLeftMode()
    {
        return SettingsHelper.Current.IsLeftMode;
    }

    public static void IsLeftModeForElement(bool isLeftMode)
    {
        UpdateNavigationViewForElement(isLeftMode);
        SettingsHelper.Current.IsLeftMode = isLeftMode;
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
