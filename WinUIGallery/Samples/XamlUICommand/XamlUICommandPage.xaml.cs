// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class XamlUICommandPage : Page
{
    public XamlUICommandPage()
    {
        this.InitializeComponent();
    }

    private void CustomXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        XamlUICommandOutput.Text = "You fired the custom command";
        UIHelper.AnnounceActionForAccessibility(CustomButton, "Activated custom XAML UI Command", "CustomXamlUICommandNotificationActivityId");
    }
}
