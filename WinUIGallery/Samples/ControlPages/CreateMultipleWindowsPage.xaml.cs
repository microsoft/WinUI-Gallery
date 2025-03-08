//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Helpers;
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
