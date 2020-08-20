//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using Windows.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class CornerRadiusPage : Page
    {
        public CornerRadiusPage()
        {
            this.InitializeComponent();
        }

        private async void ShowDialog_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog CornerRadiusDialog = new ContentDialog
            {
                Title = "I'm a dialog!",
                Content = "CornerRadius: 10",
                CloseButtonText = "OK",
                CornerRadius = new Windows.UI.Xaml.CornerRadius(10)
            };
            CornerRadiusDialog.DefaultButton = ContentDialogButton.Close;
            await CornerRadiusDialog.ShowAsync();
        }
    }
}
