// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// A single, reportable problem found while validating source data or generating the manifest.
/// Multiple issues are collected and reported together instead of failing on the first one.
/// </summary>
internal sealed record CatalogIssue(string UniqueId, string Message)
{
    public override string ToString() => string.IsNullOrEmpty(UniqueId) ? Message : $"{UniqueId}: {Message}";
}

/// <summary>Thrown by <see cref="CatalogGenerator.Generate"/> when validation fails.</summary>
internal sealed class CatalogValidationException(IReadOnlyList<CatalogIssue> issues)
    : Exception("Catalog validation failed:\n" + string.Join('\n', issues.Select(i => " - " + i)))
{
    public IReadOnlyList<CatalogIssue> Issues { get; } = issues;
}

internal sealed class CatalogGenerationOptions
{
    /// <summary>Absolute path to the repository root (folder containing WinUIGallery.slnx).</summary>
    public required string RepoRoot { get; init; }
    public string RepoOwner { get; init; } = "microsoft";
    public string RepoName { get; init; } = "WinUI-Gallery";
    public string DefaultBranch { get; init; } = "main";
}

/// <summary>
/// Both generated artifacts, produced by a single walk of the source data. They are returned
/// together on purpose: the code file is keyed by the manifest's scenario ids, so generating them
/// independently would make it possible to commit a mismatched pair.
/// </summary>
internal sealed record CatalogGenerationResult(CatalogManifest Manifest, CatalogCodeManifest Code);

/// <summary>
/// Builds the catalog/windows-samples.json manifest from ControlInfoData.json plus the on-disk
/// WinUIGallery/Samples/&lt;UniqueId&gt;/ folders. See catalog/README.md for the design.
/// </summary>
internal static class CatalogGenerator
{
    private const string ControlInfoRelativePath = "WinUIGallery/SampleSupport/Data/ControlInfoData.json";
    private const string SamplesRelativeRoot = "WinUIGallery/Samples";

