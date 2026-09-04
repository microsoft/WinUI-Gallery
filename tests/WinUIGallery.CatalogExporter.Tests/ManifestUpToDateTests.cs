// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Guards against a stale committed catalog/windows-samples.json: regenerates the manifest from
/// the real, current repository data and fails if it does not byte-for-byte match what is
/// checked in. Run `dotnet run --project tools/CatalogExporter -- generate` and commit the
/// result if this test fails.
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
        string fresh = CatalogGenerator.Serialize(CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = repoRoot }));

        Assert.AreEqual(
            fresh,
            committed,
            "catalog/windows-samples.json is stale relative to ControlInfoData.json / the Samples folders. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");
    }

    [TestMethod]
    public void RealRepository_HasNoValidationIssues()
    {
        // Re-asserts the same validation Generate() already performs, so a failure here reports
        // clearly as "the real data is invalid" rather than surfacing only via the manifest diff
        // above.
        CatalogManifest manifest = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot });
        Assert.IsTrue(manifest.SampleCount > 0);
        Assert.AreEqual(manifest.SampleCount, manifest.Samples.Count);
    }
}
