// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class ContentDialogPage : Page
{
    public ContentDialogPage()
    {
        this.InitializeComponent();
    }

    private async void ShowDialog_Click(object sender, RoutedEventArgs e)
    {
        ContentDialogExample dialog = new ContentDialogExample();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Title = "Save your work?";
        dialog.PrimaryButtonText = "Save";
        dialog.SecondaryButtonText = "Don't Save";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = new ContentDialogContent();

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            DialogResult.Text = "User saved their work";
            var peer = FrameworkElementAutomationPeer.FromElement(DialogResult) ?? FrameworkElementAutomationPeer.CreatePeerForElement(DialogResult);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
        else if (result == ContentDialogResult.Secondary)
        {
            DialogResult.Text = "User did not save their work";
            var peer = FrameworkElementAutomationPeer.FromElement(DialogResult) ?? FrameworkElementAutomationPeer.CreatePeerForElement(DialogResult);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
        else
        {
            DialogResult.Text = "User cancelled the dialog";
            var peer = FrameworkElementAutomationPeer.FromElement(DialogResult) ?? FrameworkElementAutomationPeer.CreatePeerForElement(DialogResult);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }

    private async void ShowDialogNoDefault_Click(object sender, RoutedEventArgs e)
    {
        ContentDialogExample dialog = new ContentDialogExample();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Title = "Delete file?";
        dialog.PrimaryButtonText = "Delete";
        dialog.SecondaryButtonText = "Keep";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.None;
        dialog.Content = new ContentDialogContent();

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            DialogResultNoDefault.Text = "User deleted the file";
            var peer = FrameworkElementAutomationPeer.FromElement(DialogResultNoDefault) ?? FrameworkElementAutomationPeer.CreatePeerForElement(DialogResultNoDefault);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
        else if (result == ContentDialogResult.Secondary)
        {
            DialogResultNoDefault.Text = "User kept the file";
            var peer = FrameworkElementAutomationPeer.FromElement(DialogResultNoDefault) ?? FrameworkElementAutomationPeer.CreatePeerForElement(DialogResultNoDefault);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
        else
        {
            DialogResultNoDefault.Text = "User cancelled the dialog";
            var peer = FrameworkElementAutomationPeer.FromElement(DialogResultNoDefault) ?? FrameworkElementAutomationPeer.CreatePeerForElement(DialogResultNoDefault);
            peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }
}
