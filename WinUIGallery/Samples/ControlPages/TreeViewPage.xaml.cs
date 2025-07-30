// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TreeViewPage : Page
{
    private ObservableCollection<ExplorerItem> DataSource;

    public TreeViewPage()
    {
        this.InitializeComponent();
        this.DataContext = this;
        DataSource = GetData();

        InitializeSampleTreeView(sampleTreeView);
        InitializeSampleTreeView(sampleTreeView2);
    }

    private void InitializeSampleTreeView(TreeView sampleTreeView)
    {
        TreeViewNode workFolder = new TreeViewNode() { Content = "Work Documents" };
        workFolder.IsExpanded = true;

        workFolder.Children.Add(new TreeViewNode() { Content = "XYZ Functional Spec" });
        workFolder.Children.Add(new TreeViewNode() { Content = "Feature Schedule" });

        TreeViewNode remodelFolder = new TreeViewNode() { Content = "Home Remodel" };
        remodelFolder.IsExpanded = true;

        remodelFolder.Children.Add(new TreeViewNode() { Content = "Contractor Contact Info" });
        remodelFolder.Children.Add(new TreeViewNode() { Content = "Paint Color Scheme" });

        TreeViewNode personalFolder = new TreeViewNode() { Content = "Personal Documents" };
        personalFolder.IsExpanded = true;
        personalFolder.Children.Add(remodelFolder);

        sampleTreeView.RootNodes.Add(workFolder);
        sampleTreeView.RootNodes.Add(personalFolder);
    }

    private ObservableCollection<ExplorerItem> GetData()
    {
        return new ObservableCollection<ExplorerItem>
        {
            new ExplorerItem
            {
                Name = "Documents",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                {
                    new ExplorerItem { Name = "ProjectProposal", Type = ExplorerItem.ExplorerItemType.File },
                    new ExplorerItem { Name = "BudgetReport", Type = ExplorerItem.ExplorerItemType.File }
                }
            },
            new ExplorerItem
            {
                Name = "Projects",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                {
                    new ExplorerItem { Name = "Project Plan", Type = ExplorerItem.ExplorerItemType.File }
                }
            }
        };
    }
}

public class ExplorerItem
{
    public enum ExplorerItemType
    {
        Folder,
        File,
    }

    public string Name { get; set; }
    public ExplorerItemType Type { get; set; }
    public ObservableCollection<ExplorerItem> Children { get; set; } = new ObservableCollection<ExplorerItem>();
}

partial class ExplorerItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate FolderTemplate { get; set; }
    public DataTemplate FileTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        var explorerItem = (ExplorerItem)item;
        return explorerItem.Type == ExplorerItem.ExplorerItemType.Folder ? FolderTemplate : FileTemplate;
    }
}
