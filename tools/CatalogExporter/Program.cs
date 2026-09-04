// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Regenerates or verifies catalog/windows-samples.json.
///
/// Usage:
///   dotnet run --project tools/CatalogExporter -- generate [--repo-root &lt;path&gt;]
///   dotnet run --project tools/CatalogExporter -- check [--repo-root &lt;path&gt;]
///
/// "generate" writes the manifest to catalog/windows-samples.json.
/// "check" regenerates the manifest in memory and fails (non-zero exit code) if the committed
/// file is stale or missing, without modifying anything on disk.
/// </summary>
internal static class Program
{
    private const string ManifestRelativePath = "catalog/windows-samples.json";

    private static int Main(string[] args)
    {
        if (args.Length == 0 || (args[0] != "generate" && args[0] != "check"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project tools/CatalogExporter -- <generate|check> [--repo-root <path>]");
            return 2;
        }

        string command = args[0];
        string repoRoot = ParseRepoRoot(args) ?? CatalogGenerator.FindRepoRoot(Directory.GetCurrentDirectory());
        string manifestPath = Path.Combine(repoRoot, ManifestRelativePath.Replace('/', Path.DirectorySeparatorChar));

        CatalogGenerationOptions options = new() { RepoRoot = repoRoot };

        CatalogManifest manifest;
        try
        {
            manifest = CatalogGenerator.Generate(options);
        }
        catch (CatalogValidationException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        string generated = CatalogGenerator.Serialize(manifest);

        if (command == "generate")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(manifestPath)!);
            File.WriteAllText(manifestPath, generated);
            Console.WriteLine($"Wrote {manifest.SampleCount} samples to {ManifestRelativePath}.");
            return 0;
        }

        // command == "check"
        if (!File.Exists(manifestPath))
        {
            Console.Error.WriteLine($"{ManifestRelativePath} does not exist. Run 'generate' and commit the result.");
            return 1;
        }

        string committed = File.ReadAllText(manifestPath).Replace("\r\n", "\n");
        if (committed != generated)
        {
            Console.Error.WriteLine($"{ManifestRelativePath} is stale. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");
            return 1;
        }

        Console.WriteLine($"{ManifestRelativePath} is up to date ({manifest.SampleCount} samples).");
        return 0;
    }

    private static string? ParseRepoRoot(string[] args)
    {
        for (int i = 1; i < args.Length - 1; i++)
        {
            if (args[i] == "--repo-root")
            {
                return Path.GetFullPath(args[i + 1]);
            }
        }

        return null;
    }
}
