// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WinUIGallery.Models;

namespace WinUIGallery.Pages;

public abstract class ItemsPageBase : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _itemId;
    private IEnumerable<ControlInfoDataItem> _items;

    public IEnumerable<ControlInfoDataItem> Items
    {
        get { return _items; }
        set { SetProperty(ref _items, value); }
    }

    /// <summary>
    /// Gets a value indicating whether the application's view is currently in "narrow" mode - i.e. on a mobile-ish device.
    /// </summary>
    protected virtual bool GetIsNarrowLayoutState()
    {
        throw new NotImplementedException();
    }

    protected void OnItemGridViewContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (sender.ContainerFromItem(sender.Items.LastOrDefault()) is GridViewItem container)
        {
            container.XYFocusDown = container;
        }

        var item = args.Item as ControlInfoDataItem;
        if (item != null)
        {
            args.ItemContainer.IsEnabled = item.IncludedInBuild;
        }
    }

    protected void OnItemGridViewItemClick(object sender, ItemClickEventArgs e)
    {
        var item = (ControlInfoDataItem)e.ClickedItem;

        _itemId = item.UniqueId;

        App.MainWindow.Navigate(typeof(ItemPage), _itemId, new DrillInNavigationTransitionInfo());
    }

    protected void OnItemGridViewLoaded(object sender, RoutedEventArgs e)
    {
        if (_itemId != null)
        {
            var gridView = (GridView)sender;
            var items = gridView.ItemsSource as IEnumerable<ControlInfoDataItem>;
            var item = items?.FirstOrDefault(s => s.UniqueId == _itemId);
            if (item != null)
            {
                gridView.ScrollIntoView(item);
                ((GridViewItem)gridView.ContainerFromItem(item))?.Focus(FocusState.Programmatic);
            }
        }
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value)) return false;

        storage = value;
        NotifyPropertyChanged(propertyName);
        return true;
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
