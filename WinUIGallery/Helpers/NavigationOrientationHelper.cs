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

    public static void IsLeftModeForElement(bool isLeftMode)
    {
        UpdateNavigationViewForElement(isLeftMode);
        if (NativeHelper.IsAppPackaged)
        {
            ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = isLeftMode;
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
