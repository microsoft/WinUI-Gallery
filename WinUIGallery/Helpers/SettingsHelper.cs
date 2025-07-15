// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
/// Provides utility methods for managing application settings, including operations for storing, retrieving,  and
/// manipulating lists of strings in the application's local settings.
/// </summary>
public static partial class SettingsHelper
{
    /// <summary>
    /// The maximum number of items to retain in the recently visited list.
    /// </summary>
    public const int MaxRecentlyVisitedSamples = 7;

    /// <summary>
    /// An instance of ApplicationData for the current user.
    /// </summary>
    private static ApplicationData appData = ApplicationData.GetDefault();

    /// <summary>
    /// Information Separator: Represents an invisible, JSON-safe delimiter used for internal string serialization.
    /// </summary>
    /// <remarks>This delimiter is intended for separating values in serialized strings where compatibility
    /// with JSON is a concern. It is not intended for public use or modification.</remarks>
    private const char delimiter = '\u001f';

    /// <summary>
    /// Attempts to add an item to a list associated with the specified key, ensuring no duplicates and optionally
    /// enforcing a size limit.
    /// </summary>
    /// <param name="key">The key identifying the list to which the item will be added. Cannot be null, empty, or whitespace.</param>
    /// <param name="item">The item to add to the list. Cannot be null, empty, or whitespace.</param>
    /// <param name="position">The position at which to insert the item in the list. Use <see cref="InsertPosition.First"/> to insert at the
    /// beginning, or <see cref="InsertPosition.Last"/> to insert at the end.</param>
    /// <param name="maxSize">The maximum allowed size of the list. If greater than zero, the list will be trimmed to this size after adding
    /// the item. Defaults to 0, meaning no size limit is enforced.</param>
    /// <returns><see langword="true"/> if the item was successfully added to the list and the list was saved; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool TryAddItem(string key, string item, InsertPosition position, int maxSize = 0)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(item))
            return false;

        bool enforceSizeLimit = maxSize > 0;
        var list = GetList(key);

        // Prevent duplicates by removing if already exists
        list.Remove(item);

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
    /// Attempts to remove the specified item from the list associated with the given key.
    /// </summary>
    /// <param name="key">The key identifying the list from which the item should be removed. Cannot be null or empty.</param>
    /// <param name="item">The item to remove from the list. Cannot be null.</param>
    /// <returns><see langword="true"/> if the item was successfully removed and the updated list was saved; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool TryRemoveItem(string key, string item)
    {
        if (!Exists(key) || !Contains(key, item))
        {
            return false;
        }

        var list = GetList(key);
        if (list.Remove(item))
        {
            return TrySaveList(key, list);
        }
        return false;
    }

    /// <summary>
    /// Retrieves a list of strings associated with the specified key from the application's local settings.
    /// </summary>
    /// <param name="key">The key used to locate the stored value in the application's local settings. Cannot be null.</param>
    /// <returns>A list of strings parsed from the stored value associated with the specified key.  Returns an empty list if the
    /// key does not exist or the stored value is null.</returns>
    public static List<string> GetList(string key)
    {
        string raw = appData.LocalSettings.Values[key] as string;
        return raw != null
            ? raw.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList()
            : new List<string>();
    }

    /// <summary>
    /// Determines whether the specified item exists within the set of values associated with the given key.
    /// </summary>
    /// <param name="key">The key used to retrieve the stored set of values. Cannot be null or empty.</param>
    /// <param name="item">The item to search for within the set of values. Cannot be null.</param>
    /// <returns><see langword="true"/> if the item exists in the set of values associated with the key; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool Contains(string key, string item)
    {
        string raw = appData.LocalSettings.Values[key] as string;
        if (string.IsNullOrEmpty(raw))
            return false;

        var set = new HashSet<string>(
            raw.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
        );

        return set.Contains(item);
    }

    /// <summary>
    /// Deletes the specified key and its associated value from the application's local settings.
    /// </summary>
    /// <param name="key">The key of the setting to remove. Cannot be <see langword="null"/> or empty.</param>
    public static void Delete(string key)
    {
        appData.LocalSettings.Values.Remove(key);
    }

    /// <summary>
    /// Determines whether a specified key exists in the application's local settings.
    /// </summary>
    /// <param name="key">The key to check for existence in the local settings. Cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the specified key exists and its associated value is not null or empty;  otherwise,
    /// <see langword="false"/>.</returns>
    public static bool Exists(string key)
    {
        string raw = appData.LocalSettings.Values[key] as string;
        return !string.IsNullOrEmpty(raw);
    }

    /// <summary>
    /// Attempts to save a list of strings to the application's local settings storage.
    /// </summary>
    /// <param name="key">The unique key used to store the list in local settings. Cannot be null or empty.</param>
    /// <param name="items">The list of strings to save. Cannot be null.</param>
    /// <returns><see langword="true"/> if the list is successfully saved; otherwise, <see langword="false"/> if the total size
    /// of the list exceeds the storage limit.</returns>
    private static bool TrySaveList(string key, List<string> items)
    {
        string joined = string.Join(delimiter, items);
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
