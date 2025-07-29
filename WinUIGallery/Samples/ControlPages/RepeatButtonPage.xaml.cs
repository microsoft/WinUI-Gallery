// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class RepeatButtonPage : Page
{
    public RepeatButtonPage()
    {
        this.InitializeComponent();
    }

    private static int _clicks = 0;
    private void RepeatButton_Click(object sender, RoutedEventArgs e)
    {
        _clicks += 1;
        Control1Output.Text = "Number of clicks: " + _clicks;

        AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(Control1Output) ?? FrameworkElementAutomationPeer.CreatePeerForElement(Control1Output);
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}
