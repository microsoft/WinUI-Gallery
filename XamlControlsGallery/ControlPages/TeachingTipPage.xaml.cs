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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TeachingTipPage : Page
    {
        public TeachingTipPage()
        {
            this.InitializeComponent();
        }

        private void SaveButtonClick1(object sender, RoutedEventArgs e)
        {
            TeachingTip.SetAttach(null, AutoSaveTip1);
            AutoSaveTip1.IsOpen = true;
        }

        private void SaveButtonClick2(object sender, RoutedEventArgs e)
        {
            AutoSaveTip2.IsOpen = true;
        }

        private void SaveButtonClick3(object sender, RoutedEventArgs e)
        {
            AutoSaveTip3.IsOpen = true;
        }

    }
}