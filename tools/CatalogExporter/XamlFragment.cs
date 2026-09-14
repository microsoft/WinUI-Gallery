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

        foreach (Match match in BindingPrefixRegex().Matches(xaml))
        {
            for (int group = 1; group < match.Groups.Count; group++)
            {
                string prefix = match.Groups[group].Value;
                if (prefix.Length > 0 && !IgnoredPrefixes.Contains(prefix))
                {
                    prefixes.Add(prefix);
                }
            }
        }

        return prefixes;
    }

    /// <summary>Any "prefix:" occurrence — matches winappCli's namespace-synthesis regex.</summary>
    [GeneratedRegex(@"([A-Za-z_][\w.\-]*):")]
    private static partial Regex AnyPrefixRegex();

    /// <summary>
    /// Prefix positions XAML actually resolves: element names (&lt;p:Foo, &lt;/p:Foo),
    /// attribute names (p:Foo=), markup extensions ({p:Foo}) and type references ("p:Foo").
    ///
    /// The attribute-name branch stops before the "=" rather than consuming it, so an attribute
    /// that is itself prefixed does not hide a prefixed type in its value: in
    /// x:DataType="local:Contact" both "x" and "local" have to be found.
    /// </summary>
    [GeneratedRegex(@"</?([A-Za-z_][\w.\-]*):[A-Za-z_]|\s([A-Za-z_][\w.\-]*):[A-Za-z_][\w.\-]*(?=\s*=)|\{\s*([A-Za-z_][\w.\-]*):[A-Za-z_]|=""\s*([A-Za-z_][\w.\-]*):[A-Za-z_]")]
    private static partial Regex BindingPrefixRegex();
}
