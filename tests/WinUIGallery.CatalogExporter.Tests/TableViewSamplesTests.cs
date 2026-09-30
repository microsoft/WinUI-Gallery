// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public void TableViewPublishesFourCapabilityExamples()
    {
        IndexControl tableView = GenerateTableView();

        CollectionAssert.AreEqual(
            new[]
            {
                "A basic TableView",
                "Custom columns and cell content",
                "Editing cells and validating changes",
                "Density, grid lines, and empty state",
            },
            tableView.Samples.Select(sample => sample.Header).ToArray());

        CollectionAssert.AreEquivalent(
            new[]
            {
                "BasicTable.txt",
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
}