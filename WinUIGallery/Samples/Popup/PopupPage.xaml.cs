// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class PopupPage : Page
{
    public PopupPage()
    {
        this.InitializeComponent();
    }

    // Handles the Click event on the Button on the page and opens the Popup. 
    private void ShowPopupOffsetClicked(object sender, RoutedEventArgs e)
    {
        // open the Popup if it isn't open already 
        if (!StandardPopup.IsOpen) { StandardPopup.IsOpen = true; }
        IsLightDismissEnabledToggleSwitch.IsEnabled = false;
    }

    // Handles the Click event on the Button inside the Popup control and 
    // closes the Popup. 
    private void ClosePopupClicked(object sender, RoutedEventArgs e)
    {
        // if the Popup is open, then close it 
        if (StandardPopup.IsOpen) { StandardPopup.IsOpen = false; }
        IsLightDismissEnabledToggleSwitch.IsEnabled = true;
    }

    private void PopupClosed(object sender, object e)
    {
        IsLightDismissEnabledToggleSwitch.IsEnabled = true;
    }
}
