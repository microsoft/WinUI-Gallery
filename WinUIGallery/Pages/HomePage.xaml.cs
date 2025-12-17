// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
    IReadOnlyList<ControlInfoDataItem> RecentlyAddedOrUpdatedSamplesList;
    IReadOnlyList<ControlInfoDataItem> FavoriteSamplesList;

    public HomePage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ((NavigationViewItem)App.MainWindow.NavigationView.MenuItems.First()).IsSelected = true;

        Items = ControlInfoDataSource.Instance.Groups
            .SelectMany(g => g.Items)
            .OrderBy(i => i.Title)
            .ToList();

        RecentlyVisitedSamplesList = GetValidItems(SettingsHelper.Current.RecentlyVisited, isFavorite: false);
        RecentlyAddedOrUpdatedSamplesList = Items.Where(i => i.IsNew || i.IsUpdated).ToList();
        FavoriteSamplesList = GetValidItems(SettingsHelper.Current.Favorites, isFavorite: true);

        VisualStateManager.GoToState(this, RecentlyVisitedSamplesList.Count > 0 ? "Recent" : "NoRecent", true);
        VisualStateManager.GoToState(this, FavoriteSamplesList.Count > 0 ? "Favorites" : "NoFavorites", true);
    }

    public List<ControlInfoDataItem> GetValidItems(List<string> items, bool isFavorite)
    {
        if (items == null || items.Count == 0)
            return new List<ControlInfoDataItem>();

        Dictionary<string, ControlInfoDataItem> itemMap = Items.ToDictionary(i => i.UniqueId);

        List<ControlInfoDataItem> result = new();

        foreach (string id in items)
        {
            if (itemMap.TryGetValue(id, out var item))
            {
                result.Add(item);
            }
            else
            {
                if (isFavorite)
                {
                    SettingsHelper.Current.UpdateFavorites(items => items.Remove(id));
                }
                else
                {
                    SettingsHelper.Current.UpdateRecentlyVisited(items => items.Remove(id));
                }
            }
        }

        return result;
    }
}