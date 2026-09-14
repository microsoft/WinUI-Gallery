// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Guards against stale committed catalog files: regenerates both artifacts from the real,
/// current repository data and fails if either does not byte-for-byte match what is checked in.
/// Run `dotnet run --project tools/CatalogExporter -- generate` and commit the result if this
/// test fails.
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
        string fresh = CatalogGenerator.Serialize(CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = repoRoot }).Manifest);

        Assert.AreEqual(
            fresh,
            committed,
            "catalog/windows-samples.json is stale relative to ControlInfoData.json / the Samples folders. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");
    }

    [TestMethod]
    public void CommittedCodeFile_MatchesFreshGeneration()
    {
        string repoRoot = RepoRoot;
        string codePath = Path.Combine(repoRoot, "catalog", "windows-samples.code.json");

        Assert.IsTrue(File.Exists(codePath), $"{codePath} is missing. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");

        string committed = File.ReadAllText(codePath).Replace("\r\n", "\n");
        string fresh = CatalogGenerator.Serialize(CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = repoRoot }).Code);

        Assert.AreEqual(
            fresh,
            committed,
            "catalog/windows-samples.code.json is stale relative to the Samples folders. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");
    }

    [TestMethod]
    public void RealRepository_CodeFileResolvesToManifestScenariosWithoutOrphans()
    {
        // The two files are only useful together: every code entry must resolve to a scenario in
        // the manifest, ids must be unique, and the count must match. This is the invariant that
        // makes splitting them safe. Note that the reverse is not required - see
        // RealRepository_CodelessScenariosAreTheKnownSet for scenarios that carry no code.
        CatalogGenerationResult result = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot });

        HashSet<string> manifestIds = result.Manifest.Samples
            .SelectMany(s => s.Scenarios ?? [])
            .Select(s => s.Id)
            .ToHashSet(StringComparer.Ordinal);

        string[] orphans = result.Code.Scenarios
            .Select(s => s.Id)
            .Where(id => !manifestIds.Contains(id))
            .ToArray();

        Assert.AreEqual(0, orphans.Length, "Code entries with no matching manifest scenario: " + string.Join(", ", orphans));
        Assert.AreEqual(result.Code.Scenarios.Count, result.Code.Scenarios.Select(s => s.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.AreEqual(result.Code.ScenarioCount, result.Code.Scenarios.Count);
    }

    [TestMethod]
    public void RealRepository_CodelessScenariosAreTheKnownSet()
    {
        // Pins the scenarios that appear in the manifest but contribute no code, so the gap stays
        // visible and shrinking it is a deliberate, reviewed change rather than a silent drift.
        //
        //   ContentIsland/BasicContentIslandContent - the page sets SourceCodeVisibility="Collapsed",
        //     so the gallery deliberately shows no code. Expected to stay code-less.
        //   SystemBackdropElement/SystembackdropelementSample - the gallery does show code here, but
        //     the page swaps ControlExample.XamlSource at runtime between three backdrop variants
        //     (Acrylic, Mica, MicaAlt) as the user changes a ComboBox, so no single static snippet
        //     represents it. Publishing one would also break the page: SampleCodePresenter prefers
        //     Code over CodeSourceFile, so a snippet bundle would pin the code pane to one variant.
        //     Expected to stay code-less unless the catalog grows a way to express alternatives.
        string[] expected =
        [
            "microsoft/WinUI-Gallery#ContentIsland/BasicContentIslandContent",
            "microsoft/WinUI-Gallery#SystemBackdropElement/SystembackdropelementSample",
        ];

        CatalogGenerationResult result = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot });

        HashSet<string> withCode = result.Code.Scenarios.Select(s => s.Id).ToHashSet(StringComparer.Ordinal);
        string[] codeless = result.Manifest.Samples
            .SelectMany(s => s.Scenarios ?? [])
            .Select(s => s.Id)
            .Where(id => !withCode.Contains(id))
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(
            expected,
            codeless,
            "The set of scenarios without code changed. If you added legacy-source support, remove the entries that now resolve; if a new code-less scenario appeared, confirm it is intentional and add it here.");
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
        // clearly as "the real data is invalid" rather than surfacing only via the manifest diff
        // above.
        CatalogManifest manifest = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot }).Manifest;
        Assert.IsTrue(manifest.SampleCount > 0);
        Assert.AreEqual(manifest.SampleCount, manifest.Samples.Count);
    }
}
