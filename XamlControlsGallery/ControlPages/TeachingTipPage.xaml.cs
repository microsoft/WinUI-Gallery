//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace AppUIBasics.ControlPages
{
    public sealed partial class TeachingTipPage : Page
    {
        public TeachingTipPage()
        {
            this.InitializeComponent();
        }

        private void TestButtonClick1(object sender, RoutedEventArgs e)
        {
            if (NavigationRootPage.GetForElement(this)?.PageHeader != null)
            {
                NavigationRootPage.GetForElement(this).PageHeader.TeachingTip1.IsOpen = true;
            }
        }

        private void TestButtonClick2(object sender, RoutedEventArgs e)
        {
            if (NavigationRootPage.GetForElement(this)?.PageHeader != null)
            {
                NavigationRootPage.GetForElement(this).PageHeader.TeachingTip2.IsOpen = true;
            }
        }

        private void TestButtonClick3(object sender, RoutedEventArgs e)
        {
            if (NavigationRootPage.GetForElement(this)?.PageHeader != null)
            {
                NavigationRootPage.GetForElement(this).PageHeader.TeachingTip3.IsOpen = true;
            }
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            // The non-light dismiss Teaching tips do not handle the escape key, however we do not want the page to navigate away while they are open, so we will mark these key events as

            // handled while these tips are open.
            if (e.Key == Windows.System.VirtualKey.Escape && (NavigationRootPage.GetForElement(this).PageHeader.TeachingTip3.IsOpen || NavigationRootPage.GetForElement(this).PageHeader.TeachingTip1.IsOpen))
            {
                e.Handled = true;
            }
        }
    }
}
