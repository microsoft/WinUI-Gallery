//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using WASDK = Microsoft.WindowsAppSDK;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app
// is first launched.

namespace WinUIGallery.Models;

public class Root
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
public class ControlInfoDataItem
{
    public string UniqueId { get; set; }
    public string Title { get; set; }
    public string[] BaseClasses { get; set; }
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
    public ObservableCollection<ControlInfoDocLink> Docs { get; set; }
    public ObservableCollection<string> RelatedControls { get; set; }

    public bool IncludedInBuild { get; set; }

    public string SourcePath { get; set; }

    public override string ToString() => Title;
}

public class ControlInfoDocLink
{
    public ControlInfoDocLink(string title, string uri)
    {
        Title = title;
        Uri = uri.Replace("X.Y", string.Format("{0}.{1}", WASDK.Release.Major, WASDK.Release.Minor));
    }
    public string Title { get; set; }
    public string Uri { get; set; }
}


/// <summary>
/// Generic group data model.
/// </summary>
public class ControlInfoDataGroup
{
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

    public override string ToString() => Title;
}
