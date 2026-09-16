// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Structural checks on a snippet's XAML, plus the namespace declarations a consumer needs to
/// paste it somewhere else.
/// </summary>
internal static partial class XamlFragment
{
    /// <summary>
    /// Prefixes a consumer's page already declares (or that exist only at design time), so
    /// emitting them as required imports would be noise at best and a duplicate attribute at
    /// worst.
    /// </summary>
    private static readonly HashSet<string> IgnoredPrefixes = new(StringComparer.Ordinal)
    {
        "x", "d", "mc", "xml", "xmlns",

        // Not a prefix at all: "using" is the scheme half of a XAML namespace URI, as in
        // xmlns:sys="using:System". The type-reference branch of BindingPrefixRegex cannot tell
        // that apart from a real "prefix:Type" reference, and no consumer ever needs an import
        // for it.
        "using",
    };

    /// <summary>
    /// True when <paramref name="xaml"/> parses as a well-formed XML fragment.
    ///
    /// This mirrors winappCli's ScenarioSanitizer.XamlIsWellFormed, including its trick of
    /// synthesizing a declaration for every prefix it sees, so that a snippet using an undeclared
    /// prefix is not treated as broken. Matching the consumer's check matters more than being
    /// stricter than it: anything this accepts but the consumer rejects would be published and
    /// then silently discarded on the other side.
    /// </summary>
    public static bool IsWellFormed(string xaml)
    {
        HashSet<string> prefixes = new(StringComparer.Ordinal);
        foreach (Match match in AnyPrefixRegex().Matches(xaml))
        {
            string prefix = match.Groups[1].Value;

            // xml and xmlns are reserved and cannot be (re)declared.
            if (prefix is "xml" or "xmlns")
            {
                continue;
            }

            prefixes.Add(prefix);
        }

        StringBuilder wrapped = new("<catalogExporterRoot");
        foreach (string prefix in prefixes)
        {
            wrapped.Append(" xmlns:").Append(prefix).Append("=\"urn:catalog:").Append(prefix).Append('"');
        }

        wrapped.Append('>').Append(xaml).Append("</catalogExporterRoot>");

        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
        };

