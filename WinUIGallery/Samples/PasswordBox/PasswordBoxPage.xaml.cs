// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class PasswordBoxPage : Page
{
    public PasswordBoxPage()
    {
        this.InitializeComponent();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox pb)
        {
            if (string.IsNullOrEmpty(pb.Password) || pb.Password == "Password")
            {
                Control1Output.Visibility = Visibility.Visible;
                Control1Output.Text = "'Password' is not allowed.";
                pb.Password = string.Empty;
            }
            else
            {
                Control1Output.Text = string.Empty;
                Control1Output.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        if (revealModeCheckBox.IsChecked == true)
        {
            passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Visible;
        }
        else
        {
            passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
    }
}
