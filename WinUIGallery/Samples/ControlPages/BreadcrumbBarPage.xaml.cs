using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class BreadcrumbBarPage : Page
{
    readonly List<Folder> folders = [
            new Folder { Name = "Home"},
            new Folder { Name = "Folder1" },
            new Folder { Name = "Folder2" },
            new Folder { Name = "Folder3" },
    ];

    public BreadcrumbBarPage()
    {
        InitializeComponent();
        BreadcrumbBar1.ItemsSource = new string[] { "Home", "Documents", "Design", "Northwind", "Images", "Folder1", "Folder2", "Folder3" };

        BreadcrumbBar2.ItemsSource = new ObservableCollection<Folder>(folders);
        BreadcrumbBar2.ItemClicked += BreadcrumbBar2_ItemClicked;
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
        var items = BreadcrumbBar2.ItemsSource as ObservableCollection<Folder>;
        for (int i = items.Count; i < folders.Count; i++)
        {
            items.Add(folders[i]);
        }

        // Announce reset success notifiication.
        UIHelper.AnnounceActionForAccessibility(ResetSampleBtn, "BreadcrumbBar sample reset successful.", "BreadCrumbBarSampleResetNotificationId");
    }
}

public class Folder
{
    public string Name { get; set; }
}
