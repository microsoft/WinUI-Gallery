using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TreeViewPage : Page
{
    private ObservableCollection<ExplorerItem> DataSource;

    public TreeViewPage()
    {
        InitializeComponent();
        DataContext = this;
        DataSource = GetData();

        InitializeSampleTreeView(sampleTreeView);
        InitializeSampleTreeView(sampleTreeView2);
    }

    private void InitializeSampleTreeView(TreeView sampleTreeView)
    {
        TreeViewNode workFolder = new()
        {
            Content = "Work Documents",
            IsExpanded = true
        };

        workFolder.Children.Add(new TreeViewNode() { Content = "XYZ Functional Spec" });
        workFolder.Children.Add(new TreeViewNode() { Content = "Feature Schedule" });

        TreeViewNode remodelFolder = new()
        {
            Content = "Home Remodel",
            IsExpanded = true
        };

        remodelFolder.Children.Add(new TreeViewNode() { Content = "Contractor Contact Info" });
        remodelFolder.Children.Add(new TreeViewNode() { Content = "Paint Color Scheme" });

        TreeViewNode personalFolder = new()
        {
            Content = "Personal Documents",
            IsExpanded = true
        };
        personalFolder.Children.Add(remodelFolder);

        sampleTreeView.RootNodes.Add(workFolder);
        sampleTreeView.RootNodes.Add(personalFolder);
    }

    private ObservableCollection<ExplorerItem> GetData() =>
    [
            new() {
                Name = "Documents",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                {
                    new ExplorerItem { Name = "ProjectProposal", Type = ExplorerItem.ExplorerItemType.File },
                    new ExplorerItem { Name = "BudgetReport", Type = ExplorerItem.ExplorerItemType.File }
                }
            },
            new() {
                Name = "Projects",
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children =
                {
                    new ExplorerItem { Name = "Project Plan", Type = ExplorerItem.ExplorerItemType.File }
                }
            }
    ];
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
    public ObservableCollection<ExplorerItem> Children { get; set; } = [];
}

class ExplorerItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate FolderTemplate { get; set; }
    public DataTemplate FileTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        var explorerItem = (ExplorerItem)item;
        return explorerItem.Type == ExplorerItem.ExplorerItemType.Folder ? FolderTemplate : FileTemplate;
    }
}
