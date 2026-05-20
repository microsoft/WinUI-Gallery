// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using WinUIGallery.Pages;

namespace WinUIGallery.Controls;

public sealed partial class BackgroundSection : Page
{
    public BackgroundSection()
    {
        this.InitializeComponent();
    }

    private void SystemBackdropLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        App.MainWindow.Navigate(typeof(ItemPage), "SystemBackdrops");
    }

    private void SystemBackdropElementLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        App.MainWindow.Navigate(typeof(ItemPage), "SystemBackdropElement");
    }
}
