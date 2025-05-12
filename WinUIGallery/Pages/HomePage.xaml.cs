//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using WinUIGallery.Controls;
using WinUIGallery.Helpers;
using WinUIGallery.Models;

namespace WinUIGallery.Pages;

public sealed partial class HomePage : ItemsPageBase
{
    public HomePage()
    {
        this.InitializeComponent();
    }

    public string WinAppSdkDetails => VersionHelper.WinAppSdkDetails;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
        var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.First();
        menuItem.IsSelected = true;

        Items = ControlInfoDataSource.Instance.Groups
            .SelectMany(g => g.Items)
            .OrderBy(i => i.Title)
            .ToList();
    }

    private void SampleFilterBar_SelectionChanged(object sender, AdaptiveSelectorBarItem e)
    {
        if (e == null || e.Tag == null)
            return;

        string selectedTag = e.Tag.ToString();
        List<ControlInfoDataItem> filteredItems = [];

        switch (selectedTag)
        {
            case "RecentlyAdded":
                filteredItems = Items.Where(i => i.IsNew).ToList();
                break;
            case "RecentlyUpdated":
                filteredItems = Items.Where(i => i.IsUpdated && !i.IsNew).ToList();
                break;
            case "RecentlyVisited":
                filteredItems = GetValidItems(SettingsKeys.RecentlyVisited);
                break;
            case "FavoriteSamples":
                filteredItems = GetValidItems(SettingsKeys.Favorites);
                break;
            case "PreviewSamples":
                filteredItems = Items.Where(i => i.IsNew).ToList();
                break;
            case "AllSamples":
                filteredItems = (List<ControlInfoDataItem>)Items;
                break;
            default:
                Debug.WriteLine("Unknown filter tag.");
                break;
        }

        itemsRepeater.ItemsSource = filteredItems;

        if (filteredItems.Count == 0 && (selectedTag == "RecentlyVisited" || selectedTag == "FavoriteSamples"))
        {
            fallBackMessage.Visibility = Visibility.Visible;

            if (selectedTag == "RecentlyVisited")
            {
                fallBackMessageTitle.Text = "No recently visited samples.";
                fallBackMessageSubtitle.Text = "Samples you view will show up here.";
            }
            else if (selectedTag == "FavoriteSamples")
            {
                fallBackMessageTitle.Text = "No favorite samples yet.";
                fallBackMessageSubtitle.Text = "Mark samples as favorites to see them here.";
            }
        }
        else
        {
            fallBackMessage.Visibility = Visibility.Collapsed;
        }
    }

    public List<ControlInfoDataItem> GetValidItems(string settingsKey)
    {
        List<string> keyList = StringListSettingsHelper.GetList(settingsKey);

        if (keyList == null || keyList.Count == 0)
            return new List<ControlInfoDataItem>();

        Dictionary<string,ControlInfoDataItem> itemMap = Items.ToDictionary(i => i.UniqueId);

        List<ControlInfoDataItem> result = new();

        foreach (string id in keyList)
        {
            if (itemMap.TryGetValue(id, out var item))
            {
                result.Add(item);
            }
            else
            {
               StringListSettingsHelper.RemoveItem(settingsKey, id);
            }
        }

        return result;
    }
}