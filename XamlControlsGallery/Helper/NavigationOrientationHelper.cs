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

#if UNIVERSAL
using Windows.UI.ViewManagement;
#endif

namespace AppUIBasics.Helper
{
    public static class NavigationOrientationHelper
    {

        private const string IsLeftModeKey = "NavigationIsOnLeftMode";

        public static bool IsLeftMode()
        {
            var valueFromSettings = ApplicationData.Current.LocalSettings.Values[IsLeftModeKey];
            if(valueFromSettings == null)
            {
                ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = true;
                valueFromSettings = true;
            }
            return (bool)valueFromSettings;
        }

        public static void IsLeftModeForElement(bool isLeftMode, UIElement element)
        {
            UpdateTitleBarForElement(isLeftMode, element);
            ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = isLeftMode;
        }

        public static void UpdateTitleBarForElement(bool isLeftMode, UIElement element)
        {
#if UNIVERSAL
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = isLeftMode;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
#endif
            if (isLeftMode)
            {
                NavigationRootPage.GetForElement(element).NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;
#if UNIVERSAL
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
#endif
            }
            else
            {
                NavigationRootPage.GetForElement(element).NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
#if UNIVERSAL
                var userSettings = new UISettings();
                titleBar.ButtonBackgroundColor = userSettings.GetColorValue(UIColorType.Accent);
                titleBar.ButtonInactiveBackgroundColor = userSettings.GetColorValue(UIColorType.Accent);
#endif
            }
        }
    }
}
