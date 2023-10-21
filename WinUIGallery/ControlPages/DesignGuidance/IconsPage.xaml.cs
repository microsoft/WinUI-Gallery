//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinUIGallery.DesktopWap.DataModel;

namespace AppUIBasics.ControlPages
{
    public sealed partial class IconsPage : Page
    {
        public ObservableCollection<IconData> FilteredItems = new();

        public List<double> FontSizes { get; } = new ()
            {
                16,
                24,
                32,
                48
            };

        public IconData SelectedItem
        {
            get { return (IconData)GetValue(SelectedItemProperty); }
            set {
                SetValue(SelectedItemProperty, value);
                SetSampleCodePresenterCode(value);
            }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(IconData), typeof(IconsPage), new PropertyMetadata(null));

        public IconsPage()
        {
            // Fill filtered items
            IconsDataSource.Icons.ForEach(item => FilteredItems.Add(item));
            this.InitializeComponent();
            IconsRepeater.Loaded += ItemsGridView_Loaded;
        }

        private void ItemsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            // Delegate loading of icons, so we have smooth navigating to this page
            // and not unnecessarily block UI Thread
            Task.Run(delegate ()
            {
                _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.High, () =>
                {
                    IconsRepeater.ItemsSource = FilteredItems;
                    SelectedItem = FilteredItems[0];
                    SetSampleCodePresenterCode(FilteredItems[0]);
                });
            });
        }
        private void SetSampleCodePresenterCode(IconData value)
        {
            XAMLCodePresenter.Code = $"<FontIcon Glyph=\"{value.TextGlyph}\" />";

            CSharpCodePresenter.Code = $"FontIcon icon = new FontIcon();" + Environment.NewLine + "icon.Glyph = \"" + value.CodeGlyph + "\";";
        }

        private void SearchTextBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            Filter((sender as AutoSuggestBox).Text);
        }
        
        public void Filter(string search)
        {
            string[] filter = search?.Split(" ");

            FilteredItems.Clear();

            foreach (var item in IconsDataSource.Icons)
            {
                var fitsFilter = filter.All(entry => item.Code.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase)
                        || item.Name.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase));

                if (fitsFilter)
                {
                    FilteredItems.Add(item);
                }
            }
            if (FilteredItems.Count > 0)
            {
                SelectedItem = FilteredItems[0];
            }
        }

        private void Icons_TemplatePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var oldIndex = FilteredItems.IndexOf(SelectedItem);
            var previousItem = IconsRepeater.TryGetElement(oldIndex);
            if(previousItem != null)
            {
                MoveToSelectionState(previousItem, false);
            }

            var itemIndex = IconsRepeater.GetElementIndex(sender as UIElement);
            SelectedItem = FilteredItems[itemIndex != -1 ? itemIndex : 0];
            MoveToSelectionState(sender as UIElement, true);
        }

        private static void MoveToSelectionState(UIElement previousItem, bool isSelected)
        {
            VisualStateManager.GoToState(previousItem as Control, isSelected ? "Selected" : "Default", false);
        }

        private void IconsRepeater_ElementIndexChanged(ItemsRepeater sender, ItemsRepeaterElementIndexChangedEventArgs args)
        {
            var newItem = FilteredItems[args.NewIndex];
            MoveToSelectionState(args.Element, newItem == SelectedItem);
        }

        private void IconsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
        {
            var newItem = FilteredItems[args.Index];
            MoveToSelectionState(args.Element, newItem == SelectedItem);
        }
    }
}
