// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace WinUIGallery.CatalogExporter.Tests;

[TestClass]
public sealed class ChartSamplesTests
{
    private const string ChartsNamespace = "using:Microsoft.UI.Xaml.Controls.Charts";

    private static IndexControl GenerateChart()
    {
        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        SampleIndex index = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = repoRoot }).Index;
        return index.Controls.Single(control => control.Gallery.UniqueId == "Chart");
    }

    private static XDocument LoadPage()
    {
        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        return XDocument.Load(Path.Combine(repoRoot, "WinUIGallery", "Samples", "Chart", "ChartPage.xaml"));
    }

    private static IndexSample Sample(IndexControl chart, string snippet) =>
        chart.Samples.Single(sample => sample.Gallery.Snippet == snippet);

    [TestMethod]
    public void ResourceSetupSnippetIsCopyableAndPrecedesExamples()
    {
        XElement panel = LoadPage().Root!.Elements().Single(element => element.Name.LocalName == "StackPanel");
        List<XElement> content = panel.Elements().ToList();
        XElement presenter = content.Single(element => element.Name.LocalName == "SampleCodePresenter");

        Assert.IsTrue(content.IndexOf(presenter) < content.FindIndex(element => element.Name.LocalName == "ControlExample"));
        Assert.AreEqual("True", presenter.Attribute("IsCopyButtonVisible")?.Value);
        Assert.AreEqual("XAML", presenter.Attribute("SampleType")?.Value);

        string code = presenter.Elements()
            .Single(element => element.Name.LocalName == "SampleCodePresenter.Code")
            .Elements().Single().Value;
        XElement dictionaries = XDocument.Parse(code).Descendants()
            .Single(element => element.Name.LocalName == "ResourceDictionary.MergedDictionaries");

        CollectionAssert.AreEqual(
            new[] { "XamlControlsResources", "XamlChartsResources" },
            dictionaries.Elements().Select(element => element.Name.LocalName).ToArray());
        Assert.AreEqual("using:Microsoft.UI.Xaml.Controls", dictionaries.Elements().First().Name.NamespaceName);
        Assert.AreEqual(ChartsNamespace, dictionaries.Elements().Last().Name.NamespaceName);
    }

    [TestMethod]
    public void AppMergesChartResourcesAfterControlResources()
    {
        string repoRoot = CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);
        XDocument app = XDocument.Load(Path.Combine(repoRoot, "WinUIGallery", "App.xaml"));
        List<string> dictionaries = app.Descendants()
            .Single(element => element.Name.LocalName == "ResourceDictionary.MergedDictionaries")
            .Elements().Select(element => element.Name.LocalName).ToList();

        int controls = dictionaries.IndexOf("XamlControlsResources");
        int charts = dictionaries.IndexOf("XamlChartsResources");
        Assert.IsTrue(controls >= 0 && charts > controls);
    }

    [TestMethod]
    public void ChartPublishesCapabilityExamples()
    {
        IndexControl chart = GenerateChart();

        CollectionAssert.AreEqual(
            new[]
            {
                "Create a line chart with axes and a legend",
                "Create a bar chart and sort its categories",
                "Configure axes",
                "Combine area, bar, and line series",
                "Customize lines, markers, and data labels",
                "Plot a time series",
                "Update a series data source",
            },
            chart.Samples.Select(sample => sample.Header).ToArray());

        CollectionAssert.AreEqual(
            new[]
            {
                "BasicLineChart.txt",
                "BarChart.txt",
                "AxisOptions.txt",
                "StyledSeries.txt",
                "LinesAndMarkers.txt",
                "TimeSeries.txt",
                "LiveObservableSeries.txt",
            },
            chart.Samples.Select(sample => sample.Gallery.Snippet).ToArray());

        foreach (IndexSample sample in chart.Samples)
        {
            Assert.IsNotNull(sample.Xaml);
            Assert.IsTrue(XamlFragment.IsWellFormed(sample.Xaml));
            Assert.IsFalse(TokenFallback.ContainsToken(sample.Xaml));
            Assert.IsNotNull(sample.Code);
            Assert.IsFalse(TokenFallback.ContainsToken(sample.Code));
        }
    }

    [TestMethod]
    public void InteractiveOptionsPublishTheirInitialValues()
    {
        IndexControl chart = GenerateChart();
        string basic = Sample(chart, "BasicLineChart.txt").Xaml!;
        string bar = Sample(chart, "BarChart.txt").Xaml!;
        string axes = Sample(chart, "AxisOptions.txt").Xaml!;
        string styled = Sample(chart, "StyledSeries.txt").Xaml!;
        string markers = Sample(chart, "LinesAndMarkers.txt").Xaml!;
        string timeSeries = Sample(chart, "TimeSeries.txt").Code!;

        StringAssert.Contains(basic, "ShowLegend=\"True\"");
        StringAssert.Contains(bar, "Orientation=\"Vertical\"");
        StringAssert.Contains(bar, "SortKey=\"Index\"");
        StringAssert.Contains(bar, "SortOrder=\"Ascending\"");
        StringAssert.Contains(axes, "GridLines=\"Major\"");
        StringAssert.Contains(axes, "IsVisible=\"True\"");
        StringAssert.Contains(axes, "ShowTickLabels=\"True\"");
        StringAssert.Contains(axes, "ShowTickMarks=\"True\"");
        Assert.AreEqual(3, styled.Split("IsVisible=\"True\"").Length - 1);
        StringAssert.Contains(markers, "StrokeDashStyle=\"Solid\"");
        StringAssert.Contains(markers, "StrokeThickness=\"2\"");
        StringAssert.Contains(markers, "MarkerShape=\"Circle\"");
        StringAssert.Contains(markers, "ShowDataMarkers=\"True\"");
        StringAssert.Contains(markers, "ShowDataLabels=\"True\"");
        StringAssert.Contains(timeSeries, "IntervalType = DateTimeIntervalType.Week,");
        StringAssert.Contains(timeSeries, "LabelFormat = \"month day\",");
    }

    [TestMethod]
    public void SamplesCoverTheChartApiSurface()
    {
        IndexControl chart = GenerateChart();
        string all = string.Join('\n', chart.Samples.Select(sample => sample.Xaml + '\n' + sample.Code));

        string[] members =
        [
            "<charts:LineSeries", "<charts:AreaSeries", "<charts:BarSeries",
            "<charts:CategoryAxis", "<charts:LinearAxis", "DateTimeAxis",
            "ShowLegend=", "LegendTitle=", "IsVisible=", "Orientation=", "SortKey=", "SortOrder=",
            "Minimum=", "Maximum=", "Spacing=", "GridLines=", "ShowTickLabels=", "ShowTickMarks=",
            "AxisLineBrush=", "TickBrush=", "GridLineMajorBrush=", "GridLineMinorBrush=",
            "Stroke=", "StrokeThickness=", "StrokeDashStyle=", "Fill",
            "ShowDataMarkers=", "MarkerShape=", "DataMarkerBrush=", "ShowDataLabels=", "DataLabelBrush",
            "DataLabelOverrides[", "DataMarkerOverrides[", "IntervalType =", "LabelFormat =",
            "ItemsSource=\"{x:Bind ResponseTimes, Mode=OneWay}\"", "ResponseTimes = [",
        ];

        foreach (string member in members)
        {
            StringAssert.Contains(all, member, $"No sample demonstrates {member}");
        }
    }

    [TestMethod]
    public void EveryBarSeriesSetsOrientation()
    {
        // BarSeries.Orientation defaults to Horizontal. A horizontal bar series that shares a
        // category X axis leaves the chart blank and also stops other charts on the page from
        // drawing, so every sample states the orientation it wants.
        List<XElement> pageBars = LoadPage().Descendants()
            .Where(element => element.Name.NamespaceName == ChartsNamespace && element.Name.LocalName == "BarSeries")
            .ToList();

        Assert.IsTrue(pageBars.Count > 0);
        foreach (XElement bar in pageBars)
        {
            Assert.IsNotNull(bar.Attribute("Orientation"), $"BarSeries '{bar.Attribute("Title")?.Value}' does not set Orientation.");
        }

        foreach (IndexSample sample in GenerateChart().Samples.Where(sample => sample.Xaml!.Contains("<charts:BarSeries", StringComparison.Ordinal)))
        {
            int bars = sample.Xaml!.Split("<charts:BarSeries").Length - 1;
            int orientations = sample.Xaml.Split("Orientation=\"").Length - 1;
            Assert.AreEqual(bars, orientations, $"{sample.Gallery.Snippet}: every BarSeries must set Orientation.");
        }
    }

    [TestMethod]
    public void ChartPropertiesDoNotUseThemeResourceMarkup()
    {
        // Chart objects reject {ThemeResource} on their own properties with a XamlParseException
        // ("Failed to assign to property"), and a nested theme brush does not follow a later theme
        // change. The samples rely on the theme-aware default brushes or on fixed colors instead.
        List<string> offenders = LoadPage().Descendants()
            .Where(element => element.Name.NamespaceName == ChartsNamespace)
            .SelectMany(element => element.Attributes())
            .Where(attribute => attribute.Value.Contains("{ThemeResource", StringComparison.Ordinal))
            .Select(attribute => $"{attribute.Parent!.Name.LocalName}.{attribute.Name.LocalName}")
            .ToList();

        Assert.AreEqual(0, offenders.Count, string.Join(", ", offenders));

        foreach (IndexSample sample in GenerateChart().Samples)
        {
            XElement fragment = XElement.Parse($"<Root xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:charts=\"{ChartsNamespace}\">{sample.Xaml}</Root>");
            List<string> snippetOffenders = fragment.DescendantsAndSelf()
                .Where(element => element.Name.NamespaceName == ChartsNamespace)
                .SelectMany(element => element.Attributes())
                .Where(attribute => attribute.Value.Contains("{ThemeResource", StringComparison.Ordinal))
                .Select(attribute => $"{attribute.Parent!.Name.LocalName}.{attribute.Name.LocalName}")
                .ToList();

            Assert.AreEqual(0, snippetOffenders.Count, $"{sample.Gallery.Snippet}: {string.Join(", ", snippetOffenders)}");
        }
    }
}