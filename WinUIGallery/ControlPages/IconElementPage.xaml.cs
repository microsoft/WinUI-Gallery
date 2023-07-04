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
    public sealed partial class IconElementPage : Page
    {
        public IconElementPage()
        {
            this.InitializeComponent();
        }

        private void MonochromeButton_Checked(object sender, RoutedEventArgs e)
        {
            SlicesIcon.ShowAsMonochrome = true;
            SlicesIcon.UriSource = new Uri("ms-appx:///Assets/slices.png");
        }

        private void MonochromeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            SlicesIcon.ShowAsMonochrome = false;
            SlicesIcon.UriSource = new Uri("ms-appx:///Assets/slices.png");
        }
    }
}
