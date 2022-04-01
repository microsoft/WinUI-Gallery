//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Data;
using AppUIBasics.Helper;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class CreateMultipleWindowsPage : Page
    {
        public CreateMultipleWindowsPage()
        {
            this.InitializeComponent();
        }

        private void List_GotFocus(object sender, RoutedEventArgs e)
        {
            Control1.StartBringIntoView();
        }

        private void createNewWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = WindowHelper.CreateWindow();
            var rootPage = new NavigationRootPage();
            rootPage.RequestedTheme = ThemeHelper.RootTheme;
            newWindow.Content = rootPage;
            newWindow.Activate();

            var targetPageType = typeof(NewControlsPage);
            string targetPageArguments = string.Empty;
            rootPage.Navigate(targetPageType, targetPageArguments);
        }
    }
}
