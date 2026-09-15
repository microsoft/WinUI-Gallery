// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinUIGallery.Layouts;

namespace WinUIGallery.ControlPages;

public sealed partial class LayoutPanelPage : Page
{
    private readonly StackLayout _stackLayout = new() { Orientation = Orientation.Vertical, Spacing = 8 };
    private FeaturedTileLayout? _featuredTileLayout;

    public LayoutPanelPage()
    {
        InitializeComponent();
        _featuredTileLayout = (FeaturedTileLayout)LayoutPanel1.Layout;
    }

    private void LayoutButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as RadioButtons)?.SelectedItem is not RadioButton selectedItem ||
            LayoutPanel1 is null ||
            _featuredTileLayout is null)
        {
            return;
        }

        LayoutPanel1.Layout = selectedItem.Tag?.ToString() == "Stack"
            ? _stackLayout
            : _featuredTileLayout;
    }

    private void SpacingSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        _stackLayout.Spacing = e.NewValue;

        if (_featuredTileLayout is not null)
        {
            _featuredTileLayout.Spacing = e.NewValue;
        }
    }
}
