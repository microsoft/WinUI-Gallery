// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Controls;

public sealed partial class TileGallery : UserControl
{
    public TileGallery()
    {
        InitializeComponent();
    }

    private void scroller_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
    {
        if (e.FinalView.HorizontalOffset < 1)
        {
            ScrollBackBtn.Visibility = Visibility.Collapsed;
        }
        else if (e.FinalView.HorizontalOffset > 1)
        {
            ScrollBackBtn.Visibility = Visibility.Visible;
        }

        if (e.FinalView.HorizontalOffset > scroller.ScrollableWidth - 1)
        {
            ScrollForwardBtn.Visibility = Visibility.Collapsed;
        }
        else if (e.FinalView.HorizontalOffset < scroller.ScrollableWidth - 1)
        {
            ScrollForwardBtn.Visibility = Visibility.Visible;
        }
    }

    private void ScrollBackBtn_Click(object sender, RoutedEventArgs e)
    {
        scroller.ChangeView(scroller.HorizontalOffset - scroller.ViewportWidth, null, null);
        // Manually focus to ScrollForwardBtn since this button disappears after scrolling to the end.          
        ScrollForwardBtn.Focus(FocusState.Programmatic);
    }

    private void ScrollForwardBtn_Click(object sender, RoutedEventArgs e)
    {
        scroller.ChangeView(scroller.HorizontalOffset + scroller.ViewportWidth, null, null);

        // Manually focus to ScrollBackBtn since this button disappears after scrolling to the end.    
        ScrollBackBtn.Focus(FocusState.Programmatic);
    }

    private void scroller_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateScrollButtonsVisibility();

    private void UpdateScrollButtonsVisibility() => ScrollForwardBtn.Visibility = scroller.ScrollableWidth > 0 ? Visibility.Visible : Visibility.Collapsed;
}
