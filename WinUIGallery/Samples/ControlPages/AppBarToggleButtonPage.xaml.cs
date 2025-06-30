// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class AppBarToggleButtonPage : Page
{
    public AppBarToggleButtonPage()
    {
        this.InitializeComponent();
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
