using System.Collections.Generic;

namespace WinUIGallery.Helpers;

internal static class ListExtensions
{
    public const int MaxRecentlyVisitedSamples = 7;

    public static void AddToFirst(this List<string> list, string item, bool isFavorite)
    {
        if (string.IsNullOrWhiteSpace(item))
            return;

        list.Remove(item);
        list.Insert(0, item);

        if (!isFavorite && MaxRecentlyVisitedSamples > 0 && list.Count > MaxRecentlyVisitedSamples)
        {
            list.RemoveRange(MaxRecentlyVisitedSamples, list.Count - MaxRecentlyVisitedSamples);
        }
    }

    public static void AddToLast(this List<string> list, string item, bool isFavorite)
    {
        if (string.IsNullOrWhiteSpace(item))
            return;

        list.Remove(item);
        list.Add(item);

        if (!isFavorite && MaxRecentlyVisitedSamples > 0 && list.Count > MaxRecentlyVisitedSamples)
        {
            list.RemoveRange(0, list.Count - MaxRecentlyVisitedSamples);
        }
    }
}
