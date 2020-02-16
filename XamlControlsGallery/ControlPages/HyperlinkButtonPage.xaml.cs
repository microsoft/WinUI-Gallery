//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class HyperlinkButtonPage : Page
    {
        public HyperlinkButtonPage()
        {
            this.InitializeComponent();
        }

        private void GoToHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationRootPage.RootFrame.Navigate(typeof(ItemPage), "ToggleButton");
        }
    }
}
