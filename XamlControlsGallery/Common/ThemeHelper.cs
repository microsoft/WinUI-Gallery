using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace AppUIBasics.Common
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeHelper
    {
        private const string SelectedAppThemeKey = "SelectedAppTheme";

        /// <summary>
        /// Gets the current actual theme of the app based on the requested theme of the
        /// root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    if (rootElement.RequestedTheme != ElementTheme.Default)
                    {
                        return rootElement.RequestedTheme;
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
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }

                ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey] = value.ToString();
                UpdateSystemCaptionButtonColors();
            }
        }

        public static void Initialize()
        {

            string savedTheme = ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey]?.ToString();

            if (savedTheme != null)
            {
                RootTheme = AppUIBasics.App.GetEnum<ElementTheme>(savedTheme);
            }
        }

        public static bool IsDarkTheme()
        {
            if(RootTheme == ElementTheme.Default)
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            }
            return RootTheme == ElementTheme.Dark;
        }

        public static void UpdateSystemCaptionButtonColors()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (ThemeHelper.IsDarkTheme())
            {
                titleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.Black;
            }
        }
    }
}