        try
        {
            using XmlReader reader = XmlReader.Create(new StringReader(wrapped.ToString()), settings);
            while (reader.Read())
            {
            }

            return true;
        }
        catch (XmlException)
        {
            return false;
        }
    }

    /// <summary>
    /// Collects the xmlns declarations on a page's root element, keyed by prefix.
    /// </summary>
    public static Dictionary<string, string> ReadPageDeclarations(XElement pageRoot)
    {
        Dictionary<string, string> declarations = new(StringComparer.Ordinal);
        foreach (XAttribute attribute in pageRoot.Attributes().Where(a => a.IsNamespaceDeclaration))
        {
            // The default xmlns has an empty local prefix and needs no import: a consumer pasting
            // into an existing page already has it.
            if (attribute.Name.NamespaceName.Length == 0)
            {
                continue;
            }

            declarations[attribute.Name.LocalName] = attribute.Value;
        }

        return declarations;
    }

    /// <summary>
    /// Returns the full xmlns declarations <paramref name="xaml"/> needs, resolved against the
    /// declarations on its own page. Only prefixes the snippet actually uses are returned, so a
    /// consumer is not told to add namespaces for controls that never appear in it.
    ///
    /// A prefix the page does not declare is skipped rather than guessed at: inventing a URI would
    /// produce an import that looks authoritative and does not compile.
    /// </summary>
    public static List<string> DetectImports(string xaml, IReadOnlyDictionary<string, string> pageDeclarations)
    {
        SortedSet<string> imports = new(StringComparer.Ordinal);

        foreach (string prefix in UsedPrefixes(xaml))
        {
            if (pageDeclarations.TryGetValue(prefix, out string? uri))
            {
                imports.Add($"xmlns:{prefix}=\"{uri}\"");
            }
        }

        return [.. imports];
    }

    /// <summary>
    /// Prefixes <paramref name="xaml"/> binds to that its own page never declares, so no import
    /// can be published for them unless <see cref="ResolvePrefixesFromCode"/> accounts for one.
    ///
    /// <see cref="IsWellFormed"/> cannot surface these, by design: it synthesizes a declaration for
    /// every prefix it sees so that it agrees with the consumer's parser. The fragment therefore
    /// parses on both sides and fails only when someone pastes it alongside the imports this index
    /// published — which are necessarily missing the one it actually needed.
    /// </summary>
    public static List<string> UnresolvedPrefixes(string xaml, IReadOnlyDictionary<string, string> pageDeclarations)
    {
        // A snippet that declares a prefix on its own root carries the binding with it, so it needs
        // nothing from the page and nothing published alongside it.
        HashSet<string> selfDeclared = new(StringComparer.Ordinal);
        foreach (Match match in SelfDeclaredPrefixRegex().Matches(xaml))
        {
            selfDeclared.Add(match.Groups[1].Value);
        }

        SortedSet<string> unresolved = new(StringComparer.Ordinal);

        foreach (string prefix in UsedPrefixes(xaml))
        {
            if (!pageDeclarations.ContainsKey(prefix) && !selfDeclared.Contains(prefix))
            {
                unresolved.Add(prefix);
            }
        }

        return [.. unresolved];
    }

    /// <summary>
    /// Imports for prefixes that resolve against the sample's own C# rather than its page.
    ///
    /// A snippet that hands the reader both halves of a scenario — "local:ExplorerItem" in the XAML
    /// and the ExplorerItem class in the code beside it — is complete on its own; the only thing
    /// missing is the line that binds the prefix, which the page cannot supply because the type
    /// does not live there. Synthesizing that line from the code's own namespace is not a guess:
    /// the namespace and the types are both taken from the snippet being published.
    ///
    /// A prefix is only resolved when every type it qualifies is declared in that code AND those
    /// types all sit in one namespace. One unaccounted-for type means the reader is still missing a
    /// piece; types spread across two namespaces mean no single import covers them, and naming
    /// either one would publish a binding that does not contain what the XAML asks for. Both cases
    /// leave the caller to withhold the fragment exactly as before.
    /// </summary>
    public static Dictionary<string, string> ResolvePrefixesFromCode(
        string xaml,
        IEnumerable<string> prefixes,
        string? code)
    {
        Dictionary<string, string> resolved = new(StringComparer.Ordinal);

        Dictionary<string, string> declaredTypes = CodeDeclarations.DeclaredTypes(code);
        if (declaredTypes.Count == 0)
        {
            return resolved;
        }

        Dictionary<string, HashSet<string>> referenced = ReferencedTypes(xaml);

        foreach (string prefix in prefixes)
        {
            if (!referenced.TryGetValue(prefix, out HashSet<string>? types))
            {
                continue;
            }

            string? ns = SharedNamespace(types, declaredTypes);
            if (ns is not null)
            {
                resolved[prefix] = $"using:{ns}";
            }
        }

        return resolved;
    }

    /// <summary>
    /// The one namespace holding every type in <paramref name="types"/>, or null when some type is
    /// not declared at all or they do not agree on a single namespace.
    /// </summary>
    private static string? SharedNamespace(
        HashSet<string> types,
        IReadOnlyDictionary<string, string> declaredTypes)
    {
        string? shared = null;

        foreach (string type in types)
        {
            if (!declaredTypes.TryGetValue(type, out string? candidate))
            {
                return null;
            }

            if (shared is null)
            {
                shared = candidate;
            }
            else if (!string.Equals(shared, candidate, StringComparison.Ordinal))
            {
                return null;
            }
        }

        return shared;
    }

    /// <summary>
    /// The type names each prefix qualifies, keyed by prefix. A property-element or attached
    /// property such as "local:MenuItemTemplateSelector.ItemTemplate" contributes the type half
    /// only, since that is what has to exist for the reference to resolve.
    /// </summary>
    private static Dictionary<string, HashSet<string>> ReferencedTypes(string xaml)
    {
        Dictionary<string, HashSet<string>> references = new(StringComparer.Ordinal);

        foreach ((string prefix, string type) in PrefixReferences(xaml))
        {
            if (!references.TryGetValue(prefix, out HashSet<string>? types))
            {
                types = new HashSet<string>(StringComparer.Ordinal);
                references[prefix] = types;
            }

            types.Add(type);
        }

        return references;
    }

    /// <summary>
    /// Prefixes used in a way that actually binds to a namespace: an element name, an attribute
    /// name, a markup extension, or a type reference in an attribute value.
    ///
    /// This is deliberately narrower than the regex used by <see cref="IsWellFormed"/>, which
    /// over-collects on purpose. Here, matching something like the "https" in a URL would emit a
    /// bogus import, so only the positions where XAML resolves a prefix are considered.
    /// </summary>
    private static IEnumerable<string> UsedPrefixes(string xaml)
    {
        HashSet<string> prefixes = new(StringComparer.Ordinal);

        foreach ((string prefix, _) in PrefixReferences(xaml))
        {
            prefixes.Add(prefix);
        }

        return prefixes;
    }

    /// <summary>
    /// Every "prefix:Type" the fragment resolves, paired so a caller can ask not just which
    /// namespaces are needed but what is expected to be in them.
    /// </summary>
    private static IEnumerable<(string Prefix, string Type)> PrefixReferences(string xaml)
    {
        foreach (Match match in BindingPrefixRegex().Matches(xaml))
        {
            // The pattern's alternatives each capture a prefix followed by its type, so the groups
            // are read in pairs and the one pair that participated in the match is the one filled.
            for (int group = 1; group + 1 < match.Groups.Count; group += 2)
            {
                string prefix = match.Groups[group].Value;
                if (prefix.Length > 0 && !IgnoredPrefixes.Contains(prefix))
                {
                    yield return (prefix, match.Groups[group + 1].Value);
                }
            }
        }
    }

    /// <summary>Any "prefix:" occurrence — matches winappCli's namespace-synthesis regex.</summary>
    [GeneratedRegex(@"([A-Za-z_][\w.\-]*):")]
    private static partial Regex AnyPrefixRegex();

    /// <summary>A prefix the fragment declares itself, as in &lt;StackPanel xmlns:sys="using:System"&gt;.</summary>
    [GeneratedRegex(@"xmlns:([A-Za-z_][\w.\-]*)\s*=")]
    private static partial Regex SelfDeclaredPrefixRegex();

    /// <summary>
    /// Prefix positions XAML actually resolves: element names (&lt;p:Foo, &lt;/p:Foo),
    /// attribute names (p:Foo=), markup extensions ({p:Foo}) and type references in an attribute
    /// value ("p:Foo" or 'p:Foo').
    ///
    /// Each alternative captures the prefix and the type name after it, in that order, so the
    /// groups can be read in pairs.
    ///
    /// The value branch accepts either XML quote style, and whitespace around the "=". Missing a
    /// reference here is not a harmless gap: an undetected prefix is left out of the published
    /// imports and, because it is equally invisible to the unresolved-prefix check, the fragment is
    /// published as though it bound nothing — the one way this exporter can ship XAML that does not
    /// bind on arrival.
    ///
    /// The attribute-name branch stops before the "=" rather than consuming it, so an attribute
    /// that is itself prefixed does not hide a prefixed type in its value: in
    /// x:DataType="local:Contact" both "x" and "local" have to be found.
    /// </summary>
    [GeneratedRegex(@"</?([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)|\s([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)[\w.\-]*(?=\s*=)|\{\s*([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)|=\s*[""']\s*([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)")]
    private static partial Regex BindingPrefixRegex();
}
