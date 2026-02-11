// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Helpers;
using WinUIGallery.Pages;
using WinUIGallery.Samples.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class TitleBarPage : Page
{
    public TitleBarPage()
    {
        this.InitializeComponent();
    }

    private void CreateTitleBarWindowClick(object sender, RoutedEventArgs e)
    {
        TitleBarWindow titleBarWindow = new TitleBarWindow();
        titleBarWindow.Activate();
    }

    private void TitleBar_LayoutUpdated(object sender, object e)
    {
        TitleBarHelper.ApplySystemThemeToCaptionButtons(App.MainWindow, this.ActualTheme);
    }

    private void Hyperlink_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
    {
        App.MainWindow.Navigate(typeof(ItemPage), "AppWindowTitleBar");
    }
}