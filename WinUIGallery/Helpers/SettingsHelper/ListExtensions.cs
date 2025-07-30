using System;
using System.Collections.Generic;

namespace WinUIGallery.Helpers;

public static class ListExtensions
{
    public static void AddFirstFavoriteOrRecentlyVisited(this List<string> list, string item, bool isFavorite)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (string.IsNullOrWhiteSpace(item)) return;

        list.Remove(item); // Remove duplicates
        list.Insert(0, item);

        int maxSize = isFavorite
            ? SettingsHelper.Config.MaxFavoriteSamples
            : SettingsHelper.Config.MaxRecentlyVisitedSamples;

        // Enforce max size
        if (maxSize > 0 && list.Count > maxSize)
        {
            int excess = list.Count - maxSize;
            list.RemoveRange(maxSize, excess); // Remove from end
        }
    }

    public static void AddLastFavoriteOrRecentlyVisited(this List<string> list, string item, bool isFavorite)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (string.IsNullOrWhiteSpace(item)) return;

        list.Remove(item); // Remove duplicates
        list.Add(item);

        int maxSize = isFavorite
            ? SettingsHelper.Config.MaxFavoriteSamples
            : SettingsHelper.Config.MaxRecentlyVisitedSamples;

        // Enforce max size
        if (maxSize > 0 && list.Count > maxSize)
        {
            int excess = list.Count - maxSize;
            list.RemoveRange(0, excess); // Remove from start
        }
    }
}
