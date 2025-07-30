// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app
// is first launched.

namespace WinUIGallery.Models;

public partial class Root
{
    public ObservableCollection<ControlInfoDataGroup> Groups { get; set; }
}
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Root))]
internal partial class RootContext : JsonSerializerContext
{
}

/// <summary>
/// Generic item data model.
/// </summary>
public partial class ControlInfoDataItem
{
    public string UniqueId { get; set; }
    public string Title { get; set; }
    public string[] BaseClasses { get; set; }
    public string ApiNamespace { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public string BadgeString { get; set; }
    public bool IsNew { get; set; }
    public bool IsUpdated { get; set; }
    public bool IsPreview { get; set; }
    public ObservableCollection<ControlInfoDocLink> Docs { get; set; }

    public bool IncludedInBuild { get; set; }

    public string SourcePath { get; set; }

    public override string ToString()
    {
        return this.Title;
    }
}

public partial class ControlInfoDocLink
{
    public string Title { get; set; }
    public string Uri { get; set; }
}


/// <summary>
/// Generic group data model.
/// </summary>
[WinRT.GeneratedBindableCustomPropertyAttribute]
public partial class ControlInfoDataGroup
{
    public string UniqueId { get; set; }
    public string Title { get; set; }
    public string IconGlyph { get; set; }
    public bool IsSpecialSection { get; set; }
    public string Folder { get; set; }
    public ObservableCollection<ControlInfoDataItem> Items { get; set; }

    public override string ToString()
    {
        return this.Title;
    }
}
