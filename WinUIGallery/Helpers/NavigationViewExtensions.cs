// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace WinUIGallery.Helpers;

/// <summary>
/// Provides extension methods for <see cref="NavigationView"/> and <see cref="NavigationViewItem"/>
/// to retrieve all navigation view items, including nested items.
/// </summary>
public static class NavigationViewExtensions
{
    /// <summary>
    /// Retrieves all <see cref="NavigationViewItem"/> instances contained within the specified <see cref="NavigationView"/>,
    /// including all nested items.
    /// </summary>
    /// <param name="navigationView">The <see cref="NavigationView"/> to search within.</param>
    /// <returns>An enumerable collection of all <see cref="NavigationViewItem"/> instances in the navigation view.</returns>
    public static IEnumerable<NavigationViewItem> GetAllNavigationViewItems(this NavigationView navigationView)
    {
        if (navigationView.MenuItems is null)
        {
            yield break;
        }

        foreach (var menuItem in navigationView.MenuItems.OfType<NavigationViewItem>())
        {
            yield return menuItem;

            if (menuItem.MenuItems is null)
            {
                continue;
            }

            foreach (var subMenuItem in menuItem.GetAllNavigationViewItems())
            {
                yield return subMenuItem;
            }
        }
    }

    /// <summary>
    /// Retrieves all <see cref="NavigationViewItem"/> instances contained within the specified <see cref="NavigationViewItem"/>,
    /// including its immediate and nested child items.
    /// </summary>
    /// <param name="navigationViewItem">The parent <see cref="NavigationViewItem"/> to search within.</param>
    /// <returns>An enumerable collection of all nested <see cref="NavigationViewItem"/> instances.</returns>
    public static IEnumerable<NavigationViewItem> GetAllNavigationViewItems(this NavigationViewItem navigationViewItem)
    {
        if (navigationViewItem.MenuItems is null)
        {
            yield break;
        }

        foreach (var menuItem in navigationViewItem.MenuItems.OfType<NavigationViewItem>())
        {
            yield return menuItem;

            if (menuItem.MenuItems is null)
            {
                continue;
            }

            foreach (var subMenuItem in menuItem.MenuItems.OfType<NavigationViewItem>())
            {
                yield return subMenuItem;
            }
        }
    }


    /// <summary>
    /// Attempts to expand the specified <see cref="NavigationViewItem"/> within the given <see cref="NavigationView"/>.
    /// If the item is found, it sets its <see cref="NavigationViewItem.IsExpanded"/> property to true.
    /// Returns true if the item was found and expanded; otherwise, returns false.
    /// </summary>
    public static bool TryExpandItem(this NavigationView navigationView, NavigationViewItem targetItem)
    {
        if (navigationView.MenuItems is null)
        {
            return false;
        }

        foreach (var menuItem in navigationView.MenuItems.OfType<NavigationViewItem>())
        {
            if (menuItem == targetItem)
            {
                menuItem.IsExpanded = true;
                return true;
            }

            if (menuItem.TryExpandItem(targetItem))
            {
                menuItem.IsExpanded = true;
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Attempts to expand the specified <see cref="NavigationViewItem"/> within the current <see cref="NavigationViewItem"/> hierarchy.
    /// If the target item is found, sets its <see cref="NavigationViewItem.IsExpanded"/> property to true.
    /// Returns true if the item was found and expanded; otherwise, returns false.
    /// </summary>
    public static bool TryExpandItem(this NavigationViewItem navigationViewItem, NavigationViewItem targetItem)
    {
        if (navigationViewItem == targetItem)
        {
            navigationViewItem.IsExpanded = true;
            return true;
        }

        if (navigationViewItem.MenuItems is null)
        {
            return false;
        }

        foreach (NavigationViewItem subMenuItem in navigationViewItem.MenuItems.OfType<NavigationViewItem>())
        {
            if (subMenuItem == targetItem)
            {
                subMenuItem.IsExpanded = true;
                return true;
            }

            if (subMenuItem.MenuItems is null)
            {
                continue;
            }

            if (subMenuItem.TryExpandItem(targetItem))
            {
                return true;
            }
        }

        return false;
    }
}