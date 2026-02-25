// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using WinUIGallery.Models;

namespace WinUIGallery.Helpers;

/// <summary>
/// Manages the app's taskbar JumpList, providing quick access to
/// recently visited items and user-favorited items.
/// </summary>
internal static class JumpListHelper
{
    /// <summary>
    /// Rebuilds the JumpList with recently visited items and the current set of favorite items.
    /// Safe to call at any time; silently returns if JumpList is not supported.
    /// </summary>
    public static async Task UpdateJumpListAsync()
    {
        if (!NativeMethods.IsAppPackaged || !JumpList.IsSupported())
        {
            return;
        }

        try
        {
            JumpList jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.None;

            // Dynamic group: Recent
            var recentlyVisited = SettingsHelper.Current.RecentlyVisited;
            foreach (string uniqueId in recentlyVisited)
            {
                AddItemTask(jumpList, uniqueId, groupName: "Recent");
            }

            // Dynamic group: Favorites
            var favorites = SettingsHelper.Current.Favorites;
            foreach (string uniqueId in favorites)
            {
                AddItemTask(jumpList, uniqueId, groupName: "Favorites");
            }

            await jumpList.SaveAsync();
        }
        catch
        {
            // JumpList updates are best-effort; don't crash the app.
        }
    }

    private static void AddItemTask(JumpList jumpList, string uniqueId, string groupName)
    {
        ControlInfoDataItem? item = ControlInfoDataSource.Instance.Groups
            .SelectMany(g => g.Items)
            .FirstOrDefault(i => i.UniqueId == uniqueId);

        if (item == null)
        {
            return;
        }

        JumpListItem task = JumpListItem.CreateWithArguments(uniqueId, item.Title);
        task.GroupName = groupName;
        task.Description = "Go to " + item.Title;
        task.Logo = new Uri(item.ImagePath);
        jumpList.Items.Add(task);
    }
}