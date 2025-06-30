// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace WinUIGallery.ControlPages;

public sealed partial class ScrollViewerPage : Page
{
    public ScrollViewerPage()
    {
        this.InitializeComponent();
        ScrollViewerControl.ZoomToFactor(4.0f);
    }

    private void ZoomModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ScrollViewerControl != null && ZoomSlider != null)
        {
            if (sender is ComboBox cb)
            {
                ScrollViewerControl.ZoomMode = (ZoomMode)cb.SelectedIndex;
                ZoomSlider.IsEnabled = cb.SelectedIndex == 1;

                if (!ZoomSlider.IsEnabled)
                {
                    ScrollViewerControl.ZoomToFactor(2.0f);
                }
            }
        }
    }

    private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (ScrollViewerControl != null)
        {
            ScrollViewerControl.ChangeView(null, null, (float)e.NewValue);
        }
    }

    private void hsmCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ScrollViewerControl != null)
        {
            if (sender is ComboBox cb)
            {
                ScrollViewerControl.HorizontalScrollMode = (ScrollMode)cb.SelectedIndex;
            }
        }
    }

    private void hsbvCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ScrollViewerControl != null)
        {
            if (sender is ComboBox cb)
            {
                ScrollViewerControl.HorizontalScrollBarVisibility = (ScrollBarVisibility)cb.SelectedIndex;
            }
        }
    }

    private void vsmCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ScrollViewerControl != null)
        {
            if (sender is ComboBox cb)
            {
                ScrollViewerControl.VerticalScrollMode = (ScrollMode)cb.SelectedIndex;
            }
        }
    }

    private void vsbvCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ScrollViewerControl != null)
        {
            if (sender is ComboBox cb)
            {
                ScrollViewerControl.VerticalScrollBarVisibility = (ScrollBarVisibility)cb.SelectedIndex;
            }
        }
    }

    private void ScrollViewerControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        ZoomSlider.Value = ScrollViewerControl.ZoomFactor;
    }

    private void ScrollViewerControl_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (!e.IsIntermediate)
        {
            ZoomSlider.Value = ScrollViewerControl.ZoomFactor;
        }
    }
}
