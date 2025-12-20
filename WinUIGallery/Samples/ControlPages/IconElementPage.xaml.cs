// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;


namespace WinUIGallery.ControlPages;

public sealed partial class IconElementPage : Page
{
    public IconElementPage()
    {
        this.InitializeComponent();
    }

    private void MonochromeButton_CheckedChanged(object sender, RoutedEventArgs e)
    {
        SlicesIcon.ShowAsMonochrome = MonochromeButton.IsChecked is true;
        SlicesIcon.UriSource = new Uri("ms-appx:///Assets/SampleMedia/Slices.png");
    }
}
