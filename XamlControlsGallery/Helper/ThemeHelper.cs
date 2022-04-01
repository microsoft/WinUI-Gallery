using System;
using Windows.Storage;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

#if UNIVERSAL
using Windows.UI.ViewManagement;
using Windows.System;
#endif

namespace AppUIBasics.Helper
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeHelper
    {
        private const string SelectedAppThemeKey = "SelectedAppTheme";
        private static Window CurrentApplicationWindow;
        // Keep reference so it does not get optimized/garbage collected
#if UNIVERSAL
        private static UISettings uiSettings;
#endif
        /// <summary>
        /// Gets the current actual theme of the app based on the requested theme of the
        /// root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme
        {
            get
            {
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        if (rootElement.RequestedTheme != ElementTheme.Default)
                        {
                            return rootElement.RequestedTheme;
                        }
                    }
                }

                return AppUIBasics.App.GetEnum<ElementTheme>(App.Current.RequestedTheme.ToString());
            }
        }

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        return rootElement.RequestedTheme;
                    }
                }

                return ElementTheme.Default;
            }
            set
            {
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = value;
                    }
                }

                ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey] = value.ToString();
                UpdateSystemCaptionButtonColors();
            }
        }

        public static void Initialize()
        {
#if !UNPACKAGED
            // Save reference as this might be null when the user is in another app
            CurrentApplicationWindow = App.StartupWindow;
            string savedTheme = ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey]?.ToString();

            if (savedTheme != null)
            {
                RootTheme = AppUIBasics.App.GetEnum<ElementTheme>(savedTheme);
            }
#endif
#if UNIVERSAL
            // Registering to color changes, thus we notice when user changes theme system wide
            uiSettings = new UISettings();
            uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
#endif
        }

#if UNIVERSAL
        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            // Make sure we have a reference to our window so we dispatch a UI change
            if (CurrentApplicationWindow != null)
            {
                // Dispatch on UI thread so that we have a current appbar to access and change
                CurrentApplicationWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                        {
                            UpdateSystemCaptionButtonColors();
                        });
            }
        }
#endif

        public static bool IsDarkTheme()
        {
            if (RootTheme == ElementTheme.Default)
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            }
            return RootTheme == ElementTheme.Dark;
        }

        public static void UpdateSystemCaptionButtonColors()
        {
#if UNIVERSAL
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (ThemeHelper.IsDarkTheme())
            {
                titleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.Black;
            }
#endif
        }
    }
}
