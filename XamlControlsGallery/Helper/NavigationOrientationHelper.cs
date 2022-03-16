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

        public static bool IsLeftMode
        {
            get
            {
                var valueFromSettings = ApplicationData.Current.LocalSettings.Values[IsLeftModeKey];
                if(valueFromSettings == null)
                {
                    ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = true;
                    valueFromSettings = true;
                }
                return (bool)valueFromSettings;
            }

            set
            {
                UpdateTitleBar(value);
                ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = value;
            }
        }

        public static void UpdateTitleBar(bool isLeftMode)
        {
#if UNIVERSAL
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = isLeftMode;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
#endif
            if (isLeftMode)
            {
                NavigationRootPage.Current.NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;
#if UNIVERSAL
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
#endif
            }
            else
            {
                NavigationRootPage.Current.NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
#if UNIVERSAL
                var userSettings = new UISettings();
                titleBar.ButtonBackgroundColor = userSettings.GetColorValue(UIColorType.Accent);
                titleBar.ButtonInactiveBackgroundColor = userSettings.GetColorValue(UIColorType.Accent);
#endif
            }
        }
    }
}
