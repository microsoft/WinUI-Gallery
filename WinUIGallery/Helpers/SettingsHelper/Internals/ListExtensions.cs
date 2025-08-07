using System.Collections.Generic;

namespace WinUIGallery.Helpers;

internal static class ListExtensions
{
    public static void AddToFirst(this List<string> list, string item, bool isFavorite)
    {
        if (string.IsNullOrWhiteSpace(item))
            return;

        list.Remove(item);
        list.Insert(0, item);

        if (!isFavorite && SettingsHelper.MaxRecentlyVisitedSamples > 0 && list.Count > SettingsHelper.MaxRecentlyVisitedSamples)
        {
            list.RemoveRange(SettingsHelper.MaxRecentlyVisitedSamples, list.Count - SettingsHelper.MaxRecentlyVisitedSamples);
        }
    }

    public static void AddToLast(this List<string> list, string item, bool isFavorite)
    {
        if (string.IsNullOrWhiteSpace(item))
            return;

        list.Remove(item);
        list.Add(item);

        if (!isFavorite && SettingsHelper.MaxRecentlyVisitedSamples > 0 && list.Count > SettingsHelper.MaxRecentlyVisitedSamples)
        {
            list.RemoveRange(0, list.Count - SettingsHelper.MaxRecentlyVisitedSamples);
        }
    }
}
