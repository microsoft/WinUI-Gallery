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
    public void TableViewPublishesFiveCapabilityExamples()
    {
        IndexControl tableView = GenerateTableView();

        CollectionAssert.AreEqual(
            new[]
            {
                "A basic TableView",
                "Filtering and grouping rows",
                "Customizing cells with data templates",
                "Editing cells and validating changes",
                "An advanced TableView",
            },
            tableView.Samples.Select(sample => sample.Header).ToArray());

        CollectionAssert.AreEquivalent(
            new[]
            {
                "BasicTable.txt",
                "FilteringGroupingTable.txt",
                "CustomColumnsTable.txt",
                "EditingCellsTable.txt",
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
        StringAssert.Contains(presentation.Xaml, "Text=\"No items\"");
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