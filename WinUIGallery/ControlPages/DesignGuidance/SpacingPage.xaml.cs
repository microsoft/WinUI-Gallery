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
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace WinUIGallery.ControlPages
{
    public sealed partial class SpacingPage : Page
    {
        public SpacingPage()
        {
            this.InitializeComponent();
        }

        private void CopyControlResourceToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new DataPackage();
            package.SetText("ControlCornerRadius");
            Clipboard.SetContent(package);
        }

        private void CopyOverlayResourceToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new DataPackage();
            package.SetText("OverlayCornerRadius");
            Clipboard.SetContent(package);
        }
    }
}
