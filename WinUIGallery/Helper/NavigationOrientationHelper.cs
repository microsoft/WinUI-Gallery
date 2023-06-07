using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;


namespace AppUIBasics.Helper
{
    public static class NavigationOrientationHelper
    {

        private const string IsLeftModeKey = "NavigationIsOnLeftMode";

#if UNPACKAGED
        private static bool _isLeftMode = true;
#endif

        public static bool IsLeftMode()
        {
#if !UNPACKAGED
            var valueFromSettings = ApplicationData.Current.LocalSettings.Values[IsLeftModeKey];
            if(valueFromSettings == null)
            {
                ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = true;
                valueFromSettings = true;
            }
            return (bool)valueFromSettings;
#else
            return _isLeftMode;
#endif
        }

        public static void IsLeftModeForElement(bool isLeftMode, UIElement element)
        {
            UpdateNavigationViewForElement(isLeftMode, element);
#if !UNPACKAGED
            ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = isLeftMode;
#else
            _isLeftMode = isLeftMode;
#endif
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
}
