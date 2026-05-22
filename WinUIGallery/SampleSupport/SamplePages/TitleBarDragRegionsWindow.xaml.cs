// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class TitleBarDragRegionsWindow : Window
{
    private Button? _extraButton;

    public TitleBarDragRegionsWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        SetTitleBar(titleBar);
        AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
    }

    private void BadgeIsDragRegionRadios_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        switch (BadgeIsDragRegionRadios.SelectedIndex)
        {
            case 0:
                StatusBadge.ClearValue(TitleBar.IsDragRegionProperty);
                break;
            case 1:
                TitleBar.SetIsDragRegion(StatusBadge, true);
                break;
            case 2:
                TitleBar.SetIsDragRegion(StatusBadge, false);
                break;
        }
    }

    private void StatusBadge_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        // Only fires when TitleBar.IsDragRegion is False; otherwise the
        // framework consumes the pointer for window dragging.
        StatusText.Text = "Status badge clicked";
    }

    private void ToggleExtraButton_Click(object sender, RoutedEventArgs e)
    {
        if (titleBar.Content is not Grid contentGrid)
        {
            return;
        }

        if (_extraButton is null)
        {
            _extraButton = new Button
            {
                Content = "Extra",
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetColumn(_extraButton, 1);
            contentGrid.Children.Insert(0, _extraButton);
            StatusText.Text = "Added a Button to TitleBar.Content. Call RecomputeDragRegions() to refresh drag regions.";
        }
        else
        {
            contentGrid.Children.Remove(_extraButton);
            _extraButton = null;
            StatusText.Text = "Removed the Button. Call RecomputeDragRegions() to refresh drag regions.";
        }
    }

    private void RecomputeDragRegions_Click(object sender, RoutedEventArgs e)
    {
        titleBar.RecomputeDragRegions();
        StatusText.Text = "RecomputeDragRegions() called.";
    }
}
