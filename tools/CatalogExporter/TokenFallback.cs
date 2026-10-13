// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Text.RegularExpressions;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// The last step between a snippet and the index: removes any <c>$(Token)</c> that
/// <see cref="SubstitutionResolver"/> could not resolve.
/// </summary>
/// <remarks>
/// <para>
/// The resolver is deliberately conservative and leaves a token alone rather than guess at it. That
/// is the right call for accuracy, but a published token is not something a consumer can paste: it
/// is not a valid value for the property it sits on, and an agent reading the index has no way to
/// know it is a placeholder rather than literal text.
/// </para>
/// <para>
/// So whatever survives resolution is deleted here, by removing the smallest construct that keeps
/// the fragment valid: the enclosing attribute when the token sits in one, otherwise just the token
/// text. Deleting an attribute is a safe degradation because the property then falls back to its own
/// default - which is exactly what the gallery itself is showing whenever the token was unresolvable
/// for the usual reason, namely that the option control it binds to was never given an initial value.
/// </para>
/// <para>
/// This is intentionally a fallback and not a resolution strategy. It runs only on what the resolver
/// declined to settle, and it is what lets the exporter guarantee that no published XAML contains a
/// placeholder, without having to infer a value it cannot actually know.
/// </para>
/// </remarks>
internal static partial class TokenFallback
{
    /// <summary>
    /// Marks a deletion point so the line-level cleanup can tell a line that a removal emptied from
    /// one that was already blank. A private-use code point, so it cannot collide with snippet text.
    /// </summary>
    private const char Marker = '\uE000';

    /// <summary>True when <paramref name="text"/> still contains a <c>$(Token)</c> placeholder.</summary>
    public static bool ContainsToken(string? text) => text is not null && TokenRegex().IsMatch(text);

    /// <summary>Token names still present in <paramref name="text"/>, in order of first appearance.</summary>
    public static List<string> TokenNames(string text)
    {
        List<string> names = [];
        foreach (Match match in TokenRegex().Matches(text))
        {
            string name = match.Groups["name"].Value;
            if (!names.Contains(name, StringComparer.Ordinal))
            {
                names.Add(name);
            }
        }

        return names;
    }

    /// <summary>
    /// Removes every unresolved token from a XAML fragment, deleting the attribute that contains it
    /// when there is one. A fragment with no tokens is returned unchanged.
    /// </summary>
    public static string StripFromXaml(string xaml)
    {
        if (!ContainsToken(xaml))
        {
            return xaml;
        }

        return ApplyRemovals(xaml, CollectXamlRemovals(xaml));
    }

    /// <summary>
    /// Walks the fragment and records what to delete for each token: the whole attribute when the
    /// token is inside an attribute value, otherwise the token text on its own.
    /// </summary>
    private static List<(int Start, int Length)> CollectXamlRemovals(string xaml)
    {
        List<(int Start, int Length)> removals = [];
        int index = 0;

        while (index < xaml.Length)
        {
            if (StartsWith(xaml, index, "<!--"))
            {
                int close = xaml.IndexOf("-->", index + 4, StringComparison.Ordinal);
                int commentEnd = close < 0 ? xaml.Length : close + 3;

                // A token inside a comment breaks nothing, but leaving it would still publish a
                // placeholder, so the token text goes and the surrounding prose stays.
                AddTokenRemovals(xaml, index, commentEnd, removals);
                index = commentEnd;
                continue;
            }

            if (xaml[index] == '<')
            {
                index = ScanTag(xaml, index, removals);
                continue;
            }

            int nextTag = xaml.IndexOf('<', index);
            int textEnd = nextTag < 0 ? xaml.Length : nextTag;
            AddTokenRemovals(xaml, index, textEnd, removals);
            index = textEnd;
        }

        return removals;
    }

