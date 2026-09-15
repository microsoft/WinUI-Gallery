// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinUIGallery.ControlPages;

public sealed partial class FlowLayoutPage : Page
{
    private readonly HashSet<UIElement> _realizedElements = [];
    private bool _realizedCountUpdateQueued;

    public IReadOnlyList<FlowLayoutItem> Items { get; } = Enumerable.Range(1, 500)
        .Select(index => new FlowLayoutItem($"Item {index}"))
        .ToList();

    public FlowLayoutPage()
    {
        InitializeComponent();
    }

    private void OrientationButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as RadioButtons)?.SelectedItem is RadioButton selectedItem &&
            Enum.TryParse(selectedItem.Tag?.ToString(), out Orientation orientation) &&
            FlowLayout1 is not null)
        {
            FlowLayout1.Orientation = orientation;

            if (FlowScrollViewer is not null)
            {
                bool isHorizontal = orientation == Orientation.Horizontal;
                FlowScrollViewer.HorizontalScrollMode = isHorizontal ? ScrollMode.Disabled : ScrollMode.Enabled;
                FlowScrollViewer.HorizontalScrollBarVisibility = isHorizontal
                    ? ScrollBarVisibility.Disabled
                    : ScrollBarVisibility.Auto;
                FlowScrollViewer.VerticalScrollMode = isHorizontal ? ScrollMode.Enabled : ScrollMode.Disabled;
                FlowScrollViewer.VerticalScrollBarVisibility = isHorizontal
                    ? ScrollBarVisibility.Auto
                    : ScrollBarVisibility.Disabled;
            }
        }
    }

    private void LineAlignmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as ComboBox)?.SelectedItem is ComboBoxItem selectedItem &&
            Enum.TryParse(selectedItem.Tag?.ToString(), out FlowLayoutLineAlignment alignment) &&
            FlowLayout1 is not null)
        {
            FlowLayout1.LineAlignment = alignment;
        }
    }

    private void ItemSpacingSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (FlowLayout1 is not null)
        {
            FlowLayout1.MinItemSpacing = e.NewValue;
        }
    }

    private void LineSpacingSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (FlowLayout1 is not null)
        {
            FlowLayout1.LineSpacing = e.NewValue;
        }
    }

    private void FlowRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        _realizedElements.Add(args.Element);
        QueueRealizedCountUpdate();
    }

    private void FlowRepeater_ElementClearing(ItemsRepeater sender, ItemsRepeaterElementClearingEventArgs args)
    {
        _realizedElements.Remove(args.Element);
        QueueRealizedCountUpdate();
    }

    private void QueueRealizedCountUpdate()
    {
        if (_realizedCountUpdateQueued)
        {
            return;
        }

        _realizedCountUpdateQueued = DispatcherQueue.TryEnqueue(() =>
        {
            _realizedCountUpdateQueued = false;
            RealizedCountText.Text = $"Realized elements: {_realizedElements.Count} of {Items.Count}";
        });
    }
}

public sealed record FlowLayoutItem(string Label);
