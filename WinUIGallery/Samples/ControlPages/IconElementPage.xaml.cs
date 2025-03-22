using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace WinUIGallery.ControlPages;

public sealed partial class IconElementPage : Page
{
    public IconElementPage()
    {
        InitializeComponent();
    }

    private void MonochromeButton_CheckedChanged(object sender, RoutedEventArgs e)
    {
        SlicesIcon.ShowAsMonochrome = (bool)MonochromeButton.IsChecked;
        SlicesIcon.UriSource = new Uri("ms-appx:///Assets/slices.png");
    }
}
