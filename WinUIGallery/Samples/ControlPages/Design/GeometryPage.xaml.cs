// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
