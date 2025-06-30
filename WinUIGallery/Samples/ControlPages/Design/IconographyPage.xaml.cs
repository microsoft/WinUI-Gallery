// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinUIGallery.Helpers;
using WinUIGallery.Models;

namespace WinUIGallery.ControlPages;

public sealed partial class IconographyPage : Page
{
    public List<double> FontSizes { get; } = new()
        {
            16,
            24,
            32,
            48
        };

    private string currentSearch = null;

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
        DependencyProperty.Register("SelectedItem", typeof(IconData), typeof(IconographyPage), new PropertyMetadata(null));

    public IconographyPage()
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
                IconsItemsView.Select(0);
                SidePanel.Visibility = Visibility.Visible;
            });
        });
    }
    private void SetSampleCodePresenterCode(IconData value)
    {
        XAMLCodePresenterFont.Code = $"<FontIcon Glyph=\"{value.TextGlyph}\" />";

        CSharpCodePresenterFont.Code = "FontIcon icon = new FontIcon();" + Environment.NewLine + $"icon.Glyph = \"{value.CodeGlyph}\";";

        if (value.SymbolName != null)
        {
            XAMLCodePresenterSymbol.Code = $"<SymbolIcon Symbol=\"{value.SymbolName}\" />";

            CSharpCodePresenterSymbol.Code = "SymbolIcon icon = new SymbolIcon();" + Environment.NewLine + $"icon.Symbol = Symbol.{value.SymbolName};";
        }
        else
        {
            XAMLCodePresenterSymbol.Code = null;

            CSharpCodePresenterSymbol.Code = null;
        }
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
                        || item.Name.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase)
                        || item.Tags.Any(tag => string.IsNullOrEmpty(tag) is false && tag.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase)));

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
                    IconsItemsView.Select(0);
                }
                else
                {
                    outputString = "No icons found.";
                }

                UIHelper.AnnounceActionForAccessibility(IconsAutoSuggestBox, outputString, "AutoSuggestBoxNumberIconsFoundId");
            });
        }).Start();
    }

    private void IconsItemsView_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        if (IconsItemsView.ItemsSource is IList<IconData> currentItems)
        {
            if (IconsItemsView.CurrentItemIndex != -1 && IconsItemsView.CurrentItemIndex < currentItems.Count)
            {
                SelectedItem = currentItems[IconsItemsView.CurrentItemIndex];
            }
        }

        if (TagsItemsView != null)
        {
            TagsItemsView.Layout = new WrapLayout { VerticalSpacing = 4, HorizontalSpacing = 4 };
        }
    }

    private void TagsItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is string tag)
        {
            IconsAutoSuggestBox.Text = tag;
        }
    }
}