using System.Collections.Generic;

namespace WinUIGallery.Helpers;

internal static class ListExtensions
{
    public static void AddAsFirst<T>(this List<T> list, T item, int? maxSize = null)
    {
        if (item == null || (item is string data && string.IsNullOrWhiteSpace(data)))
            return;

        list.Remove(item);
        list.Insert(0, item);

        if (maxSize.HasValue && maxSize.Value > 0 && list.Count > maxSize.Value)
        {
            list.RemoveRange(maxSize.Value, list.Count - maxSize.Value);
        }
    }

    public static void AddAsLast<T>(this List<T> list, T item, int? maxSize = null)
    {
        if (item == null || (item is string data && string.IsNullOrWhiteSpace(data)))
            return;

        list.Remove(item);
        list.Add(item);

        if (maxSize.HasValue && maxSize.Value > 0 && list.Count > maxSize.Value)
        {
            list.RemoveRange(0, list.Count - maxSize.Value);
        }
    }
}
