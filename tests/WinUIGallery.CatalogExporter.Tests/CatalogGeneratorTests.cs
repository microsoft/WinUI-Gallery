// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Exercises CatalogGenerator against small, synthetic ControlInfoData.json + Samples/ fixtures
/// (never the real repository data) so each rule can be tested in isolation.
/// </summary>
[TestClass]
public sealed class CatalogGeneratorTests
{
    private string _fixtureRoot = string.Empty;

    [TestInitialize]
    public void CreateFixtureRoot()
    {
        _fixtureRoot = Path.Combine(Path.GetTempPath(), "CatalogExporterTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_fixtureRoot);
        File.WriteAllText(Path.Combine(_fixtureRoot, "WinUIGallery.slnx"), "<Solution/>");
    }

    [TestCleanup]
    public void DeleteFixtureRoot()
    {
        if (Directory.Exists(_fixtureRoot))
        {
            Directory.Delete(_fixtureRoot, recursive: true);
        }
    }

    private CatalogGenerationOptions Options() => new() { RepoRoot = _fixtureRoot };

    /// <summary>Builds a SampleDefinition bundle in the real "--- section" format.</summary>
    private static string Bundle(string? header = null, string? xaml = null, string? csharp = null)
    {
        List<string> parts = [];
        if (header is not null)
        {
            parts.Add("--- header\n" + header);
        }

        if (xaml is not null)
        {
            parts.Add("--- xaml\n" + xaml);
        }

        if (csharp is not null)
        {
            parts.Add("--- c#\n" + csharp);
        }

        return string.Join('\n', parts) + "\n";
    }

    private void WriteControlInfoData(string json)
    {
        string dir = Path.Combine(_fixtureRoot, "WinUIGallery", "SampleSupport", "Data");
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "ControlInfoData.json"), json);
    }

    /// <summary>
    /// Wraps fixture markup in a page that declares the namespaces every real sample page uses.
    /// The exporter parses pages as XML, so a fragment using the <c>controls:</c> prefix without
    /// declaring it would fail to load for a reason unrelated to what the test is checking.
    /// </summary>
    private static string Page(string inner) =>
        $"""
        <Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:controls="using:WinUIGallery.Controls">{inner}</Page>
        """;

    private void WriteSample(string uniqueId, string pageXamlBody, params (string FileName, string Contents)[] extraFiles)    {
        string folder = Path.Combine(_fixtureRoot, "WinUIGallery", "Samples", uniqueId);
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, uniqueId + "Page.xaml"), pageXamlBody);
        File.WriteAllText(Path.Combine(folder, uniqueId + "Page.xaml.cs"), "// code-behind");
        foreach ((string fileName, string contents) in extraFiles)
        {
            File.WriteAllText(Path.Combine(folder, fileName), contents);
        }
    }

    private static string TwoItemDocument(string extraForFirstItem = "") => $$"""
    {
      "Groups": [
        {
          "UniqueId": "GroupA",
          "Title": "Group A",
          "Items": [
            {
              "UniqueId": "SampleOne",
              "Title": "Sample One",
              "Subtitle": "First sample",
              "Tags": [ "alpha" ],
              "RelatedControls": [ "SampleTwo" ]
              {{extraForFirstItem}}
            },
            {
              "UniqueId": "SampleTwo",
              "Title": "Sample Two"
            }
          ]
        }
      ]
    }
    """;

    [TestMethod]
    public void Generate_ProducesExpectedFieldsAndOmitsEmptyOptionalFields()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\SampleOneBasic.txt" />"""), ("SampleOneBasic.txt", Bundle("A basic button.", "<Button/>")));
        WriteSample("SampleTwo", "<Page></Page>");

        SampleIndex index = CatalogGenerator.Generate(Options()).Index;

        Assert.AreEqual(2, index.ControlCount);
        Assert.AreEqual(2, index.Controls.Count);

        IndexControl one = index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        Assert.AreEqual("sampleone", one.Id);
        Assert.AreEqual("Sample One", one.Name);
        Assert.AreEqual("First sample", one.Description);
        CollectionAssert.AreEqual(new[] { "alpha" }, one.CuratedKeywords);
        CollectionAssert.AreEqual(new[] { "microsoft/WinUI-Gallery#SampleTwo" }, one.Gallery.RelatedSamples);
        Assert.AreEqual(1, one.Samples.Count);
        Assert.AreEqual("SampleOneBasic.txt", one.Samples[0].Gallery.Snippet);
        Assert.AreEqual("Basic", one.Samples[0].Gallery.Name);
        Assert.AreEqual("A basic button.", one.Samples[0].Header);
        Assert.AreEqual("<Button/>", one.Samples[0].Xaml);

        IndexControl two = index.Controls.Single(c => c.Gallery.UniqueId == "SampleTwo");
        Assert.IsNull(two.Description, "Optional fields with no source data must be omitted (null), not empty strings.");
        Assert.IsNull(two.CuratedKeywords);
        Assert.IsNull(two.Gallery.RelatedSamples);
        Assert.AreEqual(0, two.Samples.Count);
    }

    [TestMethod]
    public void Generate_SortsControlsById()
    {
        WriteControlInfoData("""
        {
          "Groups": [
            {
              "UniqueId": "GroupA",
              "Title": "Group A",
              "Items": [
                { "UniqueId": "Zebra", "Title": "Zebra" },
                { "UniqueId": "Apple", "Title": "Apple" }
              ]
            }
          ]
        }
        """);
        WriteSample("Zebra", "<Page></Page>");
        WriteSample("Apple", "<Page></Page>");

        SampleIndex index = CatalogGenerator.Generate(Options()).Index;

        CollectionAssert.AreEqual(
            new[] { "apple", "zebra" },
            index.Controls.Select(c => c.Id).ToArray());
    }

    [TestMethod]
    public void Serialize_IsDeterministicAcrossRuns()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", "<Page></Page>");
        WriteSample("SampleTwo", "<Page></Page>");

        string first = CatalogGenerator.Serialize(CatalogGenerator.Generate(Options()).Index);
        string second = CatalogGenerator.Serialize(CatalogGenerator.Generate(Options()).Index);

        Assert.AreEqual(first, second);
        StringAssert.EndsWith(first, "\n");
        Assert.IsFalse(first.Contains('\r'), "Serialized index must use LF line endings only.");
    }

    [TestMethod]
    public void Generate_ThrowsOnDuplicateUniqueId()
    {
        WriteControlInfoData("""
        {
          "Groups": [
            {
              "UniqueId": "GroupA",
              "Title": "Group A",
              "Items": [
                { "UniqueId": "Dup", "Title": "First" },
                { "UniqueId": "Dup", "Title": "Second" }
              ]
            }
          ]
        }
        """);
        WriteSample("Dup", "<Page></Page>");

        CatalogValidationException ex = Assert.ThrowsException<CatalogValidationException>(() => CatalogGenerator.Generate(Options()));
        Assert.IsTrue(ex.Issues.Any(i => i.Message.Contains("Duplicate", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Generate_ThrowsWhenSampleFolderIsMissing()
    {
        WriteControlInfoData("""
        {
          "Groups": [
            {
              "UniqueId": "GroupA",
              "Title": "Group A",
              "Items": [ { "UniqueId": "Ghost", "Title": "Ghost" } ]
            }
          ]
        }
        """);
        // Intentionally do not create a WinUIGallery/Samples/Ghost folder.

        CatalogValidationException ex = Assert.ThrowsException<CatalogValidationException>(() => CatalogGenerator.Generate(Options()));
        Assert.IsTrue(ex.Issues.Any(i => i.UniqueId == "Ghost" && i.Message.Contains("No sample folder", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Generate_ThrowsWhenPageFileCaseDoesNotMatch()
    {
        WriteControlInfoData("""
        {
          "Groups": [
            {
              "UniqueId": "GroupA",
              "Title": "Group A",
              "Items": [ { "UniqueId": "Casey", "Title": "Casey" } ]
            }
          ]
        }
        """);
        string folder = Path.Combine(_fixtureRoot, "WinUIGallery", "Samples", "Casey");
        Directory.CreateDirectory(folder);
        // Wrong case: "caseyPage.xaml" instead of the expected "CaseyPage.xaml".
        File.WriteAllText(Path.Combine(folder, "caseyPage.xaml"), "<Page></Page>");

        CatalogValidationException ex = Assert.ThrowsException<CatalogValidationException>(() => CatalogGenerator.Generate(Options()));
        Assert.IsTrue(ex.Issues.Any(i => i.UniqueId == "Casey" && i.Message.Contains("case-exact", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Generate_ThrowsOnBrokenRelatedControlsReference()
    {
        WriteControlInfoData("""
        {
          "Groups": [
            {
              "UniqueId": "GroupA",
              "Title": "Group A",
              "Items": [
                { "UniqueId": "Lonely", "Title": "Lonely", "RelatedControls": [ "DoesNotExist" ] }
              ]
            }
          ]
        }
        """);
        WriteSample("Lonely", "<Page></Page>");

        CatalogValidationException ex = Assert.ThrowsException<CatalogValidationException>(() => CatalogGenerator.Generate(Options()));
        Assert.IsTrue(ex.Issues.Any(i => i.Message.Contains("does not resolve", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Generate_ThrowsWhenSampleDefinitionSnippetIsMissing()
    {
        WriteControlInfoData("""
        {
          "Groups": [
            {
              "UniqueId": "GroupA",
              "Title": "Group A",
              "Items": [ { "UniqueId": "Snippety", "Title": "Snippety" } ]
            }
          ]
        }
        """);
        WriteSample("Snippety", Page("""<controls:ControlExample SampleDefinition="Snippety\Missing.txt" />"""));

        CatalogValidationException ex = Assert.ThrowsException<CatalogValidationException>(() => CatalogGenerator.Generate(Options()));
        Assert.IsTrue(ex.Issues.Any(i => i.UniqueId == "Snippety" && i.Message.Contains("SampleDefinition", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Generate_ExcludesItemsMarkedCatalogExclude()
    {
        WriteControlInfoData("""
        {
          "Groups": [
            {
              "UniqueId": "GroupA",
              "Title": "Group A",
              "Items": [
                { "UniqueId": "Hidden", "Title": "Hidden", "Catalog": { "Exclude": true } },
                { "UniqueId": "Visible", "Title": "Visible" }
              ]
            }
          ]
        }
        """);
        // "Hidden" has no on-disk folder at all: Exclude must short-circuit before folder validation.
        WriteSample("Visible", "<Page></Page>");

        SampleIndex index = CatalogGenerator.Generate(Options()).Index;

        Assert.AreEqual(1, index.ControlCount);
        Assert.AreEqual("Visible", index.Controls.Single().Gallery.UniqueId);
    }

    [TestMethod]
    public void Generate_MergesCatalogAliasesIntoCuratedKeywordsAndKeepsRelatedSamples()
    {
        WriteControlInfoData(TwoItemDocument("""
            ,
            "Catalog": {
              "Aliases": [ "shortcut" ],
              "RelatedSamples": [ "other/repo#thing" ]
            }
        """));
        WriteSample("SampleOne", "<Page></Page>");
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");

        // Tags and aliases are both author-written, and the contract has a single slot for those.
        CollectionAssert.AreEqual(new[] { "alpha", "shortcut" }, one.CuratedKeywords);
        CollectionAssert.AreEqual(
            new[] { "microsoft/WinUI-Gallery#SampleTwo", "other/repo#thing" },
            one.Gallery.RelatedSamples);
    }

    /// <summary>
    /// ControlInfoData.json stores UniqueIds in RelatedControls, but the contract's field is
    /// display names and consumers render it verbatim. "SampleTwo" is titled "Sample Two", so a
    /// straight copy would surface an internal id to a reader.
    /// </summary>
    [TestMethod]
    public void Generate_ResolvesRelatedControlsToDisplayNames()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", "<Page></Page>");
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");

        CollectionAssert.AreEqual(new[] { "Sample Two" }, one.RelatedControls);
        // The source-qualified id keeps living in the gallery extension, where ids belong.
        CollectionAssert.AreEqual(new[] { "microsoft/WinUI-Gallery#SampleTwo" }, one.Gallery.RelatedSamples);
    }

    /// <summary>
    /// The contract's "usings" exists so a consumer can make a snippet compile standalone. They
    /// come from the imports the page's code-behind was written against, minus the gallery's own
    /// namespaces — prepending "using WinUIGallery.Helpers;" would break the very build this
    /// field exists to fix.
    /// </summary>
    [TestMethod]
    public void Generate_CollectsUsingsFromCodeBehindAndExcludesGalleryNamespaces()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="Snippet.txt" />"""),
            ("Snippet.txt", Bundle(header: "One", csharp: "var items = new ObservableCollection<string>();")));
        WriteSample("SampleTwo", "<Page></Page>");

        File.WriteAllText(
            Path.Combine(_fixtureRoot, "WinUIGallery", "Samples", "SampleOne", "SampleOnePage.xaml.cs"),
            """
            using System;
            using System.Collections.ObjectModel;
            using static System.Math;
            using Alias = System.Text.StringBuilder;
            using WinUIGallery.Helpers;
            """);

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");

        // Static and alias forms are dropped: the consumer re-emits each entry as "using X;".
        CollectionAssert.AreEqual(new[] { "System", "System.Collections.ObjectModel" }, one.Usings);
    }

    /// <summary>A control with no C# has nothing for a consumer to prepend.</summary>
    [TestMethod]
    public void Generate_OmitsUsingsWhenControlHasNoCode()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="Snippet.txt" />"""),
            ("Snippet.txt", Bundle(header: "One", xaml: "<Button Content=\"Hi\" />")));
        WriteSample("SampleTwo", "<Page></Page>");

        File.WriteAllText(
            Path.Combine(_fixtureRoot, "WinUIGallery", "Samples", "SampleOne", "SampleOnePage.xaml.cs"),
            "using System;");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");

        Assert.IsNull(one.Usings);
    }

    /// <summary>
    /// A prefixed attribute must not hide a prefixed type in its own value. In
    /// x:DataType="local:Contact" the "local" import is the one a reader actually needs, and it
    /// was being missed because the scan consumed the "=" while matching the attribute name.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsPrefixInValueOfPrefixedAttribute()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample(
            "SampleOne",
            """
            <Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:controls="using:WinUIGallery.Controls"
                  xmlns:local="using:WinUIGallery.ControlPages">
              <controls:ControlExample SampleDefinition="Snippet.txt" />
            </Page>
            """,
            ("Snippet.txt", Bundle(header: "One", xaml: """<DataTemplate x:DataType="local:Contact"><TextBlock /></DataTemplate>""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");

        CollectionAssert.Contains(
            one.XmlnsImports ?? one.Samples.Single().XmlnsImports,
            "xmlns:local=\"using:WinUIGallery.ControlPages\"");
    }
}
