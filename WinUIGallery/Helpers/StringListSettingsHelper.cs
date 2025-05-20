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
/// Ensures data does not exceed size limits by refusing to save if too large.
/// </summary>
public static class StringListSettingsHelper
{
    // An instance of ApplicationData for the current user.
    private static ApplicationData appData = ApplicationData.GetDefault();

    // Unit Separator: invisible, JSON-safe delimiter for internal string serialization.
    private const char Delimiter = '\u001f';

    /// <summary>
    /// Adds an item to the list for the given key, ignoring duplicates, appended at the end.
    /// Returns true if save succeeded; false if data size limit exceeded or inputs invalid.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="item">Item to add</param>
    /// <returns>True if item added and saved; false if save rejected due to size or input.</returns>
    public static bool AddItem(string key, string item)
    {
        return AddItem(key, item, InsertPosition.Last, 0);
    }

    /// <summary>
    /// Adds an item to the list at the specified position, ignoring duplicates.
    /// Returns true if save succeeded; false otherwise.
    /// </summary>
    public static bool AddItem(string key, string item, InsertPosition position)
    {
        return AddItem(key, item, position, 0);
    }

    /// <summary>
    /// Adds an item to the list with a maximum size limit. Trims older items if limit exceeded.
    /// Returns true if save succeeded; false otherwise.
    /// </summary>
    public static bool AddItem(string key, string item, int maxSize)
    {
        return AddItem(key, item, InsertPosition.Last, maxSize);
    }

    /// <summary>
    /// Adds an item to the list at the specified position, with optional max size limit.
    /// Duplicate values are moved to the new position if already present.
    /// Returns true if save succeeded; false if save rejected due to size or invalid input.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="item">Item to add</param>
    /// <param name="position">Insert at First or Last</param>
    /// <param name="maxSize">Max number of items; zero or negative disables trimming.</param>
    /// <returns>True if item added and saved successfully; false otherwise.</returns>
    public static bool AddItem(string key, string item, InsertPosition position, int maxSize)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(item))
            return false;

        bool enforceSizeLimit = maxSize > 0;
        var list = GetList(key);

        // Prevent duplicates by removing if already exists
        list.Remove(item);

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

        return SaveList(key, list);
    }

    /// <summary>
    /// Removes an item from the list under the specified key, if present.
    /// Returns true if item removed and saved; false otherwise.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="item">Item to remove</param>
    /// <returns>True if removed and saved; false if not found or save failed.</returns>
    public static bool RemoveItem(string key, string item)
    {
        var list = GetList(key);
        if (list.Remove(item))
        {
            return SaveList(key, list);
        }
        return false;
    }

    /// <summary>
    /// Retrieves the list of strings stored under the given key.
    /// Returns an empty list if none exist.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <returns>List of strings</returns>
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
    /// Returns false if the serialized string exceeds size limit.
    /// </summary>
    /// <param name="key">Settings key</param>
    /// <param name="items">List of strings to save</param>
    /// <returns>True if save successful; false if size limit exceeded.</returns>
    private static bool SaveList(string key, List<string> items)
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