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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

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

            (PageHeader.TeachingTip1 as TeachingTip).IsOpen = true;

        }

        private void TestButtonClick2(object sender, RoutedEventArgs e)
        {
            (PageHeader.TeachingTip2 as TeachingTip).IsOpen = true;
        }

        private void TestButtonClick3(object sender, RoutedEventArgs e)
        {
            (PageHeader.TeachingTip3 as TeachingTip).IsOpen = true;
        }
    }
}