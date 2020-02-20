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

namespace AppUIBasics.ControlPages
{
    public sealed partial class PasswordBoxPage : Page
    {
        public PasswordBoxPage()
        {
            this.InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;

            if (pb != null)
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
    }
}
