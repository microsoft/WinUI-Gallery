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
            ToggleThemeTeachingTip1.IsOpen = true;
        }

        private void TestButtonClick2(object sender, RoutedEventArgs e)
        {
            ToggleThemeTeachingTip2.IsOpen = true;
        }

        private void TestButtonClick3(object sender, RoutedEventArgs e)
        {
            ToggleThemeTeachingTip3.IsOpen = true;
        }
    }
}
