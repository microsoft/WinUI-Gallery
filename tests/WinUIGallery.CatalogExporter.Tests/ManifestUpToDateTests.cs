// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Guards against a stale committed index: regenerates it from the real, current repository data
/// and fails if it does not byte-for-byte match what is checked in. Run
/// `dotnet run --project tools/CatalogExporter -- generate` and commit the result if this test
/// fails.
/// </summary>
[TestClass]
public sealed class ManifestUpToDateTests
{
    private static string RepoRoot => CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);

    [TestMethod]
    public void CommittedManifest_MatchesFreshGeneration()
    {
        string repoRoot = RepoRoot;
        string manifestPath = Path.Combine(repoRoot, "catalog", "windows-samples.json");

        Assert.IsTrue(File.Exists(manifestPath), $"{manifestPath} is missing. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");

        string committed = File.ReadAllText(manifestPath).Replace("\r\n", "\n");
        string fresh = CatalogGenerator.Serialize(CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = repoRoot }).Index);

        Assert.AreEqual(
            fresh,
            committed,
            "catalog/windows-samples.json is stale relative to ControlInfoData.json / the Samples folders. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");
    }

    [TestMethod]
    public void RealRepository_SnippetsWithUnpublishableXamlAreTheKnownSet()
    {
        // These snippets are written for the gallery's own code viewer, where a human reads
        // "<Window ...>" as "your existing window". That is not a well-formed XML fragment, so a
        // consumer of the index parses it, fails, and discards it silently. The exporter omits the
        // XAML instead and records a warning; every one of these samples still publishes its C#.
        //
        // The set is pinned so that a newly broken snippet shows up as a failure here rather than
        // quietly disappearing from the index. If you make one of these paste-ready, delete its
        // line. Growing the list should be a deliberate choice, not a default.
        //
        // OtherXamlEasingFunctions.txt is the one entry that is not an elision: it uses a token as
        // an element name, <$(EasingFunction)/>, whose value comes from a ComboBox populated in
        // code-behind. Nothing in the markup can resolve it, so it cannot be published as XAML.
        string[] expected =
        [
            "AppWindow: 'AppWindowSettingMinimumMaximumWidth.txt'",
            "AppWindow: 'AppwindowCompactoverlaypresenter.txt'",
            "AppWindow: 'AppwindowFullscreenpresenter.txt'",
            "AppWindow: 'AppwindowOverlapedpresenter.txt'",
            "AppWindow: 'CenteringAppwindowScreenAvailable.txt'",
            "AppWindow: 'CreatingCustomizingAppwindowWindow.txt'",
            "AppWindow: 'ModalWindowOverlappedpresenterAppwindow.txt'",
            "Binding: 'ConverterBinding.txt'",
            "CustomUserControls: 'CustomUserControlsBasicCustomPasswordBox.txt'",
            "CustomUserControls: 'CustomUserControlsCounterControlIncrementDecrement.txt'",
            "CustomUserControls: 'CustomUserControlsTemperatureConverterUsercontrolExample.txt'",
            "EasingFunction: 'OtherXamlEasingFunctions.txt'",
            "TreeView: 'TreeviewItemtemplateselector.txt'",
        ];

        CatalogGenerationResult result = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot });

        string[] actual = result.Warnings
            .Select(w => $"{w.UniqueId}: '{w.Message.Split('\'')[1]}'")
            .OrderBy(w => w, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(
            expected,
            actual,
            "The set of snippets whose XAML cannot be published changed. Each omitted snippet is a sample a "
                + "consumer cannot paste, so confirm the change is intentional before updating this list.");
    }

    [TestMethod]
    public void RealRepository_EverySampleCarriesPublishableContent()
    {
        // The contract requires a sample to have XAML or code, and its consumer skips any that has
        // neither. Emitting one anyway would inflate the index with entries that vanish on the
        // other side, so the exporter leaves them out and this proves it.
        CatalogGenerationResult result = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot });

        foreach (IndexControl control in result.Index.Controls)
        {
            foreach (IndexSample sample in control.Samples)
            {
                Assert.IsTrue(
                    !string.IsNullOrWhiteSpace(sample.Xaml) || !string.IsNullOrWhiteSpace(sample.Code),
                    $"{control.Id}/{sample.Gallery.Snippet} has neither XAML nor code and must not be published.");

                Assert.AreEqual(
                    sample.Code is null ? null : "csharp",
                    sample.Language,
                    $"{control.Id}/{sample.Gallery.Snippet} must declare its language exactly when it carries code.");
            }
        }
    }

    [TestMethod]
    public void RealRepository_NoSampleUsesInlineControlExampleCode()
    {
        // ControlExample supports two ways of supplying code: a SampleDefinition snippet bundle,
        // and inline <ControlExample.Xaml> / <ControlExample.CSharp> property elements. Only the
        // first is discoverable by the exporter, so an inline example renders correctly in the
        // gallery while silently missing from the catalog. Every sample now uses SampleDefinition,
        // and this test keeps it that way: it fails on the first page that reintroduces the inline
        // form, at authoring time, instead of letting the gap reach consumers of the catalog.
        //
        // The pages are parsed rather than text-searched so that commented-out markup does not
        // count as a real usage.
        string samplesRoot = Path.Combine(RepoRoot, "WinUIGallery", "Samples");
        Assert.IsTrue(Directory.Exists(samplesRoot), $"{samplesRoot} is missing.");

        List<string> offenders = [];
        List<string> unparsable = [];
        int pagesChecked = 0;

        foreach (string page in Directory.EnumerateFiles(samplesRoot, "*.xaml", SearchOption.AllDirectories))
        {
            string relative = Path.GetRelativePath(RepoRoot, page).Replace('\\', '/');
            XDocument document;
            try
            {
                document = XDocument.Load(page);
            }
            catch (System.Xml.XmlException ex)
            {
                unparsable.Add($"{relative} ({ex.Message})");
                continue;
            }

            pagesChecked++;
            if (document.Descendants().Any(e =>
                    e.Name.LocalName is "ControlExample.Xaml" or "ControlExample.CSharp"))
            {
                offenders.Add(relative);
            }
        }

        Assert.AreEqual(0, unparsable.Count, "These sample pages are not well-formed XML: " + string.Join(", ", unparsable));
        Assert.IsTrue(pagesChecked > 0, $"No sample pages were found under {samplesRoot}.");

        Assert.AreEqual(
            0,
            offenders.Count,
            "These pages supply code inline, which the catalog exporter cannot see. Move the code into a "
                + "SampleDefinition snippet bundle (a .txt file with '--- xaml' and optional '--- c#' sections) "
                + "next to the page: " + string.Join(", ", offenders));
    }

    [TestMethod]
    public void RealRepository_HasNoValidationIssues()
    {
        // Re-asserts the same validation Generate() already performs, so a failure here reports
        // clearly as "the real data is invalid" rather than surfacing only via the index diff
        // above.
        SampleIndex index = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot }).Index;
        Assert.IsTrue(index.ControlCount > 0);
        Assert.AreEqual(index.ControlCount, index.Controls.Count);
    }
}
