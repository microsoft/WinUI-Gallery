using WinUIGallery.Helpers;
using Microsoft.UI.Xaml;
using WinRT;// required to support Window.As<ICompositionSupportsSystemBackdrop>()
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Composition;
using System;
using Windows.UI.ViewManagement;
using Microsoft.UI.System;

namespace WinUIGallery.SamplePages;

public sealed partial class SampleSystemBackdropsWindow : Window
{
    public BackdropType[] AllowedBackdrops
    {
        get => (BackdropType[])backdropComboBox.ItemsSource;
        set => backdropComboBox.ItemsSource = value;
    }

    WindowsSystemDispatcherQueueHelper wsdqHelper;
    BackdropType currentBackdrop;
    MicaController micaController;
    DesktopAcrylicController acrylicController;
    SystemBackdropConfiguration configurationSource;

    public SampleSystemBackdropsWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon(@"Assets\Tiles\GalleryIcon.ico");
        ExtendsContentIntoTitleBar = true;
        ((FrameworkElement)Content).RequestedTheme = ThemeHelper.RootTheme;
        wsdqHelper = new WindowsSystemDispatcherQueueHelper();
        wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

        backdropComboBox.SelectedIndex = 0;
        themeComboBox.SelectedIndex = 0;
    }

    public enum BackdropType
    {
        None,
        Mica,
        MicaAlt,
        Acrylic,
        AcrylicThin
    }

    public void SetBackdrop(BackdropType type)
    {
        // Reset to default color. If the requested type is supported, we'll update to that.
        // Note: This sample completely removes any previous controller to reset to the default
        //       state. This is done so this sample can show what is expected to be the most
        //       common pattern of an app simply choosing one controller type which it sets at
        //       startup. If an app wants to toggle between Mica and Acrylic it could simply
        //       call RemoveSystemBackdropTarget() on the old controller and then setup the new
        //       controller, reusing any existing m_configurationSource and Activated/Closed
        //       event handlers.

        //Reset the backdrop
        currentBackdrop = BackdropType.None;
        tbChangeStatus.Text = "";

        micaController?.Dispose();
        micaController = null;
        acrylicController?.Dispose();
        acrylicController = null;
        configurationSource = null;

        //Set the backdrop
        if (type == BackdropType.Mica)
        {
            if (TrySetMicaBackdrop(false))
                currentBackdrop = type;
            else
            {
                // Mica isn't supported. Try Acrylic.
                type = BackdropType.Acrylic;
                tbChangeStatus.Text += "  Mica isn't supported. Trying Acrylic.";
            }
        }
        if (type == BackdropType.MicaAlt)
        {
            if (TrySetMicaBackdrop(true))
                currentBackdrop = type;
            else
            {
                // MicaAlt isn't supported. Try Acrylic.
                type = BackdropType.Acrylic;
                tbChangeStatus.Text += "  MicaAlt isn't supported. Trying Acrylic.";
            }
        }
        if (type == BackdropType.Acrylic)
        {
            if (TrySetAcrylicBackdrop(false))
                currentBackdrop = type;
            else
            {
                // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                tbChangeStatus.Text += "  Acrylic Base isn't supported. Switching to default color.";
            }
        }
        if (type == BackdropType.AcrylicThin)
        {
            if (TrySetAcrylicBackdrop(true))
                currentBackdrop = type;
            else
            {
                // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                tbChangeStatus.Text += "  Acrylic Thin isn't supported. Switching to default color.";
            }
        }

        //Fix the none backdrop
        SetNoneBackdropBackground();

        //Announce visual change to automation
        UIHelper.AnnounceActionForAccessibility(backdropComboBox, $"Background changed to {currentBackdrop}", "BackgroundChangedNotificationActivityId");
    }

    bool TrySetMicaBackdrop(bool useMicaAlt)
    {
        if (MicaController.IsSupported())
        {
            // Hooking up the policy object.
            configurationSource = new SystemBackdropConfiguration();
            configurationSource.IsHighContrast = ThemeSettings.CreateForWindowId(this.AppWindow.Id).HighContrast;
            Activated += Window_Activated;
            Closed += Window_Closed;
            ((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;

            // Initial configuration state.
            configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            micaController = new MicaController { Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base };

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
            micaController.SetSystemBackdropConfiguration(configurationSource);
            return true; // Succeeded.
        }

        return false; // Mica is not supported on this system.
    }

    bool TrySetAcrylicBackdrop(bool useAcrylicThin)
    {
        if (DesktopAcrylicController.IsSupported())
        {
            // Hooking up the policy object.
            configurationSource = new SystemBackdropConfiguration();
            Activated += Window_Activated;
            Closed += Window_Closed;
            ((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;

            // Initial configuration state.
            configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            acrylicController = new DesktopAcrylicController { Kind = useAcrylicThin ? DesktopAcrylicKind.Thin : DesktopAcrylicKind.Base };

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            acrylicController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
            acrylicController.SetSystemBackdropConfiguration(configurationSource);
            return true; // Succeeded.
        }

        return false; // Acrylic is not supported on this system
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        if(configurationSource != null)
            configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        micaController?.Dispose();
        micaController = null;
        acrylicController?.Dispose();
        acrylicController = null;
        configurationSource = null;

        Activated -= Window_Activated;
        Closed -= Window_Closed;
        ((FrameworkElement)Content).ActualThemeChanged -= Window_ThemeChanged;
    }

    private void Window_ThemeChanged(FrameworkElement sender, object args)
    {
        if (configurationSource != null)
            SetConfigurationSourceTheme();
    }

    private void SetConfigurationSourceTheme()
    {
        configurationSource.IsHighContrast = ThemeSettings.CreateForWindowId(this.AppWindow.Id).HighContrast;
        configurationSource.Theme = (SystemBackdropTheme)((FrameworkElement)Content).ActualTheme;
    }

    private void BackdropComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetBackdrop((BackdropType) backdropComboBox.SelectedItem);
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ((FrameworkElement)Content).RequestedTheme = Enum.GetValues<ElementTheme>()[themeComboBox.SelectedIndex];

        TitleBarHelper.SetCaptionButtonColors(this, ((FrameworkElement)Content).ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black);
        SetNoneBackdropBackground();
    }

    //Fixes the background color not changing when switching between themes.
    void SetNoneBackdropBackground()
    {
        if (currentBackdrop == BackdropType.None && themeComboBox.SelectedIndex != 0)
            ((Grid)Content).Background = new SolidColorBrush(themeComboBox.SelectedIndex == 1 ? Colors.White : Colors.Black);
        else
            ((Grid)Content).Background = new SolidColorBrush(Colors.Transparent);
    }
}
