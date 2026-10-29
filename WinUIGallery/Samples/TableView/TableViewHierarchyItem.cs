// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

// A flat row: the tree comes from TableViewSource.ParentBy(Id, ParentId), so the item has
// no children collection, level, or expanded state.
public sealed partial class TableViewHierarchyItem(int id, int? parentId, string name, string kind, string size)
{
    public int Id { get; } = id;
    public int? ParentId { get; } = parentId;
    public string Name { get; } = name;
    public string Kind { get; } = kind;
    public string Size { get; } = size;
    public string AutomationName => $"{Name}, {Kind}" + (Size.Length > 0 ? $", {Size}" : string.Empty);

    public static ObservableCollection<TableViewHierarchyItem> CreateItems() =>
    [
        new(1, null, "Documents", "Folder", ""),
        new(2, 1, "Projects", "Folder", ""),
        new(3, 2, "Roadmap.docx", "Document", "24 KB"),
        new(4, 2, "Budget.xlsx", "Spreadsheet", "18 KB"),
        new(5, 1, "Notes.txt", "Text file", "2 KB"),
        new(6, null, "Pictures", "Folder", ""),
        new(7, 6, "Vacation", "Folder", ""),
        new(8, 7, "Beach.jpg", "Image", "2.4 MB"),
        new(9, 7, "Sunset.jpg", "Image", "3.1 MB"),
        new(10, 6, "Profile.png", "Image", "640 KB"),
        new(11, null, "Music", "Folder", ""),
        new(12, 11, "Playlist.m3u", "Playlist", "1 KB"),
        new(13, null, "Readme.md", "Markdown file", "4 KB"),
    ];
}
