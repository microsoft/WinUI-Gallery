// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinUIGallery.Helpers;

/// <summary>
/// Provides methods to migrate legacy application settings for Favorites and RecentlyVisited items
/// from the old comma-separated format stored in LocalSettings to the new JSON-based settings system.
/// 
/// Key points:
/// - Only operates in packaged app mode (NativeMethods.IsAppPackaged).
/// </summary>
public partial class SettingsMigration
{
    private static ApplicationData appData = ApplicationData.GetDefault();

    private const string RecentlyVisitedKey = "RecentlyVisited";
    private const string FavoritesKey = "Favorites";
    private const char delimiter = '\u001f';

    public static List<string> GetOldList(string key)
    {
        if (NativeMethods.IsAppPackaged)
        {
            string raw = appData.LocalSettings.Values[key] as string;
            if (string.IsNullOrEmpty(raw))
                return null;

            raw = raw.Trim();

            // Only handle old comma-separated format
            if (raw.StartsWith("[") && raw.EndsWith("]"))
            {
                // Looks like JSON → treat as new format, return null
                return null;
            }

            // Old comma-separated format
            return raw.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => s.Trim())
                      .ToList();
        }

        return null;
    }

    public static void MigrateRecentlyVisited()
    {
        var oldList = GetOldList(RecentlyVisitedKey);

        if (oldList == null || oldList.Count == 0)
            return;

        appData.LocalSettings.Values.Remove(RecentlyVisitedKey);

        SettingsHelper.Current.UpdateRecentlyVisited(list =>
        {
            foreach (var item in oldList)
                list.AddAsFirst(item, null);
        });
    }

    public static void MigrateFavorites()
    {
        var oldList = GetOldList(FavoritesKey);

        if (oldList == null || oldList.Count == 0)
            return;

        appData.LocalSettings.Values.Remove(FavoritesKey);

        SettingsHelper.Current.UpdateFavorites(list =>
        {
            foreach (var item in oldList)
                list.AddAsLast(item, null);
        });
    }
}