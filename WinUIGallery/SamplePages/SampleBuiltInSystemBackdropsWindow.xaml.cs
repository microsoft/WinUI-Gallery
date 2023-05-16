using AppUIBasics.Helper;
using Microsoft.UI.Xaml;

namespace AppUIBasics.SamplePages
{
    public sealed partial class SampleBuiltInSystemBackdropsWindow : Window
    {
        BackdropType m_currentBackdrop;

        public SampleBuiltInSystemBackdropsWindow()
        {
            this.InitializeComponent();
            ((FrameworkElement)this.Content).RequestedTheme = AppUIBasics.Helper.ThemeHelper.RootTheme;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(titleBar);
            SetBackdrop(BackdropType.Mica);
        }


        public enum BackdropType
        {
            Mica,
            MicaAlt,
            DesktopAcrylic,
            DefaultColor,
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
            m_currentBackdrop = BackdropType.DefaultColor;
            tbCurrentBackdrop.Text = "None (default theme color)";
            tbChangeStatus.Text = "";
            this.SystemBackdrop = null;

            if (type == BackdropType.Mica)
            {
                if (TrySetMicaBackdrop(false))
                {
                    tbCurrentBackdrop.Text = "Built-in Mica";
                    m_currentBackdrop = type;
                }
                else
                {
                    // Mica isn't supported. Try Acrylic.
                    type = BackdropType.DesktopAcrylic;
                    tbChangeStatus.Text += "  Mica isn't supported. Trying Acrylic.";
                }
            }
            if (type == BackdropType.MicaAlt)
            {
                if (TrySetMicaBackdrop(true))
                {
                    tbCurrentBackdrop.Text = "Built-in MicaAlt";
                    m_currentBackdrop = type;
                }
                else
                {
                    // MicaAlt isn't supported. Try Acrylic.
                    type = BackdropType.DesktopAcrylic;
                    tbChangeStatus.Text += "  MicaAlt isn't supported. Trying Acrylic.";
                }
            }
            if (type == BackdropType.DesktopAcrylic)
            {
                if (TrySetAcrylicBackdrop())
                {
                    tbCurrentBackdrop.Text = "Built-in Acrylic";
                    m_currentBackdrop = type;
                }
                else
                {
                    // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                    tbChangeStatus.Text += "  Acrylic isn't supported. Switching to default color.";
                }
            }

            // Announce visual change to automation.
            UIHelper.AnnounceActionForAccessibility(btnChangeBackdrop, $"Background changed to {tbCurrentBackdrop.Text}", "BackgroundChangedNotificationActivityId");
        }

        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                Microsoft.UI.Xaml.Media.MicaBackdrop micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
                micaBackdrop.Kind = useMicaAlt ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt : Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
                this.SystemBackdrop = micaBackdrop;
                return true;
            }

            return false; // Mica is not supported on this system
        }

        bool TrySetAcrylicBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
            {
                this.SystemBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
                return true;
            }

            return false; // Acrylic is not supported on this system
        }

        void ChangeBackdropButton_Click(object sender, RoutedEventArgs e)
        {
            BackdropType newType;

            switch (m_currentBackdrop)
            {
                case BackdropType.Mica:           newType = BackdropType.MicaAlt; break;
                case BackdropType.MicaAlt:        newType = BackdropType.DesktopAcrylic; break;
                case BackdropType.DesktopAcrylic: newType = BackdropType.DefaultColor; break;
                default:
                case BackdropType.DefaultColor:   newType = BackdropType.Mica; break;
            }

            SetBackdrop(newType);
        }
    }
}
