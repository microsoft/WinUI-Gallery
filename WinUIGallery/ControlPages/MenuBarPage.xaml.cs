using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class MenuBarPage : Page
    {
        public MenuBarPage()
        {
            this.InitializeComponent();
        }

        private void OnElementClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
    }
}
