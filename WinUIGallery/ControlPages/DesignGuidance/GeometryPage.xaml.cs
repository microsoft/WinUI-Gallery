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

namespace WinUIGallery.ControlPages;

public sealed partial class GeometryPage : Page
{
    public GeometryPage()
    {
        this.InitializeComponent();
    }

    private void ShowGeometryButtonClick1(object sender, RoutedEventArgs e)
    {
        ShowGeometryInfoTooltip1.IsOpen = !ShowGeometryInfoTooltip1.IsOpen;
    }

    private void ShowGeometryButtonClick2(object sender, RoutedEventArgs e)
    {
        ShowGeometryInfoTooltip2.IsOpen = !ShowGeometryInfoTooltip2.IsOpen;
    }

    private void ShowGeometryButtonClick3(object sender, RoutedEventArgs e)
    {
        ShowGeometryInfoTooltip3.IsOpen = !ShowGeometryInfoTooltip3.IsOpen;
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
