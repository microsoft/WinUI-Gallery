// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
/// The generated index plus any non-fatal problems found on the way.
///
/// Warnings are returned rather than thrown because they describe snippets that are correct for
/// the gallery's own code viewer but cannot be published as paste-ready code — for example a
/// fragment written as "&lt;Window ...&gt;" to stand in for the reader's own window. Failing the
/// build on those would block the exporter on authored content that is not wrong.
/// </summary>
internal sealed record CatalogGenerationResult(SampleIndex Index, IReadOnlyList<CatalogIssue> Warnings);

/// <summary>
/// Builds the catalog/windows-samples.json manifest from ControlInfoData.json plus the on-disk
/// WinUIGallery/Samples/&lt;UniqueId&gt;/ folders. See catalog/README.md for the design.
/// </summary>
internal static partial class CatalogGenerator
{
    private const string ControlInfoRelativePath = "WinUIGallery/SampleSupport/Data/ControlInfoData.json";
    private const string SamplesRelativeRoot = "WinUIGallery/Samples";

    /// <summary>Root namespace of the gallery itself, which no published snippet can rely on.</summary>
    private const string GalleryRootNamespace = "WinUIGallery";

    /// <summary>
    /// A plain "using Some.Namespace;" directive. Alias and static forms are skipped: the consumer
    /// re-emits each entry as "using X;", which would not round-trip either of them.
    /// </summary>
    [GeneratedRegex(@"^\s*using\s+(?!static\b)([A-Za-z_][\w.]*)\s*;", RegexOptions.Multiline)]
    private static partial Regex UsingDirectiveRegex();

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = CatalogReadContext.Default,
    };

    public static readonly JsonSerializerOptions WriteOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = CatalogWriteContext.Default,
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

        List<IndexControl> controls = [];
        List<CatalogIssue> warnings = [];
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

                IndexControl? control = BuildControl(item, group, samplesRoot, itemsById, options, issues, warnings);
                if (control is not null)
                {
                    controls.Add(control);
                }
            }
        }

        // Now that every included id is known, validate cross-references (RelatedControls and
        // Catalog.RelatedSamples) so a typo/rename never silently produces a broken link.
        HashSet<string> includedIds = new(controls.Select(c => $"{RepoId(options)}#{c.Gallery.UniqueId}"), StringComparer.Ordinal);
        foreach (IndexControl control in controls)
        {
            foreach (string relatedId in control.Gallery.RelatedSamples ?? [])
            {
                bool isSameRepo = relatedId.StartsWith(RepoId(options) + "#", StringComparison.Ordinal);
                if (isSameRepo && !includedIds.Contains(relatedId))
                {
                    issues.Add(new CatalogIssue(control.Gallery.UniqueId, $"Related sample reference '{relatedId}' does not resolve to an included catalog entry."));
                }
            }
        }

        // Control ids are what a consumer builds its own sample ids from, so a collision would
        // make two gallery pages indistinguishable. They are lowercased UniqueIds, so this catches
        // two pages whose ids differ only by case.
        HashSet<string> controlIds = new(StringComparer.Ordinal);
        foreach (IndexControl control in controls)
        {
            if (!controlIds.Add(control.Id))
            {
                issues.Add(new CatalogIssue(control.Gallery.UniqueId, $"Duplicate control id '{control.Id}'."));
            }
        }

        // A snippet is the stable identity of a sample within its control, so a page pointing two
        // ControlExamples at one snippet would publish the same code twice under two names.
        foreach (IndexControl control in controls)
        {
            HashSet<string> snippets = new(StringComparer.Ordinal);
            foreach (IndexSample sample in control.Samples)
            {
                if (!snippets.Add(sample.Gallery.Snippet))
                {
                    issues.Add(new CatalogIssue(control.Gallery.UniqueId, $"Duplicate snippet '{sample.Gallery.Snippet}'."));
                }
            }
        }

        if (issues.Count > 0)
        {
            throw new CatalogValidationException(issues);
        }

        controls.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));

        SampleIndex index = new()
        {
            Generator = new IndexGeneratorInfo
            {
                Repository = $"https://github.com/{options.RepoOwner}/{options.RepoName}",
                DefaultBranch = options.DefaultBranch,
            },
            ControlCount = controls.Count,
            Controls = controls,
        };

        return new CatalogGenerationResult(index, warnings);
    }

    private static IndexControl? BuildControl(
        ControlInfoItem item,
        ControlInfoGroup group,
        string samplesRoot,
        IReadOnlyDictionary<string, ControlInfoItem> itemsById,
        CatalogGenerationOptions options,
        List<CatalogIssue> issues,
        List<CatalogIssue> warnings)
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

        List<IndexSample> samples = ExtractSamples(pageFile, item.UniqueId, folder, options, issues, warnings);

        // Namespace imports every sample shares are hoisted to the control, which is exactly the
        // default the contract describes; a sample needing a different set keeps its own.
        List<string>? sharedImports = HoistSharedImports(samples);

        return new IndexControl
        {
            Id = ToControlId(item.UniqueId),
            Name = item.Title,
            Description = NullIfEmpty(item.Subtitle),
            Details = NullIfEmpty(item.Description),
            ApiNamespace = NullIfEmpty(item.ApiNamespace),
            RelatedControls = NullIfEmpty(ResolveRelatedControlNames(item, itemsById)),
            XmlnsImports = sharedImports,
            Usings = NullIfEmpty(CollectUsings(codeBehindFile, samples)),
            Keywords = NullIfEmpty(item.BaseClasses),
            CuratedKeywords = BuildCuratedKeywords(item),
            Docs = item.Docs.Count == 0
                ? null
                : item.Docs.Select(d => new IndexDocLink { Title = d.Title, Uri = d.Uri }).ToList(),
            Gallery = new IndexControlGallery
            {
                UniqueId = item.UniqueId,
                Group = new IndexGroupRef { Id = group.UniqueId, Title = group.Title },
                Page = ToRepoRelative(pageFile, options.RepoRoot),
                CodeBehind = codeBehindFile is null ? null : ToRepoRelative(codeBehindFile, options.RepoRoot),
                BaseClasses = NullIfEmpty(item.BaseClasses),
                Badges = BuildBadges(item),
                RelatedSamples = BuildRelatedSamples(item, options),
            },
            Samples = samples,
        };
    }

    /// <summary>
    /// Lowercases a UniqueId into the short, URL-safe form the contract asks for. Ids are scoped
    /// to a source there, so "Button" is unambiguous without repeating the repository in it.
    /// </summary>
    private static string ToControlId(string uniqueId) => uniqueId.ToLowerInvariant();

    /// <summary>
    /// Search terms the gallery's own authors wrote in ControlInfoData.json. Tags and aliases are
    /// merged because both are hand-written there and the contract has one slot for author terms.
    /// </summary>
    private static List<string>? BuildCuratedKeywords(ControlInfoItem item)
    {
        List<string> keywords = [];
        keywords.AddRange(item.Tags ?? []);
        keywords.AddRange(item.Catalog?.Aliases ?? []);

        return keywords.Count == 0
            ? null
            : keywords.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>
    /// Moves namespace imports up to the control when every sample needs the same ones, and clears
    /// them from the samples. When the samples differ, each keeps its own and the control declares
    /// none — the contract treats a sample's own list as a full override, not an addition, so a
    /// partial control-level list would quietly drop imports for the samples that override it.
    /// </summary>
    private static List<string>? HoistSharedImports(List<IndexSample> samples)
    {
        List<IndexSample> withXaml = samples.Where(s => s.Xaml is not null).ToList();
        if (withXaml.Count == 0)
        {
            return null;
        }

        List<string> first = withXaml[0].XmlnsImports ?? [];
        bool allMatch = withXaml.All(s => (s.XmlnsImports ?? []).SequenceEqual(first, StringComparer.Ordinal));
        if (!allMatch || first.Count == 0)
        {
            return null;
        }

        foreach (IndexSample sample in samples)
        {
            sample.XmlnsImports = null;
        }

        return first;
    }

    /// <summary>
    /// The contract's "relatedControls" is display names, but ControlInfoData.json stores
    /// UniqueIds in that field. Resolving each one keeps consumers from rendering an internal id
    /// like "XamlStyles" where a reader expects "Style". An id that does not resolve is passed
    /// through unchanged so a stale reference stays visible rather than silently disappearing.
    /// </summary>
    private static List<string> ResolveRelatedControlNames(
        ControlInfoItem item,
        IReadOnlyDictionary<string, ControlInfoItem> itemsById)
    {
        List<string> names = [];
        foreach (string relatedControl in item.RelatedControls)
        {
            names.Add(itemsById.TryGetValue(relatedControl, out ControlInfoItem? related)
                && !string.IsNullOrWhiteSpace(related.Title)
                    ? related.Title
                    : relatedControl);
        }

        return names;
    }

    /// <summary>
    /// The namespaces a control's published C# assumes, taken from the imports its page's
    /// code-behind was written against. Over-collecting is deliberate: an unused "using" compiles
    /// harmlessly, while a missing one does not.
    ///
    /// The gallery's own namespaces are excluded, because prepending something like
    /// "using WinUIGallery.Helpers;" would guarantee the failure this field exists to prevent.
    /// </summary>
    private static List<string> CollectUsings(string? codeBehindFile, List<IndexSample> samples)
    {
        if (codeBehindFile is null || !samples.Any(s => !string.IsNullOrWhiteSpace(s.Code)))
        {
            return [];
        }

        SortedSet<string> namespaces = new(StringComparer.Ordinal);
        foreach (Match match in UsingDirectiveRegex().Matches(File.ReadAllText(codeBehindFile)))
        {
            string ns = match.Groups[1].Value;
            if (ns != GalleryRootNamespace && !ns.StartsWith(GalleryRootNamespace + ".", StringComparison.Ordinal))
            {
                namespaces.Add(ns);
            }
        }

        return [.. namespaces];
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

    private static List<IndexSample> ExtractSamples(
        string pageFile,
        string uniqueId,
        string folder,
        CatalogGenerationOptions options,
        List<CatalogIssue> issues,
        List<CatalogIssue> warnings)
    {
        List<IndexSample> samples = [];
        string[] entries = Directory.GetFiles(folder);
        string folderName = Path.GetFileName(folder);

        // The page is parsed rather than pattern-matched so that each snippet can be tied to the
        // ControlExample that owns it, which is what makes its $(Token) substitutions resolvable.
        // Parsing also ignores commented-out markup, which a text scan would treat as real.
        XDocument page;
        try
        {
            page = XDocument.Load(pageFile);
        }
        catch (System.Xml.XmlException ex)
        {
            issues.Add(new CatalogIssue(uniqueId, $"{Path.GetFileName(pageFile)} is not well-formed XML: {ex.Message}"));
            return samples;
        }

        XElement? pageRoot = page.Root;
        if (pageRoot is null)
        {
            return samples;
        }

        Dictionary<string, string> pageDeclarations = XamlFragment.ReadPageDeclarations(pageRoot);

        foreach (XElement controlExample in pageRoot.DescendantsAndSelf().Where(e => e.Name.LocalName == "ControlExample"))
        {
            string? rawPath = (string?)controlExample.Attribute("SampleDefinition");
            if (string.IsNullOrWhiteSpace(rawPath))
            {
                continue;
            }

            // The directory portion is not decoration: ControlExample loads the bundle as
            // "Samples/<SampleDefinition>", so a value naming the wrong folder fails at runtime even
            // when a file of that name happens to sit next to the page. Checking only the file name
            // would let that typo through here and leave it to surface as an empty code viewer.
            string[] segments = rawPath!.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length != 2 || !string.Equals(segments[0], folderName, StringComparison.Ordinal))
            {
                issues.Add(new CatalogIssue(uniqueId, $"SampleDefinition '{rawPath}' must be '{folderName}\\<File>.txt'; the gallery resolves it under Samples\\ and would fail to load it as written."));
                continue;
            }

            string fileName = segments[1];

            string? bundlePath = FindExactCase(entries, fileName);
            if (bundlePath is null)
            {
                issues.Add(new CatalogIssue(uniqueId, $"SampleDefinition references '{fileName}' which was not found (case-exact) next to the page."));
                continue;
            }

            SampleBundle bundle = SampleBundleParser.Parse(File.ReadAllText(bundlePath));

            // A scenario legitimately has no code: either the page hides the viewer entirely
            // (SourceCodeVisibility="Collapsed"), or it swaps ControlExample.XamlSource at runtime,
            // so no single snippet represents it. The contract requires a sample to carry XAML or
            // code, so there is nothing to publish and it is left out.
            if (bundle.Xaml is null && bundle.CSharp is null)
            {
                continue;
            }

            Dictionary<string, string> substitutions = SubstitutionResolver.BuildMap(controlExample, pageRoot);

            string? xaml = NullIfEmpty(SubstitutionResolver.Apply(bundle.Xaml ?? string.Empty, substitutions));
            string? code = NullIfEmpty(SubstitutionResolver.Apply(bundle.CSharp ?? string.Empty, substitutions));

            // The resolver leaves a token alone rather than guess at it, which is right for accuracy
            // but leaves markup a consumer cannot paste. Whatever it declined to settle is dropped
            // here, along with the attribute carrying it, so published XAML never contains a
            // placeholder.
            List<string>? droppedPlaceholders = null;
            if (TokenFallback.ContainsToken(xaml))
            {
                List<string> names = TokenFallback.TokenNames(xaml!);
                string? stripped = NullIfEmpty(TokenFallback.StripFromXaml(xaml!));

                // A token standing in for an element name, as in "<$(EasingFunction)/>", has no
                // attribute to drop and no default to fall back to. Nothing is claimed in that case:
                // the fragment simply does not parse and the check below omits it, exactly as it did
                // before this fallback existed.
                if (!TokenFallback.ContainsToken(stripped))
                {
                    droppedPlaceholders = names;
                    xaml = stripped;
                    warnings.Add(new CatalogIssue(uniqueId, $"'{fileName}' had unresolved placeholders ({string.Join(", ", names)}); the attributes carrying them were dropped."));
                }
            }

            // Consumers parse the XAML and discard whatever fails, so publishing a fragment that
            // cannot parse would advertise code that never arrives. Dropping it here instead keeps
            // the index honest and makes the reason visible in the build output.
            bool malformed = xaml is not null && !XamlFragment.IsWellFormed(xaml);
            if (malformed)
            {
                warnings.Add(new CatalogIssue(uniqueId, $"'{fileName}' XAML is not a well-formed fragment and was omitted."));
                xaml = null;
            }

            // A prefix the page never declares cannot be turned into an import, and DetectImports
            // drops it rather than inventing a URI. That leaves a fragment referencing a namespace
            // nothing binds: it parses here and on the consumer's side, because both synthesize
            // declarations, and then fails the moment a reader pastes it with the imports this index
            // published. Omitting the XAML is the same degradation a malformed fragment gets, for
            // the same reason - publishing it would advertise code that does not work on arrival.
            //
            // Unless the snippet's own C# accounts for the prefix. A sample that shows "local:Foo"
            // in its XAML and declares Foo in the code beside it is self-contained, and withholding
            // its XAML hides markup the reader could paste for the sake of a namespace line that
            // can be derived from the snippet itself.
            Dictionary<string, string> sampleDeclarations = pageDeclarations;
            List<string> unboundPrefixes = xaml is null ? [] : XamlFragment.UnresolvedPrefixes(xaml, pageDeclarations);
            if (unboundPrefixes.Count > 0)
            {
                Dictionary<string, string> fromCode = XamlFragment.ResolvePrefixesFromCode(xaml!, unboundPrefixes, code);
                if (fromCode.Count > 0)
                {
                    sampleDeclarations = new Dictionary<string, string>(pageDeclarations, StringComparer.Ordinal);
                    foreach ((string prefix, string uri) in fromCode)
                    {
                        sampleDeclarations[prefix] = uri;
                    }

                    unboundPrefixes = [.. unboundPrefixes.Where(p => !fromCode.ContainsKey(p))];
                }
            }

            if (unboundPrefixes.Count > 0)
            {
                warnings.Add(new CatalogIssue(uniqueId, $"'{fileName}' XAML uses namespace prefixes neither its page nor its code declares ({string.Join(", ", unboundPrefixes)}) and was omitted."));
                xaml = null;
            }

            // Nothing below may publish a placeholder. The fallback and the parse check together are
            // meant to make this unreachable, so failing here is deliberate: the index's promise is
            // that its XAML pastes as published, and shipping a token would quietly break it.
            if (TokenFallback.ContainsToken(xaml))
            {
                issues.Add(new CatalogIssue(uniqueId, $"'{fileName}' still contains unresolved placeholders ({string.Join(", ", TokenFallback.TokenNames(xaml!))}) after fallback removal."));
                continue;
            }

            if (xaml is null && code is null)
            {
                continue;
            }

            // C# gets no such gate, and the asymmetry is deliberate. The XAML fallback works only
            // because deleting an attribute leaves the property at its own default; C# has no
            // construct whose absence means "default", and the tokens that occur here sit in
            // identifier fragments, fixed-arity argument lists, and whole statements, none of which
            // survive having the token cut out. So they ship as written, and the names are declared
            // instead, so a consumer can tell templated code from code it can paste.
            List<string>? codePlaceholders = TokenFallback.ContainsToken(code)
                ? TokenFallback.TokenNames(code!)
                : null;

            samples.Add(new IndexSample
            {
                Header = NullIfEmpty(bundle.Header) ?? DeriveScenarioName(fileName, uniqueId),
                Xaml = xaml,
                Code = code,
                Language = code is null ? null : "csharp",
                XmlnsImports = xaml is null ? null : NullIfEmpty(XamlFragment.DetectImports(xaml, sampleDeclarations)),
                Gallery = new IndexSampleGallery
                {
                    Snippet = fileName,
                    Source = ToRepoRelative(bundlePath, options.RepoRoot),
                    Name = DeriveScenarioName(fileName, uniqueId),
                    XamlOmittedAsMalformed = malformed ? true : null,
                    XamlOmittedUnboundPrefixes = unboundPrefixes.Count == 0 ? null : unboundPrefixes,
                    XamlPlaceholdersDropped = xaml is null ? null : droppedPlaceholders,
                    CodePlaceholdersPresent = codePlaceholders,
                },
            });
        }

        // Page order is preserved deliberately: it is the order a visitor sees, and the contract's
        // consumer numbers samples positionally, so sorting them would renumber ids whenever a
        // ControlExample is inserted.
        return samples;
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

    private static List<string>? NullIfEmpty(List<string> value) => value.Count == 0 ? null : value;

    /// <summary>Serializes the index deterministically (stable property/array order, LF line endings).</summary>
    public static string Serialize(SampleIndex index)
    {
        string json = JsonSerializer.Serialize(index, WriteOptions);
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
