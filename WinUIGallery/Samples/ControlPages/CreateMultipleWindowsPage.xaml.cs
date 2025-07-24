// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Pages;

namespace WinUIGallery.ControlPages;

public sealed partial class CreateMultipleWindowsPage : Page
{
    public CreateMultipleWindowsPage()
    {
        this.InitializeComponent();
    }

    private void createNewWindow_Click(object sender, RoutedEventArgs e)
    {
        var newWindow = new MainWindow();
        newWindow.Activate();

        var targetPageType = typeof(HomePage);
        string targetPageArguments = string.Empty;
        newWindow.Navigate(targetPageType, targetPageArguments);
    }
}
