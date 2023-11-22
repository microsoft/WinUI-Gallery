//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AppUIBasics.Common;
using System.Text.Json;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app
// is first launched.

namespace AppUIBasics.Data
{
    public class Root
    {
        public ObservableCollection<ControlInfoDataGroup> Groups { get; set; }
    }
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class ControlInfoDataItem
    {
        public ControlInfoDataItem(string uniqueId, string title, string apiNamespace, string subtitle, string imagePath, string iconGlyph, string badgeString, string description, string content, bool isNew, bool isUpdated, bool isPreview, bool hideSourceCodeAndRelatedControls, ObservableCollection<ControlInfoDocLink> docs, ObservableCollection<string> relatedControls)
        {
            this.UniqueId = uniqueId;
            this.Title = title;

            this.ApiNamespace = apiNamespace;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.IconGlyph = iconGlyph;
            this.BadgeString = badgeString;
            this.Content = content;
            this.IsNew = isNew;
            this.IsUpdated = isUpdated;
            this.IsPreview = isPreview;
            this.Docs = docs;
            this.RelatedControls = relatedControls;
            this.HideSourceCodeAndRelatedControls = hideSourceCodeAndRelatedControls;
        }

        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string ApiNamespace { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IconGlyph { get; set; }
        public string BadgeString { get; set; }
        public string Content { get; set; }
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsPreview { get; set; }
        public bool HideSourceCodeAndRelatedControls { get; set; }
        public ObservableCollection<ControlInfoDocLink> Docs { get; set; }
        public ObservableCollection<string> RelatedControls { get; set; }

        public bool IncludedInBuild { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class ControlInfoDocLink
    {
        public ControlInfoDocLink(string title, string uri)
        {
            this.Title = title;
            this.Uri = uri;
        }
        public string Title { get; set; }
        public string Uri { get; set; }
    }


    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class ControlInfoDataGroup
    {
        public ControlInfoDataGroup(string uniqueId, string title, string subtitle, string imagePath, string iconGlyph, string description, string apiNamespace, string folder, bool isSpecialSection)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.ApiNamespace = apiNamespace;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.IconGlyph = iconGlyph;
            this.Folder = folder;
            this.Items = new ObservableCollection<ControlInfoDataItem>();
            this.IsSpecialSection = isSpecialSection;
        }

        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IconGlyph { get; set; }
        public string ApiNamespace { get; set; }
        public bool IsSpecialSection { get; set; }
        public string Folder { get; set; }
        public ObservableCollection<ControlInfoDataItem> Items { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    ///
    /// ControlInfoSource initializes with data read from a static json file included in the
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class ControlInfoDataSource
    {
        private static readonly object _lock = new object();

        #region Singleton

        private static ControlInfoDataSource _instance;

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

        private IList<ControlInfoDataGroup> _groups = new List<ControlInfoDataGroup>();
        public IList<ControlInfoDataGroup> Groups
        {
            get { return this._groups; }
        }

        public async Task<IEnumerable<ControlInfoDataGroup>> GetGroupsAsync()
        {
            await _instance.GetControlInfoDataAsync();

            return _instance.Groups;
        }

        public async Task<ControlInfoDataGroup> GetGroupAsync(string uniqueId)
        {
            await _instance.GetControlInfoDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _instance.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public async Task<ControlInfoDataItem> GetItemAsync(string uniqueId)
        {
            await _instance.GetControlInfoDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _instance.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() > 0) return matches.First();
            return null;
        }

        public async Task<ControlInfoDataGroup> GetGroupFromItemAsync(string uniqueId)
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

            var jsonText = await FileLoader.LoadText("DataModel/ControlInfoData.json");
            var controlInfoDataGroup = JsonSerializer.Deserialize<Root>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            lock (_lock)
            {
                string pageRoot = "AppUIBasics.ControlPages.";

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
}
