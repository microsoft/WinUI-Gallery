// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class WrapPanelPage : Page
{
    public WrapPanelPage()
    {
        InitializeComponent();
    }

    private void OrientationGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as RadioButtons)?.SelectedItem is not RadioButton selectedItem ||
            Enum.TryParse<Orientation>(selectedItem.Tag?.ToString(), out Orientation orientation) is false ||
            Control1 is null)
        {
            return;
        }

        Control1.Orientation = orientation;
        LayoutHost.Height = orientation == Orientation.Vertical ? 360 : 200;
    }

    private void ItemSpacingSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (Control1 is not null)
        {
            Control1.ItemSpacing = e.NewValue;
        }
    }

    private void LineSpacingSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (Control1 is not null)
        {
            Control1.LineSpacing = e.NewValue;
        }
    }

    private void PaddingSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (Control1 is not null)
        {
            Control1.Padding = new Thickness(e.NewValue);
        }
    }

    private void StretchLastItemToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch && Control2 is not null)
        {
            Control2.ItemsStretch = toggleSwitch.IsOn ? WrapPanelItemsStretch.Last : WrapPanelItemsStretch.None;
        }
    }
}
