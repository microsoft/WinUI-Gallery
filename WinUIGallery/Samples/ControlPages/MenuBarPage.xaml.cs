// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class MenuBarPage : Page
{
    public MenuBarPage()
    {
        this.InitializeComponent();
    }

    private void OnElementClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is not MenuFlyoutItem selectedFlyoutItem)
        {
            return;
        }

        string exampleNumber = selectedFlyoutItem.Name.Substring(0, 1);

        if (exampleNumber == "o")
        {
            SelectedOptionText.Text = "You clicked: " + selectedFlyoutItem.Text;
        }
        else if (exampleNumber == "t")
        {
            SelectedOptionText1.Text = "You clicked: " + selectedFlyoutItem.Text;
        }
        else if (exampleNumber == "z")
        {
            SelectedOptionText2.Text = "You clicked: " + selectedFlyoutItem.Text;
        }
    }
}
