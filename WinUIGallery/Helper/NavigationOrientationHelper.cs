using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;


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
            UpdateTitleBarForElement(isLeftMode, element);
#if !UNPACKAGED
            ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = isLeftMode;
#else
            _isLeftMode = isLeftMode;
#endif
        }

        public static void UpdateTitleBarForElement(bool isLeftMode, UIElement element)
        {
            var window = WindowHelper.GetWindowForElement(element);
            window.ExtendsContentIntoTitleBar = isLeftMode;

            if (isLeftMode)
            {
                NavigationRootPage.GetForElement(element).NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;
            }
            else
            {
                NavigationRootPage.GetForElement(element).NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
            }
        }
    }
}
