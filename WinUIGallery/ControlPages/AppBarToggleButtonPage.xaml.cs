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
    public sealed partial class AppBarToggleButtonPage : Page
    {
        public AppBarToggleButtonPage()
        {
            this.InitializeComponent();
        }

        private void CompactButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle && toggle.IsChecked != null)
            {
                Button1.IsCompact =
                Button2.IsCompact =
                Button3.IsCompact =
                Button4.IsCompact = (bool)toggle.IsChecked;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarToggleButton b)
            {
                string name = b.Name;

                switch (name)
                {
                    case "Button1":
                        Control1Output.Text = "IsChecked = " + b.IsChecked.ToString();
                        break;
                    case "Button2":
                        Control2Output.Text = "IsChecked = " + b.IsChecked.ToString();
                        break;
                    case "Button3":
                        Control3Output.Text = "IsChecked = " + b.IsChecked.ToString();
                        break;
                    case "Button4":
                        Control4Output.Text = "IsChecked = " + b.IsChecked.ToString();
                        break;
                }
            }
        }
    }
}
