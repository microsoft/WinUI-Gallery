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
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AppBarSeparatorPage : Page
    { 
        public AppBarSeparatorPage()
        {
            this.InitializeComponent();
        }
        private void CompactButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as AppBarToggleButton).IsChecked == true)
            {
                Control1.DefaultLabelPosition = CommandBarDefaultLabelPosition.Collapsed;
            }
            else
            {
                Control1.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
            }
        }
    }
}
