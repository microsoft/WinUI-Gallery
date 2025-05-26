using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WinUIGallery.Helpers;

/// <summary>
/// Enum to specify whether an item should be inserted at the beginning or end of the list.
/// </summary>
public enum InsertPosition
{
    /// <summary>
    /// Insert at the beginning of the list.
    /// </summary>
    First,

    /// <summary>
    /// Insert at the end of the list.
    /// </summary>
    Last
}

/// <summary>
/// Helper for storing and managing string-based lists (e.g., favorites, history)
/// in LocalSettings of ApplicationData using a compact, delimiter-based format.
/// </summary>
public static class StringListSettingsHelper
{
    // An instance of ApplicationData for the current user.
    private static ApplicationData appData = ApplicationData.GetDefault();

    // Unit Separator: invisible, JSON-safe delimiter for internal string serialization.
    private const char Delimiter = '\u001f';

    /// <summary>
    /// Adds an item to the list at the specified position, with optional max size limit.
    /// Duplicate values are moved to the new position if already present.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="item">Item to add</param>
    /// <param name="position">Insert at First or Last</param>
    /// <param name="maxSize">Max number of items; zero or negative disables trimming.</param>
    /// <param name="trimEnabled">If false, prevents adding an item when size exceeds maxSize. Default is true.</param>
    /// <returns>True if item added and saved successfully; false otherwise.</returns>
    public static bool TryAddItem(string key, string item, InsertPosition position, int maxSize, bool trimEnabled = true)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(item))
            return false;

        bool enforceSizeLimit = maxSize > 0;
        var list = GetList(key);

        // Prevent duplicates by removing if already exists
        list.Remove(item);

        // Check if trimming is not enabled and list size is at the limit
        if (!trimEnabled && enforceSizeLimit && list.Count >= maxSize)
            return false; // Do not add item if trimming is not enabled and size limit is reached

        // Add item at the specified position
        if (position == InsertPosition.First)
            list.Insert(0, item);
        else
            list.Add(item);

        // Trim list if needed
        if (enforceSizeLimit && list.Count > maxSize)
        {
            int excess = list.Count - maxSize;
            if (position == InsertPosition.First)
                list.RemoveRange(maxSize, excess); // Remove from end
            else
                list.RemoveRange(0, excess); // Remove from start
        }

        return TrySaveList(key, list);
    }

    /// <summary>
    /// Removes an item from the list under the specified key, if present.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="item">Item to remove</param>
    /// <returns>True if removed and saved; false if not found or save failed.</returns>
    public static bool TryRemoveItem(string key, string item)
    {
        var list = GetList(key);
        if (list.Remove(item))
        {
            return TrySaveList(key, list);
        }
        return false;
    }

    /// <summary>
    /// Retrieves the list of strings stored under the given key.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <returns>List of strings (returns an empty list if none exist).</returns>
    public static List<string> GetList(string key)
    {
        string raw = appData.LocalSettings.Values[key] as string;
        return raw != null
            ? raw.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries).ToList()
            : new List<string>();
    }

    /// <summary>
    /// Checks whether the specified item exists in the list stored under the key.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="item">Item to check for</param>
    /// <returns>True if item exists, false otherwise</returns>
    public static bool Contains(string key, string item)
    {
        return GetList(key).Contains(item);
    }

    /// <summary>
    /// Clears the entire list stored under the given key.
    /// </summary>
    /// <param name="key">Settings key</param>
    public static void ClearList(string key)
    {
        appData.LocalSettings.Values.Remove(key);
    }

    /// <summary>
    /// Saves the list back to LocalSettings using delimiter serialization.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="items">List of strings to save</param>
    /// <returns>True if save successful; false if size limit exceeded.</returns>
    private static bool TrySaveList(string key, List<string> items)
    {
        string joined = string.Join(Delimiter, items);
        int byteSize = System.Text.Encoding.Unicode.GetByteCount(joined);

        // If the size exceeds or equals 8 KB (8192 bytes), reject the save operation
        if (byteSize >= 8192)
        {
            Debug.WriteLine($"Storage limit exceeded for key: {key}.");
            return false;
        }

        appData.LocalSettings.Values[key] = joined;
        return true;
    }
}

public static class SettingsKeys
{
    /// <summary>
    /// Key for the list of favorited Pages.
    /// </summary>
    public const string Favorites = "Favorites";

    /// <summary>
    /// Key for the list of recently visited Pages.
    /// </summary>
    public const string RecentlyVisited = "RecentlyVisited";
}