// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;
using WinUIGallery.Helpers;
using WinUIGallery.Models;
using WinUIGallery.Telemetry;
using WinUIGallery.Telemetry.Events;

namespace WinUIGallery.Pages;

public sealed partial class HomePage : ItemsPageBase
{
    IReadOnlyList<ControlInfoDataItem>? RecentlyVisitedSamplesList;
    IReadOnlyList<ControlInfoDataItem>? RecentlyAddedOrUpdatedSamplesList;
    IReadOnlyList<ControlInfoDataItem>? FavoriteSamplesList;

    public HomePage()
    {
        this.InitializeComponent();
        Loaded += OnHomePageLoaded;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        NavigatedToPageEvent.Log(nameof(HomePage));

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
        if (Items == null || items == null || items.Count == 0)
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

    private void OnHomePageLoaded(object sender, RoutedEventArgs e)
    {
        if (!SettingsHelper.Current.IsDiagnosticsMessageDismissed && PrivacyConsentHelpers.IsPrivacySensitiveRegion())
        {
            DiagnosticsInfoBar.IsOpen = true;
        }
    }

    private void DiagnosticsYesButton_Click(object sender, RoutedEventArgs e)
    {
        HandleDiagnosticsSetting(true);
    }

    private void DiagnosticsNoButton_Click(object sender, RoutedEventArgs e)
    {
        HandleDiagnosticsSetting(false);
    }

    private void HandleDiagnosticsSetting(bool isEnabled)
    {
        DiagnosticsInfoBar.IsOpen = false;
        SettingsHelper.Current.IsDiagnosticsMessageDismissed = true;
        SettingsHelper.Current.IsDiagnosticDataEnabled = isEnabled;
        TelemetryFactory.Get<ITelemetry>().IsDiagnosticTelemetryOn = isEnabled;
    }
}