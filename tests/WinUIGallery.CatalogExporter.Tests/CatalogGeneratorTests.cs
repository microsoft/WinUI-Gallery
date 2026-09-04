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

    private void WriteControlInfoData(string json)
    {
        string dir = Path.Combine(_fixtureRoot, "WinUIGallery", "SampleSupport", "Data");
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "ControlInfoData.json"), json);
    }

    private void WriteSample(string uniqueId, string pageXamlBody, params (string FileName, string Contents)[] extraFiles)
    {
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
        WriteSample("SampleOne", """<Page><controls:ControlExample SampleDefinition="SampleOne\SampleOneBasic.txt" /></Page>""", ("SampleOneBasic.txt", "<Button/>"));
        WriteSample("SampleTwo", "<Page></Page>");

        CatalogManifest manifest = CatalogGenerator.Generate(Options());

        Assert.AreEqual(2, manifest.SampleCount);
        Assert.AreEqual(2, manifest.Samples.Count);

        CatalogSample one = manifest.Samples.Single(s => s.UniqueId == "SampleOne");
        Assert.AreEqual("microsoft/WinUI-Gallery#SampleOne", one.Id);
        Assert.AreEqual("Sample One", one.Title);
        Assert.AreEqual("First sample", one.Summary);
        CollectionAssert.AreEqual(new[] { "alpha" }, one.Tags);
        CollectionAssert.AreEqual(new[] { "microsoft/WinUI-Gallery#SampleTwo" }, one.RelatedSamples);
        Assert.IsNotNull(one.Scenarios);
        Assert.AreEqual(1, one.Scenarios!.Count);
        Assert.AreEqual("SampleOneBasic.txt", one.Scenarios[0].Snippet);
        Assert.AreEqual("Basic", one.Scenarios[0].Name);

        CatalogSample two = manifest.Samples.Single(s => s.UniqueId == "SampleTwo");
        Assert.IsNull(two.Summary, "Optional fields with no source data must be omitted (null), not empty strings.");
        Assert.IsNull(two.Tags);
        Assert.IsNull(two.RelatedSamples);
        Assert.IsNull(two.Scenarios);
        Assert.IsNull(two.Source.Snippets);
    }

    [TestMethod]
    public void Generate_SortsSamplesById()
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

        CatalogManifest manifest = CatalogGenerator.Generate(Options());

        CollectionAssert.AreEqual(
            new[] { "microsoft/WinUI-Gallery#Apple", "microsoft/WinUI-Gallery#Zebra" },
            manifest.Samples.Select(s => s.Id).ToArray());
    }

    [TestMethod]
    public void Serialize_IsDeterministicAcrossRuns()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", "<Page></Page>");
        WriteSample("SampleTwo", "<Page></Page>");

        string first = CatalogGenerator.Serialize(CatalogGenerator.Generate(Options()));
        string second = CatalogGenerator.Serialize(CatalogGenerator.Generate(Options()));

        Assert.AreEqual(first, second);
        StringAssert.EndsWith(first, "\n");
        Assert.IsFalse(first.Contains('\r'), "Serialized manifest must use LF line endings only.");
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
        WriteSample("Snippety", """<Page><controls:ControlExample SampleDefinition="Snippety\Missing.txt" /></Page>""");

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

        CatalogManifest manifest = CatalogGenerator.Generate(Options());

        Assert.AreEqual(1, manifest.SampleCount);
        Assert.AreEqual("Visible", manifest.Samples.Single().UniqueId);
    }

    [TestMethod]
    public void Generate_MergesCatalogAliasesAndRelatedSamples()
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

        CatalogSample one = CatalogGenerator.Generate(Options()).Samples.Single(s => s.UniqueId == "SampleOne");

        CollectionAssert.AreEqual(new[] { "shortcut" }, one.Aliases);
        CollectionAssert.AreEqual(
            new[] { "microsoft/WinUI-Gallery#SampleTwo", "other/repo#thing" },
            one.RelatedSamples);
    }
}
