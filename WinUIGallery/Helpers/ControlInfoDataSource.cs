// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WinUIGallery.Models;

namespace WinUIGallery.Helpers;

/// <summary>
/// Creates a collection of groups and items with content read from a static json file.
///
/// ControlInfoSource initializes with data read from a static json file included in the
/// project.  This provides sample data at both design-time and run-time.
/// </summary>
public sealed partial class ControlInfoDataSource
{
    private static readonly object _lock = new();

    #region Singleton

    private static readonly ControlInfoDataSource _instance;

    public static ControlInfoDataSource Instance
    {
        get
        {
            return _instance;
        }
    }

    static ControlInfoDataSource()
    {
        _instance = new ControlInfoDataSource();
    }

    private ControlInfoDataSource() { }

    #endregion

    private readonly IList<ControlInfoDataGroup> _groups = new List<ControlInfoDataGroup>();
    public IList<ControlInfoDataGroup> Groups
    {
        get { return this._groups; }
    }

    public async Task<IEnumerable<ControlInfoDataGroup>> GetGroupsAsync()
    {
        await _instance.GetControlInfoDataAsync();

        return _instance.Groups;
    }

    public static async Task<ControlInfoDataGroup> GetGroupAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        // Simple linear search is acceptable for small data sets
        var matches = _instance.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
        if (matches.Count() == 1) return matches.First();
        return null;
    }

    public static async Task<ControlInfoDataItem> GetItemAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        // Simple linear search is acceptable for small data sets
        var matches = _instance.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
        if (matches.Count() > 0) return matches.First();
        return null;
    }

    public static async Task<ControlInfoDataGroup> GetGroupFromItemAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        var matches = _instance.Groups.Where((group) => group.Items.FirstOrDefault(item => item.UniqueId.Equals(uniqueId)) != null);
        if (matches.Count() == 1) return matches.First();
        return null;
    }

    private async Task GetControlInfoDataAsync()
    {
        lock (_lock)
        {
            if (this.Groups.Count() != 0)
            {
                return;
            }
        }

        var jsonText = await FileLoader.LoadText("Samples/Data/ControlInfoData.json");
        var controlInfoDataGroup = JsonSerializer.Deserialize(jsonText, typeof(Root), RootContext.Default) as Root;

        lock (_lock)
        {
            string pageRoot = "WinUIGallery.ControlPages.";

            controlInfoDataGroup.Groups.SelectMany(g => g.Items).ToList().ForEach(item =>
            {
#nullable enable
                string? badgeString = item switch
                {
                    { IsNew: true } => "New",
                    { IsUpdated: true } => "Updated",
                    { IsPreview: true } => "Preview",
                    _ => null
                };
                string pageString = $"{pageRoot}{item.UniqueId}Page";
                Type? pageType = Type.GetType(pageString);

                item.BadgeString = badgeString;
                item.IncludedInBuild = pageType is not null;
                item.ImagePath ??= "ms-appx:///Assets/ControlImages/Placeholder.png";
#nullable disable
            });

            foreach (var group in controlInfoDataGroup.Groups)
            {
                if (!Groups.Any(g => g.Title == group.Title))
                {
                    Groups.Add(group);
                }
            }
        }
    }
}
