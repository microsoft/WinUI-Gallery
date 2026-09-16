// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Text.RegularExpressions;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// What a snippet's C# declares: the types it defines and the namespace it puts them in.
///
/// This exists to answer one question — whether a XAML fragment's "local:Thing" refers to a type
/// the reader is being handed in the same sample — so it deliberately reads only declarations, not
/// usage. A type the code merely mentions is not a type the reader receives.
/// </summary>
internal static partial class CodeDeclarations
{
    /// <summary>
    /// Namespace published for types a snippet declares outside any namespace. The gallery's own
    /// snippets already use this name as their stand-in (see Samples\Binding\ConverterBinding.txt
    /// and Samples\TreeView\TreeviewItemtemplateselector.txt), so a reader meets the same
    /// placeholder in the import that they meet in the code.
    /// </summary>
    public const string PlaceholderNamespace = "YourNamespace";

    /// <summary>
    /// Names of the types <paramref name="code"/> declares.
    ///
    /// Comments and string literals are removed first: a snippet that talks about a class in prose
    /// — as ItemsRepeater's does about its custom layout — must not be read as declaring it, or a
    /// fragment referencing that type would be published as though the reader had the source.
    /// </summary>
    public static HashSet<string> DeclaredTypes(string? code)
    {
        HashSet<string> types = new(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(code))
        {
            return types;
        }

        string stripped = StripCommentsAndStrings(code);
        foreach (Match match in TypeDeclarationRegex().Matches(stripped))
        {
            types.Add(match.Groups[1].Value);
        }

        return types;
    }

    /// <summary>
    /// The first namespace <paramref name="code"/> declares, block-scoped or file-scoped, or
    /// <see cref="PlaceholderNamespace"/> when it declares none.
    /// </summary>
    public static string NamespaceOrPlaceholder(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return PlaceholderNamespace;
        }

        Match match = NamespaceDeclarationRegex().Match(StripCommentsAndStrings(code));
        return match.Success ? match.Groups[1].Value : PlaceholderNamespace;
    }

    /// <summary>
    /// Blanks out comments, string literals and char literals, preserving line structure so that
    /// what remains is still scannable by line-anchored patterns.
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
                index = SkipUntil(code, index, "\n", consumeTerminator: false, result);
            }
            else if (current == '/' && index + 1 < code.Length && code[index + 1] == '*')
            {
                index = SkipUntil(code, index + 2, "*/", consumeTerminator: true, result);
            }
            else if (current == '@' && index + 1 < code.Length && code[index + 1] == '"')
            {
                index = SkipVerbatimString(code, index + 2, result);
            }
            else if (current is '"' or '\'')
            {
                index = SkipQuoted(code, index + 1, current, result);
            }
            else
            {
                result.Append(current);
                index++;
            }
        }

        return result.ToString();
    }

    /// <summary>Blanks the run up to <paramref name="terminator"/>, keeping any newlines in it.</summary>
    private static int SkipUntil(string code, int start, string terminator, bool consumeTerminator, StringBuilder result)
    {
        int end = code.IndexOf(terminator, start, StringComparison.Ordinal);
        int stop = end < 0 ? code.Length : consumeTerminator ? end + terminator.Length : end;

        AppendNewlines(code, start, stop, result);
        return stop;
    }

    /// <summary>Blanks a verbatim string, whose only escape is a doubled quote.</summary>
    private static int SkipVerbatimString(string code, int start, StringBuilder result)
    {
        int index = start;
        while (index < code.Length)
        {
            if (code[index] == '"')
            {
                if (index + 1 < code.Length && code[index + 1] == '"')
                {
                    index += 2;
                    continue;
                }

                index++;
                break;
            }

            if (code[index] == '\n')
            {
                result.Append('\n');
            }

            index++;
        }

        return index;
    }

    /// <summary>Blanks a regular string or char literal, honouring backslash escapes.</summary>
    private static int SkipQuoted(string code, int start, char quote, StringBuilder result)
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
                index++;
                break;
            }

            // An unterminated literal would otherwise swallow the rest of the file; a newline ends
            // it, which is also what the compiler does.
            if (current == '\n')
            {
                result.Append('\n');
                index++;
                break;
            }

            index++;
        }

        return index;
    }

    private static void AppendNewlines(string code, int start, int end, StringBuilder result)
    {
        for (int index = start; index < end; index++)
        {
            if (code[index] == '\n')
            {
                result.Append('\n');
            }
        }
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
    /// </summary>
    [GeneratedRegex(@"\b(?:record\s+(?:class|struct)|class|struct|interface|enum|record)\s+([A-Za-z_]\w*)")]
    private static partial Regex TypeDeclarationRegex();

    /// <summary>A namespace declaration in either form: "namespace X { }" or "namespace X;".</summary>
    [GeneratedRegex(@"\bnamespace\s+([A-Za-z_][\w.]*)")]
    private static partial Regex NamespaceDeclarationRegex();
}
