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
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
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
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
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
              <controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />
            </Page>
            """,
            ("Snippet.txt", Bundle(header: "One", xaml: """<DataTemplate x:DataType="local:Contact"><TextBlock /></DataTemplate>""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");

        CollectionAssert.Contains(
            one.XmlnsImports ?? one.Samples.Single().XmlnsImports,
            "xmlns:local=\"using:WinUIGallery.ControlPages\"");
    }

    /// <summary>
    /// ControlExample loads a bundle as "Samples/&lt;SampleDefinition&gt;", so the directory half of
    /// the value has to name the sample's own folder. Validating only the file name would let a
    /// wrong-folder typo through here and leave it to appear as an empty code viewer at runtime.
    /// </summary>
    [TestMethod]
    public void Generate_RejectsSampleDefinitionNamingTheWrongFolder()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="WrongFolder\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(header: "One", xaml: "<Button />")));
        WriteSample("SampleTwo", "<Page></Page>");

        CatalogValidationException error = Assert.ThrowsException<CatalogValidationException>(() => CatalogGenerator.Generate(Options()));

        StringAssert.Contains(error.Message, "WrongFolder\\Snippet.txt");
    }

    /// <summary>
    /// A bare file name is rejected for the same reason: the gallery would resolve it as
    /// "Samples/Snippet.txt" and find nothing there.
    /// </summary>
    [TestMethod]
    public void Generate_RejectsSampleDefinitionWithNoFolder()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="Snippet.txt" />"""),
            ("Snippet.txt", Bundle(header: "One", xaml: "<Button />")));
        WriteSample("SampleTwo", "<Page></Page>");

        Assert.ThrowsException<CatalogValidationException>(() => CatalogGenerator.Generate(Options()));
    }

    /// <summary>
    /// A snippet that declares a prefix on its own root needs nothing from the page, so it is
    /// published rather than treated as unresolvable.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhoseFragmentDeclaresItsOwnPrefix()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(header: "One", xaml: """<StackPanel xmlns:sys="using:System"><sys:String>Hi</sys:String></StackPanel>""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");

        Assert.IsNotNull(one.Samples.Single().Xaml);
        Assert.IsNull(one.Samples.Single().Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// The value of such a declaration is a namespace URI, not markup. Reading "clr-namespace:X" as
    /// a type reference finds a prefix nothing declares and withholds the fragment over the very
    /// line that binds it.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhoseOwnDeclarationNamesANonUsingScheme()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<StackPanel xmlns:sys="clr-namespace:System;assembly=mscorlib"><sys:String>Hi</sys:String></StackPanel>""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A prefix neither the page nor the fragment declares cannot be published as an import, so the
    /// XAML is omitted rather than shipped in a state that will not compile on arrival.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlBindingAPrefixNothingDeclares()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(header: "One", xaml: "<mystery:Thing />", csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "mystery" }, sample.Gallery.XamlOmittedUnboundPrefixes);
        Assert.IsNotNull(sample.Code);
    }

    /// <summary>
    /// When the type behind an undeclared prefix is defined in the snippet's own C#, the reader is
    /// holding both halves of the scenario and only the namespace line is missing. It is
    /// synthesized from that code's namespace rather than the XAML being withheld.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhosePrefixIsSatisfiedByItsOwnCode()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:ExplorerItem"><TextBlock Text="{x:Bind Name}" /></DataTemplate>""",
                csharp: "namespace Contoso.Sample;\n\npublic class ExplorerItem\n{\n    public string Name { get; set; }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
        CollectionAssert.AreEqual(
            new[] { "xmlns:local=\"using:Contoso.Sample\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// A "file" type reaches no further than the source file declaring it, and the markup compiles
    /// into generated code of its own. No import can put it within reach of "local:", so the XAML
    /// is withheld rather than published with a line that cannot work.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhosePrefixIsOnlySatisfiedByAFileLocalType()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:ExplorerItem"><TextBlock Text="{x:Bind Name}" /></DataTemplate>""",
                csharp: "namespace Contoso.Sample;\n\nfile class ExplorerItem\n{\n    public string Name { get; set; }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample withheld = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(withheld.Xaml);
        CollectionAssert.AreEqual(new[] { "local" }, withheld.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// Snippets that declare their types outside any namespace are the common case in this
    /// repository, and they already write "YourNamespace" as the stand-in a reader is expected to
    /// replace. The synthesized import uses the same placeholder so both halves agree.
    /// </summary>
    [TestMethod]
    public void Generate_UsesPlaceholderNamespaceWhenTheCodeDeclaresNone()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<local:MenuItemTemplateSelector x:Key="selector"><local:MenuItemTemplateSelector.ItemTemplate><DataTemplate x:DataType="local:Category" /></local:MenuItemTemplateSelector.ItemTemplate></local:MenuItemTemplateSelector>""",
                csharp: "public class Category { }\n\nclass MenuItemTemplateSelector : DataTemplateSelector { }")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
        CollectionAssert.AreEqual(
            new[] { "xmlns:local=\"using:YourNamespace\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// The code has to declare the type, not merely mention it. ItemsRepeater's layout sample
    /// describes its custom layout class in a comment while defining a different one, and reading
    /// that as a declaration would publish a fragment referencing a type the reader never receives.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhoseTypeOnlyAppearsInACodeComment()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: "<common:VariedImageSizeLayout Width=\"200\" />",
                csharp: "// See the class VariedImageSizeLayout in the repo.\npublic class Recipe { }")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "common" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A fragment whose prefixes are only partly accounted for is still incomplete, so it is
    /// withheld — and only the prefixes that actually caused the omission are reported.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhenOnlySomePrefixesAreSatisfiedByCode()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<common:VariedImageSizeLayout><DataTemplate x:DataType="l:Recipe" /></common:VariedImageSizeLayout>""",
                csharp: "public class Recipe { }")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "common" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// The type behind the prefix can be declared in any of C#'s type forms, including the record
    /// ones. "record struct" is the form worth pinning: the keyword alternation has to prefer it
    /// over the bare "record", or the modifier is read as the type name and the real one is never
    /// seen — which would withhold the fragment for a type the snippet plainly hands over.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhosePrefixIsSatisfiedByARecordStruct()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:Point"><TextBlock Text="{x:Bind X}" /></DataTemplate>""",
                csharp: "namespace Contoso.Sample;\n\npublic readonly record struct Point(int X, int Y);")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
        CollectionAssert.AreEqual(
            new[] { "xmlns:local=\"using:Contoso.Sample\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// A snippet may declare types in more than one namespace, and only one of them can hold the
    /// type the XAML names. Publishing an import for the other would bind the prefix to a namespace
    /// that does not contain it — markup that looks complete and does not compile — so the fragment
    /// is withheld instead.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhoseTypesSpanTwoNamespaces()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<local:Chooser><DataTemplate x:DataType="local:Item" /></local:Chooser>""",
                csharp: "namespace A\n{\n    public class Chooser { }\n}\n\nnamespace B\n{\n    public class Item { }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml, "No single import covers types split across two namespaces.");
        CollectionAssert.AreEqual(new[] { "local" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// The import names the namespace the referenced type is actually in, not whichever one the
    /// snippet happens to open with.
    /// </summary>
    [TestMethod]
    public void Generate_ImportsTheNamespaceHoldingTheReferencedType()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:Foo" />""",
                csharp: "namespace A\n{\n    public class Other { }\n}\n\nnamespace B\n{\n    public class Foo { }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        CollectionAssert.AreEqual(
            new[] { "xmlns:local=\"using:B\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// A quote in text content is content, not the start of an attribute value. Pairing quotes
    /// across the whole fragment let a stray one — the "x = " below — pair with the opening quote of
    /// the next real attribute and swallow "local:Card" with it. Nothing else reads that value:
    /// it is not a name, and it holds no brace, so the reference vanished and the fragment was
    /// published with no import for local.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsATypeReferenceFollowingAQuoteInTextContent()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<StackPanel><TextBlock>x = " y</TextBlock><Style TargetType="local:Card" /></StackPanel>""",
                csharp: "namespace A\n{\n    public class Card { }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        CollectionAssert.AreEqual(
            new[] { "xmlns:local=\"using:A\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// One value may name several types, and an import has to cover all of them. Reading only the
    /// name the value opens with resolved local against the namespace holding Key alone, publishing
    /// an import that does not contain Value — markup that looks complete and does not compile.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhoseValueNamesTypesInTwoNamespaces()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<local:Map x:TypeArguments="local:Key, local:Value" />""",
                csharp: "namespace A\n{\n    public class Map { }\n\n    public class Key { }\n}\n\nnamespace B\n{\n    public class Value { }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml, "No single import covers Key and Value.");
        CollectionAssert.AreEqual(new[] { "local" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// XML permits either quote style, so a prefix named in a single-quoted attribute value binds
    /// exactly as one in a double-quoted value does. Failing to see it would publish the fragment
    /// with no import for it — and the omission would be invisible to the unresolved-prefix check,
    /// which reads the same references.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsAPrefixInASingleQuotedAttributeValue()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: "<DataTemplate x:DataType = 'local:Widget' />",
                csharp: "namespace Contoso.Sample;\n\npublic class Widget { }")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        CollectionAssert.AreEqual(
            new[] { "xmlns:local=\"using:Contoso.Sample\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// The same reference, single-quoted and with nothing declaring it, must still cost the sample
    /// its XAML — proving the quote style is what changed and not the rule.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlBindingAnUndeclaredPrefixInASingleQuotedValue()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: "<DataTemplate x:DataType='mystery:Thing' />",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "mystery" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// The page's own declaration wins when it has one: it names the namespace the gallery actually
    /// compiles against, which is more specific than anything derived from a snippet.
    /// </summary>
    [TestMethod]
    public void Generate_PrefersThePageDeclarationOverTheSnippetsCode()
    {
        WriteControlInfoData(TwoItemDocument());
        string page =
            """
            <Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:local="using:WinUIGallery.Samples.SampleOne"
                  xmlns:controls="using:WinUIGallery.Controls"><controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" /></Page>
            """;
        WriteSample("SampleOne", page,
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:ExplorerItem" />""",
                csharp: "namespace Contoso.Sample;\n\npublic class ExplorerItem { }")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        CollectionAssert.AreEqual(
            new[] { "xmlns:local=\"using:WinUIGallery.Samples.SampleOne\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// A markup extension resolves prefixes throughout its body, so a type named as an argument
    /// needs an import just as the extension itself does. Reading only the extension's own name
    /// would publish "{x:Bind local:Thing.Value}" with nothing declaring "local" — and because the
    /// unresolved-prefix check reads the same references, the gap would not be reported either.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsAPrefixUsedInsideAMarkupExtension()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TimePicker SelectedTime="{x:Bind clock:SampleTime.Default}" />""",
                csharp: "namespace Contoso.Sample;\n\npublic class SampleTime { }")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        CollectionAssert.AreEqual(
            new[] { "xmlns:clock=\"using:Contoso.Sample\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// The same reference with nothing declaring it has to cost the sample its XAML. Before the
    /// body was swept, such a fragment was published as though it bound nothing.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlBindingAnUndeclaredPrefixInsideAMarkupExtension()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TimePicker SelectedTime="{x:Bind sys:DateTime.Now.TimeOfDay}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "sys" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A nested extension's arguments are inside the outer extension's braces, so sweeping only as
    /// far as the first closing brace would step over them.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsAPrefixInsideANestedMarkupExtension()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Source={StaticResource conv:Upper}, Path=Name}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "conv" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A colon inside a format string is punctuation, not a prefix. Reading one would withhold
    /// perfectly good markup over a namespace nobody asked for.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhoseOnlyColonsAreInAFormatString()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Elapsed, StringFormat='{}{0:hh:mm}'}" />""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A type declared behind a conditional is not a type the reader is handed, so the fragment
    /// naming it keeps being withheld rather than gaining an import for something that may never
    /// compile.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhosePrefixIsOnlySatisfiedInsideAConditionalRegion()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:Widget" />""",
                csharp: "namespace Contoso.Sample;\n\n#if EXPERIMENTAL\npublic class Widget { }\n#endif")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "local" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// "using:Contoso.Sample" does not bring a nested type into scope — the XAML would have to name
    /// "local:Container.Item" — so a snippet that only nests the type has not handed the reader
    /// what its markup asks for.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhoseTypeIsOnlyDeclaredNestedInsideAnotherType()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:Item" />""",
                csharp: "namespace Contoso.Sample;\n\npublic class Container\n{\n    public class Item { }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "local" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// Two namespaces declaring the same simple name leave "local:Item" pointing at neither in
    /// particular, so the fragment is withheld rather than published with a guessed import.
    /// </summary>
    [TestMethod]
    public void Generate_OmitsXamlWhoseTypeNameIsDeclaredInTwoNamespaces()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<DataTemplate x:DataType="local:Item" />""",
                csharp: "namespace Contoso.Models\n{\n    public class Item { }\n}\n\nnamespace Contoso.View\n{\n    public class Item { }\n}")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "local" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A quoted extension argument is literal text, so the colon in a date format inside one is
    /// punctuation. Reading "HH" as a prefix would withhold markup that binds perfectly well.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhoseFormatStringHasASpaceBeforeItsColons()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Stamp, StringFormat='{}{0:yyyy-MM-dd HH:mm}'}" />""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A quoted argument is not literal, though. Quoting ends the argument; the text inside still
    /// reaches the target property's converter, and which converters resolve names is not knowable
    /// from the markup. "HH:mm" is shaped exactly like a type reference, so it is read as one and the
    /// fragment is withheld — the safe half of an unavoidable trade, since the other half publishes
    /// "Path='(attached:Badge.Count)'" with no import for attached. Authors who mean text literally
    /// have "{}" to say so.
    /// </summary>
    [TestMethod]
    public void Generate_WithholdsXamlWhoseQuotedExtensionArgumentIsShapedLikeAQualifiedName()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Elapsed, StringFormat='HH:mm'}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "HH" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// The escape still works, and it is what keeps a quoted format string publishable.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhoseQuotedExtensionArgumentIsAnEscapedLiteral()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Elapsed, StringFormat='{}{0:HH:mm}'}" />""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A quoted binding path is the case that makes the trade worth taking: the reference is real,
    /// and stepping over the quotes published the markup without the import it needs.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsAPrefixInsideAQuotedExtensionArgument()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Path='(attached:Badge.Count)'}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "attached" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// Reading quoted arguments must not reach past them: a real reference after one is still the
    /// difference between publishing an import and publishing markup that cannot bind.
    /// </summary>
    [TestMethod]
    public void Generate_StillDetectsAPrefixFollowingAQuotedExtensionArgument()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Stamp, StringFormat='{}{0:HH:mm}', Converter={StaticResource conv:Upper}}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "conv" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A named argument's value resolves prefixes just as a positional one does, and "Type=" is the
    /// common way to write a QName-valued argument. Reading only what follows whitespace found the
    /// extension's own prefix and nothing else.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsAPrefixInANamedMarkupExtensionArgument()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Tag="{local:Lookup Type=models:Customer}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "local", "models" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// Whitespace after the "=" is the same argument written differently, so it has to read the
    /// same way. The two forms disagreeing was how the gap showed itself.
    /// </summary>
    [TestMethod]
    public void Generate_ReadsANamedArgumentTheSameWithOrWithoutSpaceAfterTheEquals()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Tag="{local:Lookup Type= models:Customer}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "local", "models" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// A named argument whose prefix the sample's own code satisfies gains its import like any
    /// other reference, rather than being dropped on the way in.
    /// </summary>
    [TestMethod]
    public void Generate_ImportsANamespaceForANamedArgumentSatisfiedByItsOwnCode()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Tag="{StaticResource Key=models:Customer}" />""",
                csharp: "namespace Contoso.Models;\n\npublic class Customer { }")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexControl one = CatalogGenerator.Generate(Options()).Index.Controls.Single(c => c.Gallery.UniqueId == "SampleOne");
        IndexSample sample = one.Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        CollectionAssert.AreEqual(
            new[] { "xmlns:models=\"using:Contoso.Models\"" },
            sample.XmlnsImports ?? one.XmlnsImports);
    }

    /// <summary>
    /// A binding path carries its references inside punctuation, so an attached property named in
    /// one still needs the namespace holding it.
    /// </summary>
    [TestMethod]
    public void Generate_DetectsAPrefixInsideAParenthesizedBindingPath()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{Binding Path=(attached:Badge.Count)}" />""",
                csharp: "int x = 1;")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNull(sample.Xaml);
        CollectionAssert.AreEqual(new[] { "attached" }, sample.Gallery.XamlOmittedUnboundPrefixes);
    }

    /// <summary>
    /// "{}" is XAML's escape for a value that merely begins with a brace. What follows is text, so
    /// a colon in it is punctuation and costs the fragment nothing.
    /// </summary>
    [TestMethod]
    public void Generate_KeepsXamlWhoseValueIsAnEscapedLiteralBrace()
    {
        WriteControlInfoData(TwoItemDocument());
        WriteSample("SampleOne", Page("""<controls:ControlExample SampleDefinition="SampleOne\Snippet.txt" />"""),
            ("Snippet.txt", Bundle(
                header: "One",
                xaml: """<TextBlock Text="{}{StaticResource not:AnExtension}" />""")));
        WriteSample("SampleTwo", "<Page></Page>");

        IndexSample sample = CatalogGenerator.Generate(Options()).Index.Controls
            .Single(c => c.Gallery.UniqueId == "SampleOne").Samples.Single();

        Assert.IsNotNull(sample.Xaml);
        Assert.IsNull(sample.Gallery.XamlOmittedUnboundPrefixes);
    }
}
