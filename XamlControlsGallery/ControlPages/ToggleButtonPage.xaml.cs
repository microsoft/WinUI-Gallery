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
    public sealed partial class ToggleButtonPage : Page
    {
        public ToggleButtonPage()
        {
            this.InitializeComponent();

            // Set initial output value.
            Control1Output.Text = (bool)Toggle1.IsChecked ? "On" : "Off";
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Control1Output.Text = "On";
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Control1Output.Text = "Off";
        }
    }
}
