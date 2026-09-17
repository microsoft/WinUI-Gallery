// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Text.RegularExpressions;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// What a snippet's C# declares: the types it defines, each paired with the namespace it defines
/// them in.
///
/// This exists to answer one question — whether a XAML fragment's "local:Thing" refers to a type
/// the reader is being handed in the same sample, and if so what to call the namespace holding it
/// — so it deliberately reads only declarations, not usage. A type the code merely mentions is not
/// a type the reader receives.
///
/// The two ways to be wrong here are not equally bad. Missing a real declaration costs a sample its
/// XAML, which is the behaviour that existed before this resolution path and is safe. Reporting one
/// that is not there publishes markup with an import for a type the reader never gets, which is the
/// exact failure the exporter exists to prevent. The scanner is built to fail in the first
/// direction.
/// </summary>
internal static partial class CodeDeclarations
{
    /// <summary>
    /// Namespace reported for types a snippet declares outside any namespace. The gallery's own
    /// snippets already use this name as their stand-in (see Samples\Binding\ConverterBinding.txt
    /// and Samples\TreeView\TreeviewItemtemplateselector.txt), so a reader meets the same
    /// placeholder in the import that they meet in the code.
    /// </summary>
    public const string PlaceholderNamespace = "YourNamespace";

    /// <summary>
    /// The types <paramref name="code"/> declares at namespace or global scope, each mapped to the
    /// namespace that encloses it, or to <see cref="PlaceholderNamespace"/> when nothing does.
    ///
    /// The pairing is the point. A snippet is free to declare types in more than one namespace, and
    /// reporting a single namespace for the file would name one that does not contain the type the
    /// caller asked about.
    ///
    /// Nesting is excluded for the same reason. A type declared inside another type is reached as
    /// "Container.Item", not as "Item", so an import naming the namespace alone does not bring it
    /// into scope; reporting it would publish XAML that still cannot resolve the name it asks for.
    /// A type declared inside a method body is not reachable from XAML at all.
    ///
    /// Comments and literals are neutralised first: a snippet that talks about a class in prose —
    /// as ItemsRepeater's does about its custom layout — or that quotes markup containing the word
    /// "class" must not be read as declaring anything. Conditionally compiled regions go with them,
    /// for the same reason.
    ///
    /// A simple name declared in two namespaces is dropped rather than resolved to either. XAML
    /// asks for "local:Foo" and the snippet gives no way to say which Foo that is, so publishing
    /// one of them would be a coin toss printed as an import. Repeating a name within one namespace
    /// — a partial class split across the snippet — says nothing contradictory and is kept.
    /// </summary>
    public static Dictionary<string, string> DeclaredTypes(string? code)
    {
        Dictionary<string, string> types = new(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(code))
        {
            return types;
        }

        string stripped = BlankConditionalRegions(StripCommentsAndStrings(code));
        List<NamespaceScope> scopes = ResolveNamespaceScopes(stripped);
        int[] depths = BraceDepths(stripped);
        HashSet<string> ambiguous = new(StringComparer.Ordinal);

        foreach (Match match in TypeDeclarationRegex().Matches(stripped))
        {
            if (!IsTopLevel(scopes, depths, match.Index))
            {
                continue;
            }

            string name = match.Groups[1].Value;
            if (ambiguous.Contains(name))
            {
                continue;
            }

            string ns = NamespaceAt(scopes, match.Index);
            if (types.TryGetValue(name, out string? existing)
                && !string.Equals(existing, ns, StringComparison.Ordinal))
            {
                ambiguous.Add(name);
                types.Remove(name);
                continue;
            }

            types[name] = ns;
        }

        return types;
    }

    /// <summary>
    /// The span of source a namespace declaration governs, and whether it opened a brace to do so.
    /// A file-scoped namespace contributes no nesting, so the two forms cannot be told apart by
    /// brace depth alone.
    /// </summary>
    private readonly record struct NamespaceScope(string Name, int Start, int End, bool IsBlockScoped);

