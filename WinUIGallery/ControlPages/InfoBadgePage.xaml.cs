using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.DesktopWap.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class InfoBadgePage : Page
    {
        public InfoBadgePage()
        {
            this.InitializeComponent();
        }
        public double InfoBadgeOpacity
        {
            get { return (double)GetValue(InfoBadgeOpacityProperty); }
            set { SetValue(InfoBadgeOpacityProperty, value); }
        }

        public static readonly DependencyProperty InfoBadgeOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(PageHeader), new PropertyMetadata(0.0));

        public void NavigationViewDisplayMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string paneDisplayMode = e.AddedItems[0].ToString();

            switch (paneDisplayMode)
            {
                case "LeftExpanded":
                    nvSample1.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                    nvSample1.IsPaneOpen = true;
                    break;

                case "LeftCompact":
                    nvSample1.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact;
                    nvSample1.IsPaneOpen = false;
                    break;

                case "Top":
                    nvSample1.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
                    nvSample1.IsPaneOpen = true;
                    break;
            }
        }

        private void ToggleInfoBadgeOpacity_Toggled(object sender, RoutedEventArgs e)
        {
            InfoBadgeOpacity = (InfoBadgeOpacity == 0.0) ? 1.0 : 0.0;
        }

        public void InfoBadgeStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string infoBadgeStyle = e.AddedItems[0].ToString();

            switch (infoBadgeStyle)
            {
                case "Attention":
                    infoBadge2.Style = Application.Current.Resources["AttentionIconInfoBadgeStyle"] as Style;
                    infoBadge3.Style = Application.Current.Resources["AttentionValueInfoBadgeStyle"] as Style;
                    infoBadge4.Style = Application.Current.Resources["AttentionDotInfoBadgeStyle"] as Style;
                    break;

                case "Informational":
                    infoBadge2.Style = Application.Current.Resources["InformationalIconInfoBadgeStyle"] as Style;
                    infoBadge3.Style = Application.Current.Resources["InformationalValueInfoBadgeStyle"] as Style;
                    infoBadge4.Style = Application.Current.Resources["InformationalDotInfoBadgeStyle"] as Style;
                    break;

                case "Success":
                    infoBadge2.Style = Application.Current.Resources["SuccessIconInfoBadgeStyle"] as Style;
                    infoBadge3.Style = Application.Current.Resources["SuccessValueInfoBadgeStyle"] as Style;
                    infoBadge4.Style = Application.Current.Resources["SuccessDotInfoBadgeStyle"] as Style;
                    break;

                case "Critical":
                    infoBadge2.Style = Application.Current.Resources["CriticalIconInfoBadgeStyle"] as Style;
                    infoBadge3.Style = Application.Current.Resources["CriticalValueInfoBadgeStyle"] as Style;
                    infoBadge4.Style = Application.Current.Resources["CriticalDotInfoBadgeStyle"] as Style;
                    break;
            }
        }

        private void ValueNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            if ((int)args.NewValue >= -1)
            {
                DynamicInfoBadge.Value = (int)args.NewValue;
            }
        }
    }
}
