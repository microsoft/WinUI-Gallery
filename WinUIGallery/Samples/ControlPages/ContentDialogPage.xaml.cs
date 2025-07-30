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

    private void SetDialogResultText(TextBlock targetTextBlock, string text)
    {
        targetTextBlock.Text = text;
        var peer = FrameworkElementAutomationPeer.FromElement(targetTextBlock) ?? FrameworkElementAutomationPeer.CreatePeerForElement(targetTextBlock);
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
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
            SetDialogResultText(DialogResult, "User saved their work");
        }
        else if (result == ContentDialogResult.Secondary)
        {
            SetDialogResultText(DialogResult, "User did not save their work");
        }
        else
        {
            SetDialogResultText(DialogResult, "User cancelled the dialog");
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
            SetDialogResultText(DialogResultNoDefault, "User deleted the file");
        }
        else if (result == ContentDialogResult.Secondary)
        {
            SetDialogResultText(DialogResultNoDefault, "User kept the file");
        }
        else
        {
            SetDialogResultText(DialogResultNoDefault, "User cancelled the dialog");
        }
    }
}
