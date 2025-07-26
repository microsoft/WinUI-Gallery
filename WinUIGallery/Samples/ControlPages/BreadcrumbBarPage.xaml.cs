// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class BreadcrumbBarPage : Page
{
    // We use a separate _defaultFolders list to preserve the original folder structure,
    // because BreadcrumbBar2.ItemsSource is bound directly to the Folders collection.
    // When a breadcrumb item is clicked, items are removed directly from Folders,
    // which means we lose access to the full original list.
    private readonly List<Folder> _defaultFolders = new()
    {
        new Folder { Name = "Home" },
        new Folder { Name = "Folder1" },
        new Folder { Name = "Folder2" },
        new Folder { Name = "Folder3" },
    };

    public ObservableCollection<Folder> Folders { get; } = new();

    public readonly string[] FoldersString = new string[] { "Home", "Documents", "Design", "Northwind", "Images", "Folder1", "Folder2", "Folder3" };
    public BreadcrumbBarPage()
    {
        this.InitializeComponent();

        BreadcrumbBar2.ItemClicked += BreadcrumbBar2_ItemClicked;
        Folders.Clear();
        foreach (var folder in _defaultFolders)
            Folders.Add(folder);
    }

    private void BreadcrumbBar2_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var items = BreadcrumbBar2.ItemsSource as ObservableCollection<Folder>;
        for (int i = items.Count - 1; i >= args.Index + 1; i--)
        {
            items.RemoveAt(i);
        }
    }

    private void ResetSampleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // To restore the BreadcrumbBar to its initial state, we compare Folders (the live collection)
        // with _defaultFolders (the original state), and add back any missing items.
        // This ensures reset works even after user navigation modifies the ItemsSource.
        var items = BreadcrumbBar2.ItemsSource as ObservableCollection<Folder>;
        foreach (var folder in _defaultFolders)
        {
            if (!items.Contains(folder))
            {
                items.Add(folder);
            }
        }


        // Announce reset success notifiication.
        UIHelper.AnnounceActionForAccessibility(ResetSampleBtn, "BreadcrumbBar sample reset successful.", "BreadCrumbBarSampleResetNotificationId");
    }
}

public class Folder
{
    public string Name { get; set; }
}
