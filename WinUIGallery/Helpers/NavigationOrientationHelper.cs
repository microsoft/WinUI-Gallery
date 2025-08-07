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
        return Settings.Current.IsLeftMode;
    }

    public static void IsLeftModeForElement(bool isLeftMode)
    {
        UpdateNavigationViewForElement(isLeftMode);
        Settings.Current.IsLeftMode = isLeftMode;
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
