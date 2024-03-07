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
using WinUIGallery.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinUIGallery.DesktopWap.DataModel;
using System.Threading;

namespace WinUIGallery.ControlPages
{
    public sealed partial class IconsPage : Page
    {
        public List<double> FontSizes { get; } = new()
            {
                16,
                24,
                32,
                48
            };

        private string currentSearch = null;
        private Thread searchThread = null;

        public IconData SelectedItem
        {
            get { return (IconData)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
                SetSampleCodePresenterCode(value);
            }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(IconData), typeof(IconsPage), new PropertyMetadata(null));

        public IconsPage()
        {
            // Fill filtered items
            this.InitializeComponent();
            IconsItemsView.Loaded += IconsItemsView_Loaded;
        }

        private void IconsItemsView_Loaded(object sender, RoutedEventArgs e)
        {
            // Delegate loading of icons, so we have smooth navigating to this page
            // and not unnecessarily block UI Thread
            Task.Run(delegate ()
            {
                _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.High, () =>
                {
                    IconsItemsView.ItemsSource = new List<IconData>(IconsDataSource.Icons);
                    SelectedItem = IconsDataSource.Icons[0];
                    SetSampleCodePresenterCode(IconsDataSource.Icons[0]);
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
            // Clearing itemssource so user thinks we are doing something
            IconsItemsView.ItemsSource = null;
            // Setting current search to trigger breaking condition of other threads
            currentSearch = search;

            string[] filter = search?.Split(" ");

            // Spawning a new thread to not have the UI freeze because of our search
            new Thread(() =>
            {
                var newItems = new List<IconData>();
                foreach (var item in IconsDataSource.Icons)
                {
                    // Skip UI update if this thread is not handling the current search term
                    if (search != currentSearch)
                    {
                        return;
                    }

                    var fitsFilter = filter.All(entry => item.Code.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase)
                            || item.Name.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase));

                    if (fitsFilter)
                    {
                        newItems.Add(item);
                    }
                }

                // Skip UI update if this thread is not handling the current search term
                if (search != currentSearch) return;

                // Updates to anything UI related (e.g. setting ItemsSource) need to be run on UI thread so queue it through dispatcher
                DispatcherQueue.TryEnqueue(() =>
                {
                    IconsItemsView.ItemsSource = newItems;

                    string outputString;
                    var filteredItemsCount = newItems.Count;
                    if (filteredItemsCount > 0)
                    {
                        SelectedItem = newItems[0];
                        outputString = filteredItemsCount > 1 ? filteredItemsCount + " icons found." : "1 icon found.";
                    }
                    else
                    {
                        outputString = "No icon found.";
                    }

                    UIHelper.AnnounceActionForAccessibility(IconsAutoSuggestBox, outputString, "AutoSuggestBoxNumberIconsFoundId");
                });
            }).Start();
        }

        private void IconsItemsView_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
        {
            if (IconsItemsView.CurrentItemIndex != -1)
            {
                SelectedItem = (IconsItemsView.ItemsSource as IList<IconData>)[IconsItemsView.CurrentItemIndex];
            }
        }
    }
}