    /// <summary>
    /// Scans one tag, recording a removal for each attribute whose value contains a token, and
    /// returns the index just past the tag.
    /// </summary>
    private static int ScanTag(string xaml, int tagStart, List<(int Start, int Length)> removals)
    {
        int index = tagStart + 1;

        // The element name, which cannot contain an attribute and so is skipped wholesale.
        while (index < xaml.Length && xaml[index] != '>' && !char.IsWhiteSpace(xaml[index]))
        {
            index++;
        }

        while (index < xaml.Length && xaml[index] != '>')
        {
            if (char.IsWhiteSpace(xaml[index]) || xaml[index] == '/')
            {
                index++;
                continue;
            }

            int nameStart = index;
            while (index < xaml.Length && xaml[index] != '=' && xaml[index] != '>' && xaml[index] != '/' && !char.IsWhiteSpace(xaml[index]))
            {
                index++;
            }

            int cursor = SkipWhiteSpace(xaml, index);
            if (cursor >= xaml.Length || xaml[cursor] != '=')
            {
                // Not "name=value". A token standing where an attribute name belongs is a
                // substitution that expands to whole attributes, so the token itself is the smallest
                // construct that can be deleted. Left in place it is not even well-formed XML.
                if (xaml.AsSpan(nameStart, index - nameStart).Contains("$(", StringComparison.Ordinal))
                {
                    AddRemoval(xaml, nameStart, index, removals);
                }

                index = cursor;
                continue;
            }

            cursor = SkipWhiteSpace(xaml, cursor + 1);
            if (cursor >= xaml.Length || (xaml[cursor] != '"' && xaml[cursor] != '\''))
            {
                index = cursor;
                continue;
            }

            char quote = xaml[cursor];
            int valueStart = cursor + 1;
            int valueEnd = xaml.IndexOf(quote, valueStart);
            if (valueEnd < 0)
            {
                // Unterminated attribute: the fragment is malformed and is rejected elsewhere.
                return xaml.Length;
            }

            if (xaml.AsSpan(valueStart, valueEnd - valueStart).Contains("$(", StringComparison.Ordinal))
            {
                AddRemoval(xaml, nameStart, valueEnd + 1, removals);
            }

            index = valueEnd + 1;
        }

        return index < xaml.Length ? index + 1 : xaml.Length;
    }

    /// <summary>
    /// Records the removal of the span [<paramref name="start"/>, <paramref name="end"/>) along with
    /// the whitespace in front of it.
    /// </summary>
    private static void AddRemoval(string xaml, int start, int end, List<(int Start, int Length)> removals)
    {
        // The whitespace separating this attribute from whatever precedes it goes with it, newlines
        // included. Taking it from the front rather than the back is what keeps the result tidy: an
        // attribute written on its own line takes that line with it, and the last attribute in a tag
        // does not leave the closing "/>" stranded.
        while (start > 0 && char.IsWhiteSpace(xaml[start - 1]))
        {
            start--;
        }

        removals.Add((start, end - start));
    }

    /// <summary>Records a removal for each token in <paramref name="text"/> between two offsets.</summary>
    private static void AddTokenRemovals(string text, int start, int end, List<(int Start, int Length)> removals)
    {
        foreach (Match match in TokenRegex().Matches(text[start..end]))
        {
            removals.Add((start + match.Index, match.Length));
        }
    }

    private static int SkipWhiteSpace(string text, int index)
    {
        while (index < text.Length && char.IsWhiteSpace(text[index]))
        {
            index++;
        }

        return index;
    }

    private static bool StartsWith(string text, int index, string value) =>
        index + value.Length <= text.Length && text.AsSpan(index, value.Length).SequenceEqual(value);

    /// <summary>
    /// Cuts the recorded spans out of <paramref name="text"/> and tidies the result: a line that a
    /// removal emptied is dropped rather than left as whitespace, and a line a removal shortened
    /// loses any trailing whitespace it gained.
    /// </summary>
    private static string ApplyRemovals(string text, List<(int Start, int Length)> removals)
    {
        if (removals.Count == 0)
        {
            return text;
        }

        removals.Sort(static (left, right) => left.Start.CompareTo(right.Start));

        StringBuilder builder = new(text.Length);
        int index = 0;

        foreach ((int start, int length) in removals)
        {
            // Two tokens in one attribute value produce two removals covering the same span.
            if (start < index)
            {
                continue;
            }

            builder.Append(text, index, start - index);
            builder.Append(Marker);
            index = start + length;
        }

        builder.Append(text, index, text.Length - index);
        return CleanLines(builder.ToString());
    }

    private static string CleanLines(string text)
    {
        List<string> kept = [];

        foreach (string line in text.Split('\n'))
        {
            if (!line.Contains(Marker))
            {
                kept.Add(line);
                continue;
            }

            string cleaned = line.Replace(Marker.ToString(), string.Empty);

            // The line held nothing but the removed attribute, so the blank it left goes too.
            if (cleaned.Trim().Length == 0)
            {
                continue;
            }

            kept.Add(TrimTrailingSpaces(cleaned));
        }

        return string.Join('\n', kept);
    }

    /// <summary>
    /// Trims spaces and tabs from the end of a line, leaving any carriage return in place so that
    /// CRLF snippets keep their line endings.
    /// </summary>
    private static string TrimTrailingSpaces(string line)
    {
        int end = line.Length;
        bool carriageReturn = end > 0 && line[end - 1] == '\r';
        if (carriageReturn)
        {
            end--;
        }

        while (end > 0 && (line[end - 1] == ' ' || line[end - 1] == '\t'))
        {
            end--;
        }

        return line[..end] + (carriageReturn ? "\r" : string.Empty);
    }

    /// <summary>A <c>$(Token)</c> placeholder, capturing the token name.</summary>
    [GeneratedRegex(@"\$\((?<name>[^)]*)\)")]
    private static partial Regex TokenRegex();
}
