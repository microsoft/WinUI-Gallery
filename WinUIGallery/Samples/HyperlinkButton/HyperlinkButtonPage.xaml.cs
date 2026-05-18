// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Pages;

namespace WinUIGallery.ControlPages;

public sealed partial class HyperlinkButtonPage : Page
{
    public HyperlinkButtonPage()
    {
        this.InitializeComponent();
    }

    private void GoToHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        App.MainWindow.Navigate(typeof(ItemPage), "ToggleButton");
    }
}