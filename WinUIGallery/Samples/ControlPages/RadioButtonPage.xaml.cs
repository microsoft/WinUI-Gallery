// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class RadioButtonPage : Page
{
    public RadioButtonPage()
    {
        this.InitializeComponent();
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        Control1Output.Text = string.Format("You selected {0}", (sender as RadioButton).Content.ToString());
    }
}
