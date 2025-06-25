// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation.Metadata;

namespace WinUIGallery.ControlPages;

public sealed partial class CommandBarFlyoutPage : Page
{
    public CommandBarFlyoutPage()
    {
        this.InitializeComponent();
    }

    private void OnElementClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Do custom logic
        SelectedOptionText.Text = "You clicked: " + (sender as AppBarButton).Label;
    }

    private void ShowMenu(bool isTransient)
    {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            FlyoutShowOptions myOption = new FlyoutShowOptions
            {
                ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard,
                Placement = FlyoutPlacementMode.RightEdgeAlignedTop
            };
            CommandBarFlyout1.ShowAt(Image1, myOption);
        }
        else
        {
            CommandBarFlyout1.ShowAt(Image1);
        }
    }

    private void MyImageButton_ContextRequested(Microsoft.UI.Xaml.UIElement sender, ContextRequestedEventArgs args)
    {
        // Show a context menu in standard mode
        // Focus will move to the menu
        ShowMenu(false);
    }

    private void MyImageButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Show a context menu in transient mode
        // Focus will not move to the menu
        ShowMenu(true);
    }
}
