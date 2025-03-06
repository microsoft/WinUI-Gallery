using Microsoft.UI.Xaml;
using Windows.Storage;

namespace WinUIGallery.Helpers;

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

            return WinUIGallery.App.GetEnum<ElementTheme>(App.Current.RequestedTheme.ToString());
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

            if (NativeHelper.IsAppPackaged)
            {
                ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey] = value.ToString();
            }
        }
    }

    public static void Initialize()
    {
        if (NativeHelper.IsAppPackaged)
        {
            string savedTheme = ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey]?.ToString();

            if (savedTheme != null)
            {
                RootTheme = WinUIGallery.App.GetEnum<ElementTheme>(savedTheme);
            }
        }
    }

    public static bool IsDarkTheme()
    {
        if (RootTheme == ElementTheme.Default)
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }
        return RootTheme == ElementTheme.Dark;
    }
}