    /// <summary>
    /// Locates each namespace declaration and the region it covers: up to the closing brace for a
    /// block-scoped one, to the end of the file for a file-scoped one.
    ///
    /// Tracking the extent rather than just the position matters for a snippet that closes a
    /// namespace and then declares something after it; taking "the nearest declaration above" would
    /// put that type in a namespace it has already left.
    /// </summary>
    private static List<NamespaceScope> ResolveNamespaceScopes(string code)
    {
        List<NamespaceScope> scopes = [];

        foreach (Match match in NamespaceDeclarationRegex().Matches(code))
        {
            int cursor = match.Index + match.Length;
            while (cursor < code.Length && char.IsWhiteSpace(code[cursor]))
            {
                cursor++;
            }

            if (cursor >= code.Length)
            {
                continue;
            }

            if (code[cursor] == ';')
            {
                scopes.Add(new NamespaceScope(match.Groups[1].Value, cursor, code.Length, IsBlockScoped: false));
            }
            else if (code[cursor] == '{')
            {
                scopes.Add(new NamespaceScope(match.Groups[1].Value, cursor, EndOfBlock(code, cursor), IsBlockScoped: true));
            }
        }

        return scopes;
    }

    /// <summary>
    /// The brace nesting in effect at each index, where a closing brace takes effect at its own
    /// position and an opening one only after it.
    ///
    /// Comments and literals are already blanked by the time this runs, so every brace it counts is
    /// a real one.
    /// </summary>
    private static int[] BraceDepths(string code)
    {
        int[] depths = new int[code.Length];
        int depth = 0;

        for (int index = 0; index < code.Length; index++)
        {
            if (code[index] == '}' && depth > 0)
            {
                depth--;
            }

            depths[index] = depth;

            if (code[index] == '{')
            {
                depth++;
            }
        }

        return depths;
    }

    /// <summary>
    /// True when the declaration at <paramref name="index"/> sits directly in a namespace or at
    /// global scope rather than inside another type or a member body.
    ///
    /// The test is that its brace depth accounts for nothing but the block-scoped namespaces
    /// around it. Anything deeper is enclosed by something the import cannot name: "N.Container.Item"
    /// is not reachable as "Item" under "using:N", and a type declared in a method body is not
    /// reachable at all, so in both cases the snippet does not hand the reader the type its XAML
    /// asks for.
    /// </summary>
    private static bool IsTopLevel(List<NamespaceScope> scopes, int[] depths, int index)
    {
        int enclosingNamespaces = scopes.Count(
            scope => scope.IsBlockScoped && index > scope.Start && index < scope.End);

        return depths[index] == enclosingNamespaces;
    }

    /// <summary>
    /// Index of the brace closing the one at <paramref name="open"/>, or the end of the string when
    /// the source is truncated mid-block.
    /// </summary>
    private static int EndOfBlock(string code, int open)
    {
        int depth = 0;

        for (int index = open; index < code.Length; index++)
        {
            if (code[index] == '{')
            {
                depth++;
            }
            else if (code[index] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return index;
                }
            }
        }

        return code.Length;
    }

    /// <summary>
    /// The namespace in effect at <paramref name="index"/>. Nested declarations are joined outward
    /// in, so a type inside "namespace A { namespace B { ... } }" reports "A.B" rather than just the
    /// innermost half.
    /// </summary>
    private static string NamespaceAt(List<NamespaceScope> scopes, int index)
    {
        List<string> enclosing =
        [
            .. scopes
                .Where(scope => index > scope.Start && index < scope.End)
                .OrderBy(scope => scope.Start)
                .Select(scope => scope.Name)
        ];

        return enclosing.Count == 0 ? PlaceholderNamespace : string.Join('.', enclosing);
    }

    /// <summary>
    /// Replaces every conditional-compilation region, and the directives bounding it, with blanks.
    ///
    /// Whether such a region reaches the compiler depends on symbols the exporter cannot see, so a
    /// type declared inside one is not a type the reader is guaranteed to receive. Counting it
    /// would let the exporter publish an import for something the compiler drops — the
    /// false-positive direction this scanner is built to avoid — while ignoring a region that would
    /// in fact have compiled costs the sample nothing worse than its XAML.
    ///
    /// Only #if, #elif, #else and #endif are treated this way. #region, #pragma and the rest do not
    /// decide whether code exists, so the code around them is read normally.
    /// </summary>
    private static string BlankConditionalRegions(string code)
    {
        StringBuilder result = new(code.Length);
        int depth = 0;
        int index = 0;

        while (index < code.Length)
        {
            int newline = code.IndexOf('\n', index);
            int end = newline < 0 ? code.Length : newline + 1;
            string line = code[index..end];
            Match directive = ConditionalDirectiveRegex().Match(line);

            if (directive.Success)
            {
                if (directive.Groups[1].Value == "if")
                {
                    depth++;
                }

                result.Append(BlankLine(line));

                if (directive.Groups[1].Value == "endif" && depth > 0)
                {
                    depth--;
                }
            }
            else
            {
                result.Append(depth > 0 ? BlankLine(line) : line);
            }

            index = end;
        }

        return result.ToString();
    }

    /// <summary>
    /// The line with everything but its ending replaced by spaces, so that the text drops out
    /// without the offsets of anything after it moving.
    /// </summary>
    private static string BlankLine(string line)
    {
        StringBuilder blanked = new(line.Length);

        foreach (char character in line)
        {
            blanked.Append(character is '\r' or '\n' ? character : ' ');
        }

        return blanked.ToString();
    }

    /// <summary>
    /// Replaces every comment and literal with a single space, preserving the newlines inside it.
    ///
    /// A space rather than nothing, because the compiler treats a comment as whitespace and so it
    /// separates tokens: deleting one outright welds "class/* which */Widget" into "classWidget"
    /// and loses a declaration that is really there.
    /// </summary>
    private static string StripCommentsAndStrings(string code)
    {
        StringBuilder result = new(code.Length);
        int index = 0;

        while (index < code.Length)
        {
            char current = code[index];

            if (current == '/' && index + 1 < code.Length && code[index + 1] == '/')
            {
                index = Blank(code, index, LineCommentEnd(code, index), result);
            }
            else if (current == '/' && index + 1 < code.Length && code[index + 1] == '*')
            {
                index = Blank(code, index, BlockCommentEnd(code, index + 2), result);
            }
            else if (current == '"' && QuoteRunLength(code, index) >= 3)
            {
                index = Blank(code, index, RawStringEnd(code, index), result);
            }
            else if (current == '@' && index + 1 < code.Length && code[index + 1] == '"')
            {
                index = Blank(code, index, VerbatimStringEnd(code, index + 2), result);
            }
            else if (current is '"' or '\'')
            {
                index = Blank(code, index, QuotedEnd(code, index + 1, current), result);
            }
            else
            {
                result.Append(current);
                index++;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Emits a space for the run [<paramref name="start"/>, <paramref name="end"/>) plus any
    /// newlines it contained, and returns <paramref name="end"/> so the caller advances past it.
    ///
    /// Every caller passes an end strictly greater than its start, which is what guarantees the
    /// scan in <see cref="StripCommentsAndStrings"/> always moves forward.
    /// </summary>
    private static int Blank(string code, int start, int end, StringBuilder result)
    {
        result.Append(' ');

        for (int index = start; index < end; index++)
        {
            if (code[index] == '\n')
            {
                result.Append('\n');
            }
        }

        return end;
    }

    private static int LineCommentEnd(string code, int start)
    {
        int end = code.IndexOf('\n', start);
        return end < 0 ? code.Length : end;
    }

    private static int BlockCommentEnd(string code, int start)
    {
        int end = code.IndexOf("*/", start, StringComparison.Ordinal);
        return end < 0 ? code.Length : end + 2;
    }

    /// <summary>Length of the run of double quotes starting at <paramref name="start"/>.</summary>
    private static int QuoteRunLength(string code, int start)
    {
        int length = 0;
        while (start + length < code.Length && code[start + length] == '"')
        {
            length++;
        }

        return length;
    }

    /// <summary>
    /// End of a raw string literal. Its closing delimiter is a run of at least as many quotes as
    /// the opening one, which is precisely how a raw literal is able to contain quotes of its own —
    /// so a shorter run inside it is content and must not end the scan.
    ///
    /// Getting this wrong is the worst case in the file: raw literals in these snippets hold markup
    /// or code, so a scan that walks off the delimiter lands in text that reads exactly like a
    /// declaration and invents one.
    /// </summary>
    private static int RawStringEnd(string code, int start)
    {
        int opening = QuoteRunLength(code, start);

        for (int index = start + opening; index < code.Length; index++)
        {
            if (code[index] != '"')
            {
                continue;
            }

            int run = QuoteRunLength(code, index);
            if (run >= opening)
            {
                return index + run;
            }

            index += run - 1;
        }

        return code.Length;
    }

    /// <summary>End of a verbatim string, whose only escape is a doubled quote.</summary>
    private static int VerbatimStringEnd(string code, int start)
    {
        int index = start;

        while (index < code.Length)
        {
            if (code[index] != '"')
            {
                index++;
                continue;
            }

            if (index + 1 < code.Length && code[index + 1] == '"')
            {
                index += 2;
                continue;
            }

            return index + 1;
        }

        return code.Length;
    }

    /// <summary>End of a regular string or char literal, honouring backslash escapes.</summary>
    private static int QuotedEnd(string code, int start, char quote)
    {
        int index = start;

        while (index < code.Length)
        {
            char current = code[index];

            if (current == '\\')
            {
                index += 2;
                continue;
            }

            if (current == quote)
            {
                return index + 1;
            }

            // An unterminated literal would otherwise swallow the rest of the file; a newline ends
            // it, which is also what the compiler does.
            if (current == '\n')
            {
                return index;
            }

            index++;
        }

        return code.Length;
    }

    /// <summary>
    /// A type declaration, as in "public sealed class Foo" or "record struct Bar". The keyword is
    /// required to start a token so that an identifier ending in one — "MyClass" — is not read as
    /// a declaration.
    ///
    /// "record" is matched with an optional "class" or "struct" after it, and that longer form is
    /// tried first: without it the alternation settles for the bare "record" and captures the
    /// modifier as the type name, so "record struct Bar" would declare a type called "struct" and
    /// never mention Bar.
    ///
    /// The pattern is position-blind: it matches a nested declaration as readily as a namespace-level
    /// one, so <see cref="IsTopLevel"/> decides which of its matches count.
    /// </summary>
    [GeneratedRegex(@"\b(?:record\s+(?:class|struct)|class|struct|interface|enum|record)\s+([A-Za-z_]\w*)")]
    private static partial Regex TypeDeclarationRegex();

    /// <summary>A namespace declaration in either form: "namespace X { }" or "namespace X;".</summary>
    [GeneratedRegex(@"\bnamespace\s+([A-Za-z_][\w.]*)")]
    private static partial Regex NamespaceDeclarationRegex();

    /// <summary>
    /// A directive that decides whether the code around it exists. C# allows whitespace both before
    /// the "#" and between it and the keyword, so both are permitted here; the word boundary keeps
    /// "#region" from being read as an "#if" family member on the strength of its first letters.
    /// </summary>
    [GeneratedRegex(@"^[^\S\r\n]*#[^\S\r\n]*(if|elif|else|endif)\b")]
    private static partial Regex ConditionalDirectiveRegex();
}
