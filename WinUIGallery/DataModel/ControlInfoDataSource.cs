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
using Windows.Data.Json;
using AppUIBasics.Common;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app
// is first launched.

namespace AppUIBasics.Data
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class ControlInfoDataItem
    {
        public ControlInfoDataItem(string uniqueId, string title, string apiNamespace, string subtitle, string imagePath, string imageIconPath, string badgeString, string description, string content, bool isNew, bool isUpdated, bool isPreview, bool hideSourceCodeAndRelatedControls)
        {
            this.UniqueId = uniqueId;
            this.Title = title;

            this.ApiNamespace = apiNamespace;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.ImageIconPath = imageIconPath;
            this.BadgeString = badgeString;
            this.Content = content;
            this.IsNew = isNew;
            this.IsUpdated = isUpdated;
            this.IsPreview = isPreview;
            this.Docs = new ObservableCollection<ControlInfoDocLink>();
            this.RelatedControls = new ObservableCollection<string>();
            this.HideSourceCodeAndRelatedControls = hideSourceCodeAndRelatedControls;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string ApiNamespace { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string ImageIconPath { get; private set; }
        public string BadgeString { get; private set; }
        public string Content { get; private set; }
        public bool IsNew { get; private set; }
        public bool IsUpdated { get; private set; }
        public bool IsPreview { get; private set; }
        public bool HideSourceCodeAndRelatedControls { get; private set; }
        public ObservableCollection<ControlInfoDocLink> Docs { get; private set; }
        public ObservableCollection<string> RelatedControls { get; private set; }

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
        public string Title { get; private set; }
        public string Uri { get; private set; }
    }


    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class ControlInfoDataGroup
    {
        public ControlInfoDataGroup(string uniqueId, string title, string subtitle, string imagePath, string imageIconPath, string description, string apiNamespace, bool isSpecialSection)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.ApiNamespace = apiNamespace;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.ImageIconPath = imageIconPath;
            this.Items = new ObservableCollection<ControlInfoDataItem>();
            this.IsSpecialSection = isSpecialSection;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string ImageIconPath { get; private set; }
        public string ApiNamespace { get; private set; } = "";
        public bool IsSpecialSection { get; set; }
        public ObservableCollection<ControlInfoDataItem> Items { get; private set; }

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

            string jsonText = await FileLoader.LoadText("DataModel/ControlInfoData.json");

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            lock (_lock)
            {
                string pageRoot = "AppUIBasics.ControlPages.";
                foreach (JsonValue groupValue in jsonArray)
                {

                    JsonObject groupObject = groupValue.GetObject();

                    var usesCustomNavigationItems = groupObject.ContainsKey("IsSpecialSection") ? groupObject["IsSpecialSection"].GetBoolean() : false;
                    ControlInfoDataGroup group = new ControlInfoDataGroup(groupObject["UniqueId"].GetString(),
                                                                          groupObject["Title"].GetString(),
                                                                          groupObject["ApiNamespace"].GetString(),
                                                                          groupObject["Subtitle"].GetString(),
                                                                          groupObject["ImagePath"].GetString(),
                                                                          groupObject["ImageIconPath"].GetString(),
                                                                          groupObject["Description"].GetString(),
                                                                          usesCustomNavigationItems);

                    foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                    {
                        JsonObject itemObject = itemValue.GetObject();

                        string badgeString = null;

                        bool isNew = itemObject.ContainsKey("IsNew") ? itemObject["IsNew"].GetBoolean() : false;
                        bool isUpdated = itemObject.ContainsKey("IsUpdated") ? itemObject["IsUpdated"].GetBoolean() : false;
                        bool isPreview = itemObject.ContainsKey("IsPreview") ? itemObject["IsPreview"].GetBoolean() : false;

                        if (isNew)
                        {
                            badgeString = "New";
                        }
                        else if (isUpdated)
                        {
                            badgeString = "Updated";
                        }
                        else if (isPreview)
                        {
                            badgeString = "Preview";
                        }

                        var hideSourceCodeAndRelatedControls = itemObject.ContainsKey("HideSourceCodeAndRelatedControls") ? itemObject["HideSourceCodeAndRelatedControls"].GetBoolean() : false;
                        var item = new ControlInfoDataItem(itemObject["UniqueId"].GetString(),
                                                                itemObject["Title"].GetString(),
                                                                itemObject["ApiNamespace"].GetString(),
                                                                itemObject["Subtitle"].GetString(),
                                                                itemObject["ImagePath"].GetString(),
                                                                itemObject["ImageIconPath"].GetString(),
                                                                badgeString,
                                                                itemObject["Description"].GetString(),
                                                                itemObject["Content"].GetString(),
                                                                isNew,
                                                                isUpdated,
                                                                isPreview,
                                                                hideSourceCodeAndRelatedControls);

                        {
                            string pageString = pageRoot + item.UniqueId + "Page";
                            Type pageType = Type.GetType(pageString);
                            item.IncludedInBuild = pageType != null;
                        }

                        if (itemObject.ContainsKey("Docs"))
                        {
                            foreach (JsonValue docValue in itemObject["Docs"].GetArray())
                            {
                                JsonObject docObject = docValue.GetObject();
                                item.Docs.Add(new ControlInfoDocLink(docObject["Title"].GetString(), docObject["Uri"].GetString()));
                            }
                        }
                        if (itemObject.ContainsKey("RelatedControls"))
                        {
                            foreach (JsonValue relatedControlValue in itemObject["RelatedControls"].GetArray())
                            {
                                item.RelatedControls.Add(relatedControlValue.GetString());
                            }
                        }

                        group.Items.Add(item);
                    }
                    if (!Groups.Any(g => g.Title == group.Title))
                    {
                        Groups.Add(group);
                    }
                }
            }
        }
    }
}
