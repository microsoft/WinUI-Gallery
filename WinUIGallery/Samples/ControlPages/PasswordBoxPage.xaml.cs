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

namespace WinUIGallery.ControlPages;

public sealed partial class PasswordBoxPage : Page
{
    public PasswordBoxPage()
    {
        InitializeComponent();
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

    private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e) => passworBoxWithRevealmode.PasswordRevealMode = revealModeCheckBox.IsChecked == true ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden;
}
