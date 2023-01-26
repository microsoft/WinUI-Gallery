using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using AppUIBasics.SamplePages;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace AppUIBasics.ControlPages
{
    public sealed partial class TypographyPage : Page
    {
        public TypographyPage()
        {
            this.InitializeComponent();

        }

        private void TestButtonClick1(object sender, RoutedEventArgs e)
        {
            ShowTypographyInfoTooltip1.IsOpen = true;
        }

        private void TestButtonClick2(object sender, RoutedEventArgs e)
        {
            ShowTypographyInfoTooltip2.IsOpen = true;
        }

        private void TestButtonClick3(object sender, RoutedEventArgs e)
        {
            ShowTypographyInfoTooltip3.IsOpen = true;
        }
    }
}
