// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Regenerates or verifies the generated catalog files.
///
/// Usage:
///   dotnet run --project tools/CatalogExporter -- generate [--repo-root &lt;path&gt;]
///   dotnet run --project tools/CatalogExporter -- check [--repo-root &lt;path&gt;]
///
/// "generate" writes catalog/windows-samples.json and catalog/windows-samples.code.json.
/// "check" regenerates both in memory and fails (non-zero exit code) if either committed file is
/// stale or missing, without modifying anything on disk.
/// </summary>
internal static class Program
{
    private const string ManifestRelativePath = "catalog/windows-samples.json";
    private const string CodeRelativePath = "catalog/windows-samples.code.json";

    private static int Main(string[] args)
    {
        if (args.Length == 0 || (args[0] != "generate" && args[0] != "check"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project tools/CatalogExporter -- <generate|check> [--repo-root <path>]");
            return 2;
        }

        string command = args[0];
        string repoRoot = ParseRepoRoot(args) ?? CatalogGenerator.FindRepoRoot(Directory.GetCurrentDirectory());

        CatalogGenerationOptions options = new() { RepoRoot = repoRoot };

        CatalogGenerationResult result;
        try
        {
            result = CatalogGenerator.Generate(options);
        }
        catch (CatalogValidationException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        (string Path, string Content)[] outputs =
        [
            (ManifestRelativePath, CatalogGenerator.Serialize(result.Manifest)),
            (CodeRelativePath, CatalogGenerator.Serialize(result.Code)),
        ];

        if (command == "generate")
        {
            foreach ((string relativePath, string content) in outputs)
            {
                string absolutePath = Absolute(repoRoot, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
                File.WriteAllText(absolutePath, content);
            }

            Console.WriteLine($"Wrote {result.Manifest.SampleCount} samples to {ManifestRelativePath} and {result.Code.ScenarioCount} scenarios to {CodeRelativePath}.");
            return 0;
        }

        // command == "check"
        bool stale = false;
        foreach ((string relativePath, string content) in outputs)
        {
            string absolutePath = Absolute(repoRoot, relativePath);
            if (!File.Exists(absolutePath))
            {
                Console.Error.WriteLine($"{relativePath} does not exist. Run 'generate' and commit the result.");
                stale = true;
                continue;
            }

            if (File.ReadAllText(absolutePath).Replace("\r\n", "\n") != content)
            {
                Console.Error.WriteLine($"{relativePath} is stale. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");
                stale = true;
            }
        }

        if (stale)
        {
            return 1;
        }

        Console.WriteLine($"Catalog is up to date ({result.Manifest.SampleCount} samples, {result.Code.ScenarioCount} scenarios).");
        return 0;
    }

    private static string Absolute(string repoRoot, string relativePath) =>
        Path.Combine(repoRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

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
