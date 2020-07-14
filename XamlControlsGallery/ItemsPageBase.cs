//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation.Metadata;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;

#if USING_CSWINRT
using System.ComponentModel;
#else
using Microsoft.UI.Xaml.Data;
#endif

namespace AppUIBasics
{
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
            var gridView = (GridView)sender;
            var item = (ControlInfoDataItem)e.ClickedItem;

            _itemId = item.UniqueId;

            this.Frame.Navigate(typeof(ItemPage), _itemId, new DrillInNavigationTransitionInfo());
        }

        protected void OnItemGridViewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Up)
            {
                var nextElement = FocusManager.FindNextElement(FocusNavigationDirection.Up);
                if (nextElement?.GetType() == typeof(Microsoft.UI.Xaml.Controls.NavigationViewItem))
                {
                    NavigationRootPage.Current.PageHeader.Focus(FocusState.Programmatic);
                }
                else
                {
                    FocusManager.TryMoveFocus(FocusNavigationDirection.Up);
                }
            }
        }

        protected async void OnItemGridViewLoaded(object sender, RoutedEventArgs e)
        {
            if (_itemId != null)
            {
                var gridView = (GridView)sender;
                var items = gridView.ItemsSource as IEnumerable<ControlInfoDataItem>;
                var item = items?.FirstOrDefault(s => s.UniqueId == _itemId);
                if (item != null)
                {
                    gridView.ScrollIntoView(item);

                    if (NavigationRootPage.Current.IsFocusSupported)
                    {
                        ((GridViewItem)gridView.ContainerFromItem(item))?.Focus(FocusState.Programmatic);
                    }

                    ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("controlAnimation");

                    if (animation != null)
                    {
                        // Setup the "basic" configuration if the API is present. 
                        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
                        {
                            animation.Configuration = new BasicConnectedAnimationConfiguration();
                        }

                        await gridView.TryStartConnectedAnimationAsync(animation, item, "controlRoot");
                    }
                }
            }
        }

        protected void OnItemGridViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gridView = (GridView)sender;

            if (gridView.ItemsPanelRoot is ItemsWrapGrid wrapGrid)
            {
                if (GetIsNarrowLayoutState())
                {
                    wrapGrid.ItemWidth = gridView.ActualWidth - gridView.Padding.Left - gridView.Padding.Right;
                }
                else
                {
                    wrapGrid.ItemWidth = double.NaN;
                }
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
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
}
