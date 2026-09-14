// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        //     via the legacy ControlExample.XamlSource property, which the exporter does not read
        //     yet. Expected to disappear from this list once legacy sources are supported.
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
