// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using WinUIGallery.Pages;

namespace WinUIGallery.Helpers;

public static class NavigationOrientationHelper
{

    private const string IsLeftModeKey = "NavigationIsOnLeftMode";
    private static bool _isLeftMode = true;

    public static bool IsLeftMode()
    {
        if (NativeHelper.IsAppPackaged)
        {
            var valueFromSettings = ApplicationData.Current.LocalSettings.Values[IsLeftModeKey];
            if (valueFromSettings == null)
            {
                ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = true;
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
            ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = isLeftMode;
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
