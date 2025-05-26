//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Helpers;

public static class OuterScrollingHelper
{
    public static void SuspendOuterScrolling(this Page page)
    {
        page.Loaded += OnPageLoaded;
    }

    private static void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        var page = sender as Page;
        if (page != null)
        {
            page.Loaded -= OnPageLoaded;
            page.Unloaded += OnPageUnloaded;
            OverrideUpperScrollViewer(page, isSuspended: true);
        }
    }

    private static void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        var page = sender as Page;
        if (page != null)
        {
            OverrideUpperScrollViewer(page, isSuspended: false);
            page.Unloaded -= OnPageUnloaded;
        }
    }

    private static ScrollViewer scrollViewer;
    private static ScrollBarVisibility? oldOuterScrollingValue;
    private static void OverrideUpperScrollViewer(Page page, bool isSuspended)
    {
        scrollViewer ??= page.FindAscendant<ScrollViewer>();

        if (scrollViewer != null)
        {
            if (isSuspended)
            {
                oldOuterScrollingValue ??= scrollViewer.VerticalScrollBarVisibility;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            else
            {
                if (oldOuterScrollingValue.HasValue)
                {
                    scrollViewer.VerticalScrollBarVisibility = oldOuterScrollingValue.Value;
                    oldOuterScrollingValue = null;
                }

                scrollViewer = null;
            }
        }
    }
}
