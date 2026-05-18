// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class StackPanelPage : Page
{
    public StackPanelPage()
    {
        this.InitializeComponent();
    }

    private void OrientationGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as RadioButtons)?.SelectedItem is not RadioButton selectedItem ||
            Enum.TryParse<Orientation>(selectedItem.Tag?.ToString(), out var orientation) is false ||
            Control1 is null)
        {
            return;
        }

        Control1.Orientation = orientation;
    }
}
