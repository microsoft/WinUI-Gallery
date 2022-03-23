﻿//*********************************************************
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateMultipleWindowsPage : Page
    {
        private IEnumerable<ControlInfoDataGroup> _groups;

        public CreateMultipleWindowsPage()
        {
            this.InitializeComponent();
        }
        public IEnumerable<ControlInfoDataGroup> Groups
        {
            get { return this._groups; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _groups = ControlInfoDataSource.Instance.Groups;
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
