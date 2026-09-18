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
    ///
    /// Three things are read. Element and attribute names are a pattern match, because a prefix
    /// there is delimited by the markup itself. Attribute values are located by scanning the tag
    /// structure, so that a quote in text content cannot be mistaken for one that opens a value.
    /// The values themselves are parsed, because a value holding a markup extension is written in a
    /// grammar of its own: arguments separated by commas, named or positional, quoted or bare,
    /// nesting freely.
    /// </summary>
    private static IEnumerable<(string Prefix, string Type)> PrefixReferences(string xaml)
    {
        foreach (Match match in NameReferenceRegex().Matches(xaml))
        {
            foreach ((string Prefix, string Type) reference in PairedCaptures(match))
            {
                yield return reference;
            }
        }

        List<(int Start, int End)> values = [];

        foreach ((int Start, int End, bool IsNamespaceDeclaration) span in AttributeValueSpans(xaml))
        {
            values.Add((span.Start, span.End));

            // An xmlns value is a namespace URI, not a type reference. "clr-namespace:Contoso"
            // reads as a "prefix:Type" whose prefix nothing declares, and withholding a fragment
            // over it would be withholding it over the very declaration that binds its markup.
            if (span.IsNamespaceDeclaration)
            {
                continue;
            }

            foreach ((string Prefix, string Type) reference in ValueReferences(xaml[span.Start..span.End]))
            {
                yield return reference;
            }
        }

        // An extension written outside an attribute value — in a snippet missing a quote, or one
        // that opens mid-element — would otherwise take every reference inside it out of sight.
        // Missing a reference is the one failure that ships XAML which does not bind, so a brace no
        // attribute accounts for is still read as an extension.
        int index = 0;

        while (index < xaml.Length)
        {
            int brace = xaml.IndexOf('{', index);
            if (brace < 0)
            {
                break;
            }

            if (values.Any(value => brace >= value.Start && brace < value.End))
            {
                index = brace + 1;
                continue;
            }

            List<(string Prefix, string Type)> found = [];
            index = ReadExtension(xaml, brace, found);

            foreach ((string Prefix, string Type) reference in Resolvable(found))
            {
                yield return reference;
            }
        }
    }

    /// <summary>
    /// The span of every attribute value in the fragment, as offsets into it.
    ///
    /// Finding these with a pattern over the whole fragment was wrong in a way that mattered.
    /// Matching "=" followed by a quoted run pairs quotes wherever it meets them, so a stray quote
    /// in text content — "&lt;TextBlock&gt;x = " y&lt;/TextBlock&gt;" — pairs with the opening quote
    /// of the next real attribute and swallows its value. The value was then read by nothing: it is
    /// not a name, and it holds no brace for the extension sweep to find. A reference lost that way
    /// is published without its import, which is the one failure this file exists to prevent.
    ///
    /// Walking the tag structure instead removes the ambiguity rather than narrowing it. A quote is
    /// only a delimiter inside a start tag, so text, comments, CDATA and processing instructions are
    /// stepped over as the units they are and never offer a quote to pair with.
    ///
    /// Each span says whether its attribute is an xmlns declaration, because such a value holds a
    /// namespace URI rather than markup: it is still a span the extension sweep must not re-read,
    /// but nothing in it names a type.
    /// </summary>
    private static IEnumerable<(int Start, int End, bool IsNamespaceDeclaration)> AttributeValueSpans(string xaml)
    {
        int index = 0;

        while (index < xaml.Length)
        {
            if (xaml[index] != '<')
            {
                index++;
                continue;
            }

            if (Matches(xaml, index, "<!--"))
            {
                index = EndOf(xaml, index + 4, "-->");
                continue;
            }

            if (Matches(xaml, index, "<![CDATA["))
            {
                index = EndOf(xaml, index + 9, "]]>");
                continue;
            }

            if (Matches(xaml, index, "<?"))
            {
                index = EndOf(xaml, index + 2, "?>");
                continue;
            }

            index++;

            while (index < xaml.Length && xaml[index] != '>')
            {
                char current = xaml[index];

                if (current is '"' or '\'')
                {
                    int close = xaml.IndexOf(current, index + 1);
                    if (close < 0)
                    {
                        // A quote that never closes leaves no way to tell where the value ends, so
                        // the rest is treated as one. Reading too much withholds a fragment; reading
                        // nothing would publish one whose references were never looked at.
                        //
                        // What it reaches is no longer one attribute's value, so the xmlns exemption
                        // does not apply to it: that exemption says a namespace URI names no type,
                        // and everything after the quote is markup that may well name several.
                        yield return (index + 1, xaml.Length, false);
                        yield break;
                    }

                    yield return (index + 1, close, NamesNamespaceDeclaration(xaml, index));
                    index = close + 1;
                    continue;
                }

                index++;
            }

            index++;
        }
    }

    /// <summary>
    /// True when the quote at <paramref name="quote"/> opens the value of an xmlns declaration.
    ///
    /// The name is read backwards from the delimiter because that is where the scan already stands
    /// and the shape is fixed: a value is preceded by "=", and before that the attribute name. An
    /// attribute written without one is malformed, and reporting it as an ordinary value only means
    /// its contents are read — the direction that withholds a fragment rather than publishing one.
    /// </summary>
    private static bool NamesNamespaceDeclaration(string xaml, int quote)
    {
        int index = quote - 1;

        while (index >= 0 && char.IsWhiteSpace(xaml[index]))
        {
            index--;
        }

        if (index < 0 || xaml[index] != '=')
        {
            return false;
        }

        index--;

        while (index >= 0 && char.IsWhiteSpace(xaml[index]))
        {
            index--;
        }

        int end = index + 1;

        while (index >= 0 && (char.IsLetterOrDigit(xaml[index]) || xaml[index] is '_' or '.' or '-' or ':'))
        {
            index--;
        }

        ReadOnlySpan<char> name = xaml.AsSpan(index + 1, end - index - 1);

        return name.Equals("xmlns", StringComparison.Ordinal) || name.StartsWith("xmlns:", StringComparison.Ordinal);
    }

    private static bool Matches(string text, int index, string token) =>
        index + token.Length <= text.Length && string.CompareOrdinal(text, index, token, 0, token.Length) == 0;

    /// <summary>Index just past <paramref name="token"/>, or the end of the text when it is absent.</summary>
    private static int EndOf(string text, int start, string token)
    {
        int found = text.IndexOf(token, start, StringComparison.Ordinal);
        return found < 0 ? text.Length : found + token.Length;
    }

    /// <summary>
    /// The references an attribute value contributes: those of the markup extension it holds, or
    /// the types it names outright, as in x:DataType="local:Contact".
    ///
    /// A value opening with "{}" is XAML's escape for text that merely starts with a brace, so
    /// nothing in it is an extension and nothing in it resolves.
    /// </summary>
    private static IEnumerable<(string Prefix, string Type)> ValueReferences(string value)
    {
        int start = 0;
        while (start < value.Length && char.IsWhiteSpace(value[start]))
        {
            start++;
        }

        if (start < value.Length && value[start] == '{')
        {
            if (IsEscapedLiteral(value, start))
            {
                yield break;
            }

            List<(string Prefix, string Type)> found = [];
            ReadExtension(value, start, found);

            foreach ((string Prefix, string Type) reference in Resolvable(found))
            {
                yield return reference;
            }

            yield break;
        }

        // A plain value is a type reference only when it begins by naming a type; reading a QName
        // from anywhere inside one would find prefixes in prose and withhold the fragment over them.
        // Once it does begin with one, every name in it counts, because a value may name several —
        // x:TypeArguments="local:Key, local:Value" needs both, and resolving the prefix against only
        // the first would publish an import that does not contain the rest.
        if (!LeadingQualifiedNameRegex().IsMatch(value))
        {
            yield break;
        }

        foreach (Match match in QualifiedNameRegex().Matches(value))
        {
            foreach ((string Prefix, string Type) reference in PairedCaptures(match))
            {
                yield return reference;
            }
        }
    }

    /// <summary>
    /// True when the text at <paramref name="start"/> opens with XAML's "{}" escape, which marks
    /// everything after it as literal rather than as markup to resolve.
    /// </summary>
    private static bool IsEscapedLiteral(string text, int start) =>
        start + 1 < text.Length && text[start] == '{' && text[start + 1] == '}';

    /// <summary>
    /// Reads the markup extension opening at <paramref name="index"/>, collecting the qualified
    /// names it resolves, and returns the index just past its closing brace.
    ///
    /// The grammar is small: a type name, then arguments separated by commas, each either bare, or
    /// quoted, or an extension of its own, and a named argument writes "Name=" before its value.
    /// Following it rather than approximating it is what lets the cases be told apart — a nested
    /// extension has to be entered, and a quoted argument ends at its own delimiter rather than at
    /// the next comma — where a pattern over the raw body can only guess from the punctuation
    /// nearby.
    ///
    /// Argument names are read as tokens like any other. They hold no colon, so they contribute
    /// nothing, and treating them separately would buy nothing but a state to get wrong.
    /// </summary>
    private static int ReadExtension(string text, int index, List<(string Prefix, string Type)> found)
    {
        index++;

        while (index < text.Length && text[index] != '}')
        {
            char current = text[index];

            if (current == '{')
            {
                index = ReadExtension(text, index, found);
            }
            else if (current is '"' or '\'')
            {
                index = ReadQuoted(text, index, found);
            }
            else if (current is ',' or '=' || char.IsWhiteSpace(current))
            {
                index++;
            }
            else
            {
                index = ReadToken(text, index, found);
            }
        }

        return index < text.Length ? index + 1 : index;
    }

    /// <summary>
    /// Reads one bare token — an extension's type name, an argument name, or an unquoted value —
    /// and records the qualified names in it.
    ///
    /// The whole token is searched rather than just its start, because a binding path carries its
    /// references inside punctuation: "(local:Grid.Row)" names a type that has to resolve.
    ///
    /// Its one caller only enters here on a character that is none of the delimiters below, so the
    /// token is never empty and the scan always advances.
    /// </summary>
    private static int ReadToken(string text, int index, List<(string Prefix, string Type)> found)
    {
        int start = index;

        while (index < text.Length
            && text[index] is not (',' or '=' or '{' or '}' or '"' or '\'')
            && !char.IsWhiteSpace(text[index]))
        {
            index++;
        }

        foreach (Match match in QualifiedNameRegex().Matches(text[start..index]))
        {
            found.Add((match.Groups[1].Value, match.Groups[2].Value));
        }

        return index;
    }

    /// <summary>
    /// Reads a quoted argument, recording the qualified names in it, and returns the index just
    /// past the closing quote. A backslash escapes the delimiter inside one, so it is stepped over
    /// in pairs.
    ///
    /// Quoting delimits an argument; it does not make the argument literal. The value still reaches
    /// the target property's type converter, and some of those resolve names —
    /// "{Binding Path='(attached:Badge.Count)'}" needs attached imported exactly as the unquoted
    /// form does. Stepping over the contents lost those references and published the fragment
    /// without the import they needed.
    ///
    /// Which properties resolve names is not knowable here, so the contents are read the same way
    /// everywhere. That reads a name out of "StringFormat='HH:mm'" too, and the fragment is withheld
    /// over a prefix that was never one. Nothing in the text tells "HH:mm" apart from
    /// "attached:Badge", and of the two ways to be wrong only this one is safe: a withheld fragment
    /// is a sample the catalog does not carry, where a missing import is a sample that does not run.
    /// The "{}" escape remains the author's way to say a value is literal, and is honoured.
    /// </summary>
    private static int ReadQuoted(string text, int open, List<(string Prefix, string Type)> found)
    {
        char quote = text[open];
        int close = text.Length;

        for (int index = open + 1; index < text.Length; index++)
        {
            if (text[index] == '\\')
            {
                index++;
                continue;
            }

            if (text[index] == quote)
            {
                close = index;
                break;
            }
        }

        int start = open + 1;
        while (start < close && char.IsWhiteSpace(text[start]))
        {
            start++;
        }

        if (!IsEscapedLiteral(text, start))
        {
            foreach (Match match in QualifiedNameRegex().Matches(text[start..close]))
            {
                found.Add((match.Groups[1].Value, match.Groups[2].Value));
            }
        }

        return close < text.Length ? close + 1 : text.Length;
    }

    /// <summary>Drops the prefixes XAML resolves without any import of its own.</summary>
    private static IEnumerable<(string Prefix, string Type)> Resolvable(
        IEnumerable<(string Prefix, string Type)> found) =>
        found.Where(reference => !IgnoredPrefixes.Contains(reference.Prefix));

    /// <summary>
    /// Reads a match whose groups are prefix/type pairs, skipping the alternatives that did not
    /// participate and the prefixes XAML resolves without an import.
    /// </summary>
    private static IEnumerable<(string Prefix, string Type)> PairedCaptures(Match match)
    {
        for (int group = 1; group + 1 < match.Groups.Count; group += 2)
        {
            string prefix = match.Groups[group].Value;
            if (prefix.Length > 0 && !IgnoredPrefixes.Contains(prefix))
            {
                yield return (prefix, match.Groups[group + 1].Value);
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
    /// Prefix positions in the markup itself: element names (&lt;p:Foo, &lt;/p:Foo) and attribute
    /// names (p:Foo=). A prefix here is delimited by the XML around it, so a pattern reads it
    /// exactly; attribute values are a grammar of their own and are parsed instead.
    ///
    /// Each alternative captures the prefix and the type name after it, in that order, so the
    /// groups can be read in pairs.
    ///
    /// The attribute-name branch stops before the "=" rather than consuming it, so an attribute
    /// that is itself prefixed does not hide a prefixed type in its value: in
    /// x:DataType="local:Contact" both "x" and "local" have to be found.
    /// </summary>
    [GeneratedRegex(@"</?([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)|\s([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)[\w.\-]*(?=\s*=)")]
    private static partial Regex NameReferenceRegex();

    /// <summary>
    /// A "prefix:Type" opening an attribute value, as in TargetType="local:Card". Anchoring to the
    /// start is what separates a value that names types from a colon that happens to appear in prose
    /// or in a path; it gates the value rather than being the whole of the value, since a value that
    /// names one type may go on to name others.
    /// </summary>
    [GeneratedRegex(@"^\s*([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)")]
    private static partial Regex LeadingQualifiedNameRegex();

    /// <summary>
    /// A "prefix:Type" reference inside a single bare token of a markup extension.
    ///
    /// No lookbehind is needed, and none is wanted. The token has already been cut out by the
    /// parser, so everything reaching this pattern stands where XAML resolves a namespace; the
    /// colons that are punctuation — those in a quoted format string — were stepped over as a
    /// quoted argument and never arrive.
    /// </summary>
    [GeneratedRegex(@"([A-Za-z_][\w.\-]*):([A-Za-z_]\w*)")]
    private static partial Regex QualifiedNameRegex();
}
