// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Regenerates or verifies the generated sample index.
///
/// Usage:
///   dotnet run --project tools/CatalogExporter -- generate [--repo-root &lt;path&gt;]
///   dotnet run --project tools/CatalogExporter -- check [--repo-root &lt;path&gt;]
///
/// "generate" writes catalog/windows-samples.json. "check" regenerates it in memory and fails
/// (non-zero exit code) if the committed file is stale or missing, without modifying anything on
/// disk.
/// </summary>
internal static class Program
{
    private const string IndexRelativePath = "catalog/windows-samples.json";

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

        string content = CatalogGenerator.Serialize(result.Index);
        string absolutePath = Absolute(repoRoot, IndexRelativePath);
        int sampleCount = result.Index.Controls.Sum(c => c.Samples.Count);

        foreach (CatalogIssue warning in result.Warnings)
        {
            Console.WriteLine($"warning: {warning}");
        }

        if (command == "generate")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            File.WriteAllText(absolutePath, content);

            Console.WriteLine($"Wrote {result.Index.ControlCount} controls and {sampleCount} samples to {IndexRelativePath}.");
            return 0;
        }

        // command == "check"
        if (!File.Exists(absolutePath))
        {
            Console.Error.WriteLine($"{IndexRelativePath} does not exist. Run 'generate' and commit the result.");
            return 1;
        }

        if (File.ReadAllText(absolutePath).Replace("\r\n", "\n") != content)
        {
            Console.Error.WriteLine($"{IndexRelativePath} is stale. Run 'dotnet run --project tools/CatalogExporter -- generate' and commit the result.");
            return 1;
        }

        Console.WriteLine($"Index is up to date ({result.Index.ControlCount} controls, {sampleCount} samples).");
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