    private static readonly Regex SampleDefinitionRegex = new(
        "SampleDefinition\\s*=\\s*\"(?<path>[^\"]+)\"",
        RegexOptions.Compiled);

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static readonly JsonSerializerOptions WriteOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>Reads ControlInfoData.json and the Samples folders and produces validated artifacts.</summary>
    public static CatalogGenerationResult Generate(CatalogGenerationOptions options)
    {
        string controlInfoPath = Path.Combine(options.RepoRoot, Normalize(ControlInfoRelativePath));
        if (!File.Exists(controlInfoPath))
        {
            throw new FileNotFoundException($"Could not find ControlInfoData.json at '{controlInfoPath}'.", controlInfoPath);
        }

        string json = File.ReadAllText(controlInfoPath);
        ControlInfoRoot? root = JsonSerializer.Deserialize<ControlInfoRoot>(json, ReadOptions);
        if (root is null)
        {
            throw new InvalidDataException($"'{controlInfoPath}' did not deserialize to a valid ControlInfoData document.");
        }

        List<CatalogIssue> issues = [];
        string samplesRoot = Path.Combine(options.RepoRoot, Normalize(SamplesRelativeRoot));

        // Every UniqueId in the file must be unique - collisions would silently shadow items in
        // the running app too, but we check independently here since the exporter is a separate
        // source of truth for validation.
        Dictionary<string, ControlInfoItem> itemsById = new(StringComparer.Ordinal);
        foreach (ControlInfoGroup group in root.Groups)
        {
            foreach (ControlInfoItem item in group.Items)
            {
                if (string.IsNullOrWhiteSpace(item.UniqueId))
                {
                    issues.Add(new CatalogIssue(string.Empty, $"Item with empty UniqueId in group '{group.UniqueId}'."));
                    continue;
                }

                if (!itemsById.TryAdd(item.UniqueId, item))
                {
                    issues.Add(new CatalogIssue(item.UniqueId, "Duplicate UniqueId across ControlInfoData.json groups."));
                }
            }
        }

        List<CatalogSample> samples = [];
        List<CatalogScenarioCode> code = [];
        foreach (ControlInfoGroup group in root.Groups)
        {
            foreach (ControlInfoItem item in group.Items)
            {
                if (string.IsNullOrWhiteSpace(item.UniqueId))
                {
                    continue; // already reported above
                }

                if (item.Catalog?.Exclude == true)
                {
                    continue;
                }

                CatalogSample? sample = BuildSample(item, group, samplesRoot, options, issues, code);
                if (sample is not null)
                {
                    samples.Add(sample);
                }
            }
        }

        // Now that every included sample id is known, validate cross-references (RelatedControls
        // and Catalog.RelatedSamples) so a typo/rename never silently produces a broken link.
        HashSet<string> includedIds = new(samples.Select(s => s.Id), StringComparer.Ordinal);
        foreach (CatalogSample sample in samples)
        {
            if (sample.RelatedSamples is null)
            {
                continue;
            }

            foreach (string relatedId in sample.RelatedSamples)
            {
                bool isSameRepo = relatedId.StartsWith(RepoId(options) + "#", StringComparison.Ordinal);
                if (isSameRepo && !includedIds.Contains(relatedId))
                {
                    issues.Add(new CatalogIssue(sample.UniqueId, $"Related sample reference '{relatedId}' does not resolve to an included catalog entry."));
                }
            }
        }

        // Scenario ids are the join key into the code file, so a collision would make one
        // scenario's source unreachable. They are derived from snippet file names, so this
        // catches two ControlExamples in one page pointing at the same snippet.
        HashSet<string> scenarioIds = new(StringComparer.Ordinal);
        foreach (CatalogSample sample in samples)
        {
            foreach (CatalogScenario scenario in sample.Scenarios ?? [])
            {
                if (!scenarioIds.Add(scenario.Id))
                {
                    issues.Add(new CatalogIssue(sample.UniqueId, $"Duplicate scenario id '{scenario.Id}'."));
                }
            }
        }

        if (issues.Count > 0)
        {
            throw new CatalogValidationException(issues);
        }

        samples.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));
        code.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));

        CatalogManifest manifest = new()
        {
            Generator = new CatalogGeneratorInfo(),
            Repository = new CatalogRepository
            {
                Id = RepoId(options),
                Owner = options.RepoOwner,
                Name = options.RepoName,
                Url = $"https://github.com/{options.RepoOwner}/{options.RepoName}",
                DefaultBranch = options.DefaultBranch,
                License = "MIT",
            },
            Defaults = new CatalogDefaults(),
            SampleCount = samples.Count,
            Samples = samples,
        };

        CatalogCodeManifest codeManifest = new()
        {
            Generator = new CatalogGeneratorInfo(),
            ScenarioCount = code.Count,
            Scenarios = code,
        };

        return new CatalogGenerationResult(manifest, codeManifest);
    }

    private static CatalogSample? BuildSample(
        ControlInfoItem item,
        ControlInfoGroup group,
        string samplesRoot,
        CatalogGenerationOptions options,
        List<CatalogIssue> issues,
        List<CatalogScenarioCode> code)
    {
        string folder = Path.Combine(samplesRoot, item.UniqueId);
        if (!Directory.Exists(folder))
        {
            issues.Add(new CatalogIssue(item.UniqueId, $"No sample folder found at 'WinUIGallery/Samples/{item.UniqueId}'."));
            return null;
        }

        string[] entries = Directory.GetFiles(folder);
        string expectedPageFile = item.UniqueId + "Page.xaml";
        string expectedCodeBehindFile = item.UniqueId + "Page.xaml.cs";

        string? pageFile = FindExactCase(entries, expectedPageFile);
        if (pageFile is null)
        {
            issues.Add(new CatalogIssue(
                item.UniqueId,
                $"Expected page file '{expectedPageFile}' was not found (case-exact) in 'WinUIGallery/Samples/{item.UniqueId}'."));
            return null;
        }

        string? codeBehindFile = FindExactCase(entries, expectedCodeBehindFile);

        List<string> snippets = entries
            .Where(f => f.EndsWith(".txt", StringComparison.Ordinal))
            .Select(f => Path.GetFileName(f))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();

        string relativeRoot = ToRepoRelative(folder, options.RepoRoot);
        string id = $"{RepoId(options)}#{item.UniqueId}";

        List<CatalogScenario> scenarios = ExtractScenarios(pageFile, item.UniqueId, id, folder, options, issues, code);

        List<string>? relatedSamples = BuildRelatedSamples(item, options);

        return new CatalogSample
        {
            Id = id,
            UniqueId = item.UniqueId,
            Title = item.Title,
            Group = new CatalogGroupRef { Id = group.UniqueId, Title = group.Title },
            Summary = NullIfEmpty(item.Subtitle),
            Description = NullIfEmpty(item.Description),
            ApiNamespace = NullIfEmpty(item.ApiNamespace),
            BaseClasses = NullIfEmpty(item.BaseClasses),
            Tags = NullIfEmpty(item.Tags),
            Aliases = NullIfEmpty(item.Catalog?.Aliases),
            RelatedSamples = relatedSamples,
            Docs = item.Docs.Count == 0
                ? null
                : item.Docs.Select(d => new CatalogDocLink { Title = d.Title, Uri = d.Uri }).ToList(),
            Badges = BuildBadges(item),
            Source = new CatalogSource
            {
                Root = relativeRoot,
                Page = ToRepoRelative(pageFile, options.RepoRoot),
                CodeBehind = codeBehindFile is null ? null : ToRepoRelative(codeBehindFile, options.RepoRoot),
                Snippets = snippets.Count == 0 ? null : snippets.Select(f => $"{relativeRoot}/{f}").ToList(),
            },
            Scenarios = scenarios.Count == 0 ? null : scenarios,
        };
    }

    private static List<string>? BuildRelatedSamples(ControlInfoItem item, CatalogGenerationOptions options)
    {
        List<string> related = [];
        foreach (string relatedControl in item.RelatedControls)
        {
            related.Add($"{RepoId(options)}#{relatedControl}");
        }

        if (item.Catalog?.RelatedSamples is { Length: > 0 } extra)
        {
            related.AddRange(extra);
        }

        if (related.Count == 0)
        {
            return null;
        }

        return related.Distinct(StringComparer.Ordinal).OrderBy(r => r, StringComparer.Ordinal).ToList();
    }

    private static List<string>? BuildBadges(ControlInfoItem item)
    {
        List<string> badges = [];
        if (item.IsNew)
        {
            badges.Add("New");
        }

        if (item.IsUpdated)
        {
            badges.Add("Updated");
        }

        if (item.IsPreview)
        {
            badges.Add("Preview");
        }

        return badges.Count == 0 ? null : badges;
    }

    private static List<CatalogScenario> ExtractScenarios(
        string pageFile,
        string uniqueId,
        string sampleId,
        string folder,
        CatalogGenerationOptions options,
        List<CatalogIssue> issues,
        List<CatalogScenarioCode> code)
    {
        string contents = File.ReadAllText(pageFile);
        List<CatalogScenario> scenarios = [];
        string[] entries = Directory.GetFiles(folder);

        foreach (Match match in SampleDefinitionRegex.Matches(contents))
        {
            // SampleDefinition values are written as "<Folder>\<File>.txt" (folder name matches
            // the sample's UniqueId), so only the file name is meaningful here.
            string rawPath = match.Groups["path"].Value;
            string fileName = rawPath.Replace('\\', '/').Split('/').Last();

            string? bundlePath = FindExactCase(entries, fileName);
            if (bundlePath is null)
            {
                issues.Add(new CatalogIssue(uniqueId, $"SampleDefinition references '{fileName}' which was not found (case-exact) next to the page."));
                continue;
            }

            SampleBundle bundle = SampleBundleParser.Parse(File.ReadAllText(bundlePath));

            string scenarioId = $"{sampleId}/{Path.GetFileNameWithoutExtension(fileName)}";

            scenarios.Add(new CatalogScenario
            {
                Id = scenarioId,
                Name = DeriveScenarioName(fileName, uniqueId),
                Description = NullIfEmpty(bundle.Header),
                Snippet = fileName,
            });

            // A scenario legitimately has no code: either the page hides the viewer entirely
            // (SourceCodeVisibility="Collapsed"), or the code still comes from the legacy
            // ControlExample.XamlSource/CSharpSource properties, which this exporter does not
            // read yet. Such scenarios appear in the manifest but contribute no code entry.
            // KnownCodelessScenariosTests pins the current set so this gap stays visible.
            if (bundle.Xaml is null && bundle.CSharp is null)
            {
                continue;
            }

            code.Add(new CatalogScenarioCode
            {
                Id = scenarioId,
                Source = ToRepoRelative(bundlePath, options.RepoRoot),
                Xaml = NullIfEmpty(bundle.Xaml),
                Code = NullIfEmpty(bundle.CSharp),
            });
        }

        return scenarios
            .OrderBy(s => s.Snippet, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Snippet files are named "&lt;UniqueId&gt;&lt;Scenario&gt;.txt" by convention (e.g.
    /// "ButtonBuiltInStyles.txt" for the "Button" sample). Strip the UniqueId prefix and the
    /// extension, then insert spaces before capitals, to get a human-readable scenario name.
    /// Falls back to the file name (without extension) when the convention isn't followed.
    /// </summary>
    private static string DeriveScenarioName(string fileName, string uniqueId)
    {
        string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string suffix = withoutExtension.StartsWith(uniqueId, StringComparison.OrdinalIgnoreCase)
            ? withoutExtension[uniqueId.Length..]
            : withoutExtension;

        if (string.IsNullOrWhiteSpace(suffix))
        {
            return uniqueId;
        }

        StringBuilder sb = new();
        for (int i = 0; i < suffix.Length; i++)
        {
            char c = suffix[i];
            if (i > 0 && char.IsUpper(c) && !char.IsUpper(suffix[i - 1]))
            {
                sb.Append(' ');
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private static string? FindExactCase(string[] filesInFolder, string expectedFileName)
    {
        foreach (string file in filesInFolder)
        {
            string name = Path.GetFileName(file);
            if (string.Equals(name, expectedFileName, StringComparison.Ordinal))
            {
                return file;
            }
        }

        return null;
    }

    private static string ToRepoRelative(string absolutePath, string repoRoot)
    {
        string relative = Path.GetRelativePath(repoRoot, absolutePath);
        return relative.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
    }

    private static string RepoId(CatalogGenerationOptions options) => $"{options.RepoOwner}/{options.RepoName}";

    private static string Normalize(string relativePath) => relativePath.Replace('/', Path.DirectorySeparatorChar);

    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;

    private static List<string>? NullIfEmpty(string[]? value) => value is null || value.Length == 0 ? null : [.. value];

    /// <summary>Serializes the manifest deterministically (stable property/array order, LF line endings).</summary>
    public static string Serialize(CatalogManifest manifest) => SerializeDocument(manifest);

    /// <summary>Serializes the code file deterministically, matching <see cref="Serialize(CatalogManifest)"/>.</summary>
    public static string Serialize(CatalogCodeManifest code) => SerializeDocument(code);

    private static string SerializeDocument<T>(T document)
    {
        string json = JsonSerializer.Serialize(document, WriteOptions);
        return json.Replace("\r\n", "\n").TrimEnd('\n') + "\n";
    }

    /// <summary>Walks up from <paramref name="startDirectory"/> to find the repo root (WinUIGallery.slnx).</summary>
    public static string FindRepoRoot(string startDirectory)
    {
        DirectoryInfo? dir = new(startDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "WinUIGallery.slnx")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException($"Could not locate WinUIGallery.slnx above '{startDirectory}'.");
    }
}
