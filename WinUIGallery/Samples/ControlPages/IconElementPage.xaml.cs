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


namespace WinUIGallery.ControlPages;

public sealed partial class IconElementPage : Page
{
    public IconElementPage()
    {
        this.InitializeComponent();
    }

    private void MonochromeButton_CheckedChanged(object sender, RoutedEventArgs e)
    {
        SlicesIcon.ShowAsMonochrome = (bool)MonochromeButton.IsChecked;
        SlicesIcon.UriSource = new Uri("ms-appx:///Assets/SampleMedia/Slices.png");
    }
}
