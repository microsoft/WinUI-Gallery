using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Colors = Microsoft.UI.Colors;

namespace AppUIBasics.Helper
{
    public static class ThemeHelper
    {
        private const string SelectedAppThemeKey = "SelectedAppTheme";
        private static Window _mainWindow;
        private static UISettings _uiSettings;
        private static ElementTheme _rootTheme = ElementTheme.Default;

        public static ElementTheme ActualTheme =>
            _mainWindow?.Content is FrameworkElement root
                ? root.ActualTheme
                : Application.Current.RequestedTheme == ApplicationTheme.Dark
                    ? ElementTheme.Dark : ElementTheme.Light;

        public static ElementTheme RootTheme
        {
            get => _rootTheme;
            set
            {
                if (!Enum.IsDefined(typeof(ElementTheme), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _rootTheme = value;
                foreach (var window in WindowHelper.Windows)
                {
                    if (window.Content is FrameworkElement root)
                    {
                        root.RequestedTheme = value;
                    }
                }

                ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey] = value.ToString();
                UpdateSystemCaptionButtonColors();
            }
        }

        public static void Initialize(Window window)
        {
            if (_mainWindow != null)
            {
                return;
            }

            _mainWindow = window;
            string savedTheme = ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey]?.ToString();
            if (savedTheme != null)
            {
                RootTheme = App.GetEnum<ElementTheme>(savedTheme);
            }

            if (window.Content is FrameworkElement root)
            {
                root.ActualThemeChanged += (_, _) => UpdateSystemCaptionButtonColors();
            }

            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
            window.Closed += (_, _) => _uiSettings.ColorValuesChanged -= UiSettings_ColorValuesChanged;
            UpdateSystemCaptionButtonColors();
        }

        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            if (!_mainWindow.DispatcherQueue.TryEnqueue(UpdateSystemCaptionButtonColors))
            {
                Trace.TraceWarning("The Gallery dispatcher rejected a theme update during shutdown.");
            }
        }

        public static bool IsDarkTheme() => ActualTheme == ElementTheme.Dark;

        public static void UpdateSystemCaptionButtonColors()
        {
            foreach (var window in WindowHelper.Windows)
            {
                var theme = window.Content is FrameworkElement root ? root.ActualTheme : ActualTheme;
                window.AppWindow.TitleBar.ButtonForegroundColor =
                    theme == ElementTheme.Dark ? Colors.White : Colors.Black;
            }
        }
    }
}
