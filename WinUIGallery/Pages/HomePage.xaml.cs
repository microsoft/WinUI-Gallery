//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;
using WinUIGallery.Helpers;
using WinUIGallery.Models;

namespace WinUIGallery.Pages;

public sealed partial class HomePage : ItemsPageBase
{
    IReadOnlyList<ControlInfoDataItem> RecentlyVisitedSamplesList;
    IReadOnlyList<ControlInfoDataItem> RecentlyUpdatedSamplesList;
    IReadOnlyList<ControlInfoDataItem> FavoriteSamplesList;

    public HomePage()
    {
        this.InitializeComponent();
    }


    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
        var menuItem = (NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.First();
        menuItem.IsSelected = true;

        Items = ControlInfoDataSource.Instance.Groups
            .SelectMany(g => g.Items)
            .OrderBy(i => i.Title)
            .ToList();

        RecentlyVisitedSamplesList = GetValidItems(SettingsKeys.RecentlyVisited);
        RecentlyUpdatedSamplesList = Items.Where(i => i.IsNew || i.IsUpdated).ToList();
        FavoriteSamplesList = GetValidItems(SettingsKeys.Favorites);

        VisualStateManager.GoToState(this, RecentlyVisitedSamplesList.Count > 0 ? "Recent" : "NoRecent", true);
        VisualStateManager.GoToState(this, FavoriteSamplesList.Count > 0 ? "Favorites" : "NoFavorites", true);
    }

    public List<ControlInfoDataItem> GetValidItems(string settingsKey)
    {
        List<string> keyList = SettingsHelper.GetList(settingsKey);

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
               SettingsHelper.TryRemoveItem(settingsKey, id);
            }
        }

        return result;
    }
}