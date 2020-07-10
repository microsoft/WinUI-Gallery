//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class RadioButtonPage : Page
    {
        public RadioButtonPage()
        {
            this.InitializeComponent();
        }

        private void Option1RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Control1Output.Text = "You selected option 1.";
        }

        private void Option2RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Control1Output.Text = "You selected option 2.";
        }

        private void Option3RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Control1Output.Text = "You selected option 3.";
        }
    }
}
