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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using WinUIGallery.Helpers;
using WinUIGallery.Models;
using Microsoft.UI.Xaml.Hosting;
using System.Threading.Tasks;
using System;

namespace WinUIGallery.Pages;

public sealed partial class HomePage : ItemsPageBase
{
    IReadOnlyList<ControlInfoDataItem> RecentlyVisitedSamplesList;
    IReadOnlyList<ControlInfoDataItem> RecentlyAddedSamplesList;
    IReadOnlyList<ControlInfoDataItem> RecentlyUpdatedSamplesList;
    IReadOnlyList<ControlInfoDataItem> FavoriteSamplesList;

    public HomePage()
    {
        this.InitializeComponent();
    }

    public string WinAppSdkDetails => VersionHelper.WinAppSdkDetails;

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
        RecentlyAddedSamplesList = Items.Where(i => i.IsNew).ToList();
        RecentlyUpdatedSamplesList = Items.Where(i => i.IsUpdated && !i.IsNew).ToList();
        FavoriteSamplesList = GetValidItems(SettingsKeys.Favorites);

        if (RecentlyVisitedSamplesList.Count > 0)
        {
            RecentlyVisitedTiltle.Visibility = Visibility.Visible;
            RecentlyVisitedContainer.Visibility = Visibility.Visible;
        }
        else
        {
            RecentlyVisitedTiltle.Visibility = Visibility.Collapsed;
            RecentlyVisitedContainer.Visibility = Visibility.Collapsed;
        }

        if (FavoriteSamplesList.Count > 0)
        {
            FavoriteSamplesFallbackMessage.Visibility = Visibility.Collapsed;
            FavoriteSamples.Visibility = Visibility.Visible;
        }
        else
        {
            FavoriteSamplesFallbackMessage.Visibility = Visibility.Visible;
            FavoriteSamples.Visibility = Visibility.Collapsed;
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

    private async void CategorySelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender is not SelectorBar selectorBar) return;
        if (selectorBar.SelectedItem is not SelectorBarItem item) return;
        if (item.Tag is not string tag) return;

        switch (tag)
        {
            case "Recent":
                await FavoriteSamplesPanel.FadeOut();
                await RecentSamplesPanel.FadeIn();
                break;
            case "Favorites":
                await RecentSamplesPanel.FadeOut();
                await FavoriteSamplesPanel.FadeIn();
                break;
            default:
                Debug.Assert(false, $"Unknown tag: {tag}");
                break;
        }
    }
}

public static class UIElementExtensions
{
    public static async Task FadeIn(this UIElement element, double duration = 150)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        var fadeIn = compositor.CreateScalarKeyFrameAnimation();
        fadeIn.InsertKeyFrame(1f, 1f);
        fadeIn.Duration = TimeSpan.FromMilliseconds(duration);

        element.Visibility = Visibility.Visible;
        visual.Opacity = 0;
        visual.StartAnimation("Opacity", fadeIn);

        await Task.Delay((int)duration);
    }

    public static async Task FadeOut(this UIElement element, double duration = 150)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        var fadeOut = compositor.CreateScalarKeyFrameAnimation();
        fadeOut.InsertKeyFrame(1f, 0f);
        fadeOut.Duration = TimeSpan.FromMilliseconds(duration);

        visual.StartAnimation("Opacity", fadeOut);
        await Task.Delay((int)duration);

        element.Visibility = Visibility.Collapsed;
    }
}