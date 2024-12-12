using System;
using System.Runtime.InteropServices; // For DllImport
using WinUIGallery.Helper;
using Microsoft.UI.Xaml;
using WinRT;
using Microsoft.UI.Composition.SystemBackdrops;
using System.Linq;
using Microsoft.UI.Xaml.Media; // required to support Window.As<ICompositionSupportsSystemBackdrop>()

namespace WinUIGallery.SamplePages
{
    class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }
        
        [DllImport("CoreMessaging.dll")]
        private static unsafe extern int CreateDispatcherQueueController(DispatcherQueueOptions options, IntPtr* instance);

        IntPtr m_dispatcherQueueController = IntPtr.Zero;
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (m_dispatcherQueueController == IntPtr.Zero)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;    // DQTYPE_THREAD_CURRENT
                options.apartmentType = 2; // DQTAT_COM_STA

                unsafe
                {
                    IntPtr dispatcherQueueController;
                    CreateDispatcherQueueController(options, &dispatcherQueueController);
                    m_dispatcherQueueController = dispatcherQueueController;
                }
            }
        }
    }

    public sealed partial class SampleSystemBackdropsWindow : Window
    {
        public SampleSystemBackdropsWindow()
        {
            InitializeComponent();
            ((FrameworkElement)Content).RequestedTheme = ThemeHelper.RootTheme;
            wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            AppWindow.SetIcon(@"Assets\Tiles\GalleryIcon.ico");
            this.SetTitleBarTheme();
            SetBackdrop(BackdropType.Mica);
        }

        public enum BackdropType
        {
            None,
            Mica,
            MicaAlt,
            Acrylic,
            AcrylicThin
        }

        WindowsSystemDispatcherQueueHelper wsdqHelper;
        BackdropType currentBackdrop;
        public BackdropType[] AllowedBackdrops;
        MicaController micaController;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration configurationSource;

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
            currentBackdrop = BackdropType.None;
            tbCurrentBackdrop.Text = "None";
            tbChangeStatus.Text = "";
            if (micaController != null)
            {
                micaController.Dispose();
                micaController = null;
            }
            if (acrylicController != null)
            {
                acrylicController.Dispose();
                acrylicController = null;
            }
            Activated -= Window_Activated;
            Closed -= Window_Closed;
            ((FrameworkElement)Content).ActualThemeChanged -= Window_ThemeChanged;
            configurationSource = null;

            if (type == BackdropType.Mica)
            {
                if (TrySetMicaBackdrop(false))
                {
                    tbCurrentBackdrop.Text = "Custom Mica";
                    currentBackdrop = type;
                }
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
                {
                    tbCurrentBackdrop.Text = "Custom MicaAlt";
                    currentBackdrop = type;
                }
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
                {
                    tbCurrentBackdrop.Text = "Custom Acrylic (Base)";
                    currentBackdrop = type;
                }
                else
                {
                    // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                    tbChangeStatus.Text += "  Acrylic Base isn't supported. Switching to default color.";
                }
            }
            if (type == BackdropType.AcrylicThin)
            {
                if (TrySetAcrylicBackdrop(true))
                {
                    tbCurrentBackdrop.Text = "Custom Acrylic (Thin)";
                    currentBackdrop = type;
                }
                else
                {
                    // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                    tbChangeStatus.Text += "  Acrylic Thin isn't supported. Switching to default color.";
                }
            }

            SystemBackdrop backdrop = currentBackdrop switch
            {
                BackdropType.Mica => new MicaBackdrop(),
                BackdropType.MicaAlt => new MicaBackdrop() { Kind = MicaKind.BaseAlt },
                BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
                BackdropType.AcrylicThin => new DesktopAcrylicBackdrop(),
                _ => null
            }; 
            this.SetTitleBarBackdrop(backdrop);

            // announce visual change to automation
            UIHelper.AnnounceActionForAccessibility(btnChangeBackdrop, $"Background changed to {tbCurrentBackdrop.Text}", "BackgroundChangedNotificationActivityId");
        }

        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (MicaController.IsSupported())
            {
                // Hooking up the policy object.
                configurationSource = new SystemBackdropConfiguration();
                Activated += Window_Activated;
                Closed += Window_Closed;
                ((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                micaController = new MicaController();

                micaController.Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base;

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
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

                acrylicController = new DesktopAcrylicController();
                acrylicController.Kind = useAcrylicThin ? DesktopAcrylicKind.Thin : DesktopAcrylicKind.Base;

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                acrylicController.SetSystemBackdropConfiguration(configurationSource);
                return true; // Succeeded.
            }

            return false; // Acrylic is not supported on this system
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
            if (micaController != null)
            {
                micaController.Dispose();
                micaController = null;
            }
            if (acrylicController != null)
            {
                acrylicController.Dispose();
                acrylicController = null;
            }
            Activated -= Window_Activated;
            configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)Content).ActualTheme)
            {
                case ElementTheme.Dark:    configurationSource.Theme = SystemBackdropTheme.Dark; break;
                case ElementTheme.Light:   configurationSource.Theme = SystemBackdropTheme.Light; break;
                case ElementTheme.Default: configurationSource.Theme = SystemBackdropTheme.Default; break;
            }
        }

        void ChangeBackdropButton_Click(object sender, RoutedEventArgs e)
        {
            var newType = currentBackdrop switch
            {
                BackdropType.Mica => BackdropType.MicaAlt,
                BackdropType.MicaAlt => BackdropType.Acrylic,
                BackdropType.Acrylic => BackdropType.AcrylicThin,
                BackdropType.AcrylicThin => BackdropType.None,
                _ => BackdropType.Mica,
            };

            if (!AllowedBackdrops.Any(b => b == newType))
            {
                currentBackdrop = newType;
                ChangeBackdropButton_Click(sender, e);
                return;
            }

            SetBackdrop(newType);
        }
    }
}
