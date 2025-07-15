// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections;

namespace WinUIGallery.Helpers;

public static partial class TabViewHelper
{
    public static void PopulateTabViewContextMenu(MenuFlyout menuFlyout)
    {
        menuFlyout.Items.Clear();

        var item = (TabViewItem)menuFlyout.Target;
        if (item == null)
        {
            return;
        }
        ListView tabViewListView = null;
        TabView tabView = null;

        DependencyObject current = item;

        while (current != null)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(current);

            if (parent is ListView parentTabViewListView)
            {
                tabViewListView = parentTabViewListView;
            }
            else if (parent is TabView parentTabView)
            {
                tabView = parentTabView;
            }

            if (tabViewListView != null && tabView != null)
            {
                break;
            }

            current = parent;
        }

        if (tabViewListView == null || tabView == null)
        {
            return;
        }

        // First, if there are tabs to the left or to the right of the tab on which this context menu is opening,
        // then we'll include menu items to move this tab to the left or to the right.
        //
        // There are two possible cases for tab views: either they have explicitly set tab items, or they have a data item source set.
        // To move a tab left or right with explicitly set tab items, we'll remove and replace the tab item itself.
        // To move a tab left or right with a data item source set, we'll instead remove and replace the data item in the source list.
        int index = tabViewListView.IndexFromContainer(item);

        if (index > 0)
        {
            MenuFlyoutItem moveLeftItem = new() { Text = "Move tab left" };
            moveLeftItem.Click += (s, args) =>
            {
                if (tabView.TabItemsSource is IList itemsSourceList)
                {
                    var item = itemsSourceList[index];
                    itemsSourceList.RemoveAt(index);
                    itemsSourceList.Insert(index - 1, item);
                }
                else
                {
                    var item = tabView.TabItems[index];
                    tabView.TabItems.RemoveAt(index);
                    tabView.TabItems.Insert(index - 1, item);
                }
            };
            menuFlyout.Items.Add(moveLeftItem);
        }

        if (index < tabViewListView.Items.Count - 1)
        {
            MenuFlyoutItem moveRightItem = new() { Text = "Move tab right" };
            moveRightItem.Click += (s, args) =>
            {
                if (tabView.TabItemsSource is IList itemsSourceList)
                {
                    var item = itemsSourceList[index];
                    itemsSourceList.RemoveAt(index);
                    itemsSourceList.Insert(index + 1, item);
                }
                else
                {
                    var item = tabView.TabItems[index];
                    tabView.TabItems.RemoveAt(index);
                    tabView.TabItems.Insert(index + 1, item);
                }
            };
            menuFlyout.Items.Add(moveRightItem);
        }
    }
}
