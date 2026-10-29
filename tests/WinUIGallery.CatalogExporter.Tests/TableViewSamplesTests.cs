// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

[TestClass]
public sealed class TableViewSamplesTests
{
    private static IndexControl GenerateTableView()
    {
        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        SampleIndex index = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = repoRoot }).Index;
        return index.Controls.Single(control => control.Gallery.UniqueId == "TableView");
    }

    [TestMethod]
    public void ResourceSetupSnippetIsCopyableAndPrecedesExamples()
    {
        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        XDocument page = XDocument.Load(Path.Combine(repoRoot, "WinUIGallery", "Samples", "TableView", "TableViewPage.xaml"));
        XElement panel = page.Root!.Elements().Single(element => element.Name.LocalName == "StackPanel");
        List<XElement> content = panel.Elements().ToList();
        XElement presenter = content.Single(element => element.Name.LocalName == "SampleCodePresenter");

        Assert.IsTrue(content.IndexOf(presenter) < content.FindIndex(element => element.Name.LocalName == "ControlExample"));
        Assert.AreEqual("True", presenter.Attribute("IsCopyButtonVisible")?.Value);
        Assert.AreEqual("XAML", presenter.Attribute("SampleType")?.Value);

        string code = presenter.Elements()
            .Single(element => element.Name.LocalName == "SampleCodePresenter.Code")
            .Elements().Single().Value;
        XDocument snippet = XDocument.Parse(code);
        XElement dictionaries = snippet.Descendants()
            .Single(element => element.Name.LocalName == "ResourceDictionary.MergedDictionaries");

        CollectionAssert.AreEqual(
            new[] { "XamlControlsResources", "TabularControlsResources" },
            dictionaries.Elements().Select(element => element.Name.LocalName).ToArray());
        Assert.AreEqual("using:Microsoft.UI.Xaml.Controls", dictionaries.Elements().First().Name.NamespaceName);
        Assert.AreEqual("using:Microsoft.UI.Xaml.Controls.Tabular", dictionaries.Elements().Last().Name.NamespaceName);
    }

    [TestMethod]
    public void TableViewPublishesEightCapabilityExamples()
    {
        IndexControl tableView = GenerateTableView();

        CollectionAssert.AreEqual(
            new[]
            {
                "A basic TableView",
                "Filtering and grouping rows",
                "Showing hierarchical rows",
                "Customizing cells with data templates",
                "Editing cells and validating changes",
                "Adding context menus to cells, rows, and headers",
                "Updating rows from live data",
                "An advanced TableView",
            },
            tableView.Samples.Select(sample => sample.Header).ToArray());

        CollectionAssert.AreEquivalent(
            new[]
            {
                "BasicTable.txt",
                "FilteringGroupingTable.txt",
                "HierarchyTable.txt",
                "CustomColumnsTable.txt",
                "EditingCellsTable.txt",
                "ContextMenuTable.txt",
                "LiveDataTable.txt",
                "PresentationTable.txt",
            },
            tableView.Samples.Select(sample => sample.Gallery.Snippet).ToArray());

        foreach (IndexSample sample in tableView.Samples)
        {
            Assert.IsNotNull(sample.Xaml);
            Assert.IsTrue(XamlFragment.IsWellFormed(sample.Xaml));
            Assert.IsFalse(TokenFallback.ContainsToken(sample.Xaml));
        }
    }

    [TestMethod]
    public void InteractiveOptionsPublishTheirInitialValues()
    {
        IndexControl tableView = GenerateTableView();
        IndexSample basic = tableView.Samples.Single(sample => sample.Gallery.Snippet == "BasicTable.txt");
        IndexSample editing = tableView.Samples.Single(sample => sample.Gallery.Snippet == "EditingCellsTable.txt");
        IndexSample presentation = tableView.Samples.Single(sample => sample.Gallery.Snippet == "PresentationTable.txt");

        StringAssert.Contains(basic.Xaml, "CanUserSortColumns=\"True\"");
        StringAssert.Contains(basic.Xaml, "SelectionMode=\"Single\"");
        StringAssert.Contains(basic.Xaml, "CanUserResizeColumns=\"True\"");
        StringAssert.Contains(basic.Xaml, "CanResize=\"False\"");
        StringAssert.Contains(basic.Xaml, "CanSort=\"False\"");
        StringAssert.Contains(basic.Xaml, "FrozenEdge=\"Leading\"");
        StringAssert.Contains(basic.Xaml, "SortCycle=\"DescendingAscendingNone\"");
        StringAssert.Contains(editing.Xaml, "IsReadOnly=\"False\"");
        StringAssert.Contains(editing.Xaml, "Header=\"ID\" IsReadOnly=\"True\"");
        StringAssert.Contains(presentation.Xaml, "Density=\"Standard\"");
        StringAssert.Contains(presentation.Xaml, "GridLinesVisibility=\"All\"");
        StringAssert.Contains(presentation.Xaml, "AlternatingRowBackground=\"{ThemeResource SubtleFillColorSecondaryBrush}\"");
        StringAssert.Contains(presentation.Xaml, "RowBackground=\"{ThemeResource CardBackgroundFillColorDefaultBrush}\"");
        StringAssert.Contains(presentation.Xaml, "Text=\"No items\"");
    }

    [TestMethod]
    public void AdvancedSampleProvidesRowBandingOption()
    {
        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        XDocument page = XDocument.Load(Path.Combine(repoRoot, "WinUIGallery", "Samples", "TableView", "TableViewPage.xaml"));
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        XElement toggle = page.Descendants()
            .Single(element => element.Name.LocalName == "ToggleSwitch" &&
                element.Attribute(x + "Name")?.Value == "RowBandingToggle");

        Assert.AreEqual("Row banding", toggle.Attribute("Header")?.Value);
        Assert.AreEqual("True", toggle.Attribute("IsOn")?.Value);
        Assert.AreEqual("TableViewRowBandingToggle", toggle.Attribute("AutomationProperties.AutomationId")?.Value);
        Assert.AreEqual("RowBandingToggle_Toggled", toggle.Attribute("Toggled")?.Value);
    }

    [TestMethod]
    public void RealizedRowsReceiveAccessibleNames()
    {
        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        string pageCode = File.ReadAllText(
            Path.Combine(repoRoot, "WinUIGallery", "Samples", "TableView", "TableViewPage.xaml.cs"));

        StringAssert.Contains(pageCode, "PART_RowsRepeater");
        StringAssert.Contains(pageCode, "rowsRepeater.ElementPrepared += RowsRepeater_ElementPrepared;");
        StringAssert.Contains(pageCode, "AutomationProperties.NameProperty");
        StringAssert.Contains(pageCode, "nameof(TableViewSampleItem.AutomationName)");
    }

    [TestMethod]
    public void FilteringAndGroupingShareOneSource()
    {
        IndexControl tableView = GenerateTableView();
        IndexSample filteringGrouping = tableView.Samples.Single(sample => sample.Gallery.Snippet == "FilteringGroupingTable.txt");

        Assert.IsNotNull(filteringGrouping.Code);
        StringAssert.Contains(filteringGrouping.Xaml, "TextChanged=\"FilterBox_TextChanged\"");
        StringAssert.Contains(filteringGrouping.Xaml, "SelectionChanged=\"GroupByComboBox_SelectionChanged\"");
        StringAssert.Contains(filteringGrouping.Xaml, "Text=\"No items match the filter.\"");
        StringAssert.Contains(filteringGrouping.Code, "TableViewSource.From(TableViewSampleItem.CreateItems(12))");
        StringAssert.Contains(filteringGrouping.Code, "FilteringGroupingTable.ItemsSource = _filteringGroupingSource;");
        StringAssert.Contains(filteringGrouping.Code, "_filteringGroupingSource.Filter(");
        StringAssert.Contains(filteringGrouping.Code, "_filteringGroupingSource.ClearFilter();");
        StringAssert.Contains(filteringGrouping.Code, "_filteringGroupingSource.GroupBy(");
        StringAssert.Contains(filteringGrouping.Code, "_filteringGroupingSource.ClearGroupBy();");
        StringAssert.Contains(filteringGrouping.Code, "FilteringGroupingTable.ExpandAllGroups();");
        StringAssert.Contains(filteringGrouping.Code, "FilteringGroupingTable.CollapseAllGroups();");
    }

    [TestMethod]
    public void HierarchySampleBuildsTreeFromFlatSource()
    {
        IndexControl tableView = GenerateTableView();
        IndexSample hierarchy = tableView.Samples.Single(sample => sample.Gallery.Snippet == "HierarchyTable.txt");

        Assert.IsNotNull(hierarchy.Code);
        StringAssert.Contains(hierarchy.Xaml, "x:Name=\"HierarchyTable\"");
        StringAssert.Contains(hierarchy.Xaml, "TextChanged=\"HierarchyFilterBox_TextChanged\"");
        StringAssert.Contains(hierarchy.Xaml, "Click=\"ExpandAllRows_Click\"");
        StringAssert.Contains(hierarchy.Xaml, "Click=\"CollapseAllRows_Click\"");
        StringAssert.Contains(hierarchy.Code, "TableViewSource.From(TableViewHierarchyItem.CreateItems())");
        StringAssert.Contains(hierarchy.Code, "HierarchyTable.ItemsSource = _hierarchySource;");
        StringAssert.Contains(hierarchy.Code, "_hierarchySource.ParentBy(");
        StringAssert.Contains(hierarchy.Code, "_hierarchySource.Filter(");
        StringAssert.Contains(hierarchy.Code, "_hierarchySource.ClearFilter();");
        StringAssert.Contains(hierarchy.Code, "HierarchyTable.ExpandAllRows();");
        StringAssert.Contains(hierarchy.Code, "HierarchyTable.CollapseAllRows();");
        StringAssert.Contains(hierarchy.Code, "public sealed partial class TableViewHierarchyItem");

        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        string pageCode = File.ReadAllText(
            Path.Combine(repoRoot, "WinUIGallery", "Samples", "TableView", "TableViewPage.xaml.cs"));
        StringAssert.Contains(pageCode, "_hierarchySource.ParentBy(");
    }

    [TestMethod]
    public void ContextMenuSampleCoversEveryMenuLevel()
    {
        IndexControl tableView = GenerateTableView();
        IndexSample contextMenus = tableView.Samples.Single(sample => sample.Gallery.Snippet == "ContextMenuTable.txt");

        Assert.IsNotNull(contextMenus.Code);
        StringAssert.Contains(contextMenus.Xaml, "CellContextFlyout=\"{StaticResource NameCellMenu}\"");
        StringAssert.Contains(contextMenus.Xaml, "HeaderContextFlyout=\"{StaticResource HeaderMenu}\"");
        StringAssert.Contains(contextMenus.Xaml, "<Style TargetType=\"tabular:TableViewRow\">");
        StringAssert.Contains(contextMenus.Xaml, "<Setter Property=\"ContextFlyout\" Value=\"{StaticResource RowMenu}\" />");
        StringAssert.Contains(contextMenus.Xaml, "<tabular:TableView.ContextFlyout>");
        StringAssert.Contains(contextMenus.Xaml, "Opening=\"HeaderMenu_Opening\"");
        StringAssert.Contains(contextMenus.Code, "DataContext: TableViewSampleItem item");
        StringAssert.Contains(contextMenus.Code, "menu.Target as TableViewColumnHeader");
        StringAssert.Contains(contextMenus.Code, "Visibility = Visibility.Collapsed");
    }

    [TestMethod]
    public void LiveDataSampleUsesLiveShapingAndStopsItsTimer()
    {
        IndexControl tableView = GenerateTableView();
        IndexSample liveData = tableView.Samples.Single(sample => sample.Gallery.Snippet == "LiveDataTable.txt");

        Assert.IsNotNull(liveData.Code);
        StringAssert.Contains(liveData.Xaml, "x:Name=\"LiveDataTable\"");
        StringAssert.Contains(liveData.Xaml, "Toggled=\"LiveShapingToggle_Toggled\"");
        StringAssert.Contains(liveData.Code, "_liveSource.IsLiveShaping = true;");
        StringAssert.Contains(liveData.Code, "LiveDataTable.SortByColumn(LiveValueColumn, SortDirection.Descending);");
        StringAssert.Contains(liveData.Code, "_liveSource.Filter(");
        StringAssert.Contains(liveData.Code, "_liveTimer.Stop()");
        StringAssert.Contains(liveData.Code, "public sealed partial class TableViewLiveItem");

        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        string pageCode = File.ReadAllText(
            Path.Combine(repoRoot, "WinUIGallery", "Samples", "TableView", "TableViewPage.xaml.cs"));
        StringAssert.Contains(pageCode, "_liveSource.IsLiveShaping = true;");
        StringAssert.Contains(pageCode, "_liveTimer.Stop();");
    }

    [TestMethod]
    public void CustomTemplatesIncludeRichContentAndSortingPaths()
    {
        IndexControl tableView = GenerateTableView();
        IndexSample custom = tableView.Samples.Single(sample => sample.Gallery.Snippet == "CustomColumnsTable.txt");

        StringAssert.Contains(custom.Xaml, "<PersonPicture");
        StringAssert.Contains(custom.Xaml, "CaptionTextBlockStyle");
        StringAssert.Contains(custom.Xaml, "<ProgressBar");
        StringAssert.Contains(custom.Xaml, "<RatingControl");
        StringAssert.Contains(custom.Xaml, "Header=\"ID\"");
        StringAssert.Contains(custom.Xaml, "Header=\"Status\"");
        StringAssert.Contains(custom.Xaml, "Header=\"Rating\"");
        StringAssert.Contains(custom.Xaml, "SortMemberPath=\"Name\"");
        StringAssert.Contains(custom.Xaml, "SortMemberPath=\"Status\"");
        StringAssert.Contains(custom.Xaml, "SortMemberPath=\"Value\"");
        StringAssert.Contains(custom.Xaml, "SortMemberPath=\"Rating\"");
        StringAssert.Contains(custom.Xaml, "CanUserSortColumns=\"True\"");
        StringAssert.Contains(custom.Xaml, "GridLinesVisibility=\"None\"");
        Assert.IsNotNull(custom.Code);
        StringAssert.Contains(custom.Code, "public ObservableCollection<TableViewSampleItem> CustomColumnItems");
        StringAssert.Contains(custom.Code, "public string Status");
        StringAssert.Contains(custom.Code, "public int Rating");
    }
}