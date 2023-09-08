using AppUIBasics.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class MenuBarPage : Page
    {
        public MenuBarPage()
        {
            this.InitializeComponent();
        }

        private void OnElementClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var selectedFlyoutItem = sender as MenuFlyoutItem;
            string exampleNumber = selectedFlyoutItem.Name.Substring(0, 1);
            if(exampleNumber == "o")
            {
                SelectedOptionText.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
            }
            else if(exampleNumber == "t")
            {
                SelectedOptionText1.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
            }
            else if(exampleNumber == "z")
            {
                SelectedOptionText2.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
            }
        }

        // Workaround for known issue with menu themes in WinAppSDK 1.4 (#8678, #8756)
        private void MenuBar_LayoutUpdated(object sender, object e)
        {
            foreach (var popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(this.XamlRoot))
            {
                popup.RequestedTheme = ThemeHelper.RootTheme;
            }
        }
    }
}
