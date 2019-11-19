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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuBarPage : Page
    {
        public MenuBarPage()
        {
            this.InitializeComponent();
        }

        private void OnElementClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ControlExample example = (sender as MenuFlyoutItem).Parent as ControlExample;
            var selectedFlyoutItem = sender as MenuFlyoutItem;
            string exampleNumber = selectedFlyoutItem.Name.Substring(0, 1);
            if(exampleNumber == "1")
            {
                SelectedOptionText.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
            }
            else if(exampleNumber == "2")
            {
                SelectedOptionText.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
            }
            else if (exampleNumber == "3")
            {
                SelectedOptionText.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
            }
        }

        //private void OnElementClicked1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    example1.onClickedText = "You clicked: " + (sender as MenuFlyoutItem).Text;
        //}
        //private void OnElementClicked2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    example2.onClickedText = "You clicked: " + (sender as MenuFlyoutItem).Text;
        //}
    }
}
