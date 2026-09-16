// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Resolves the <c>$(Token)</c> placeholders that snippet bundles contain.
/// </summary>
/// <remarks>
/// <para>
/// A sample page can pair a snippet with a set of <c>ControlExampleSubstitution</c> entries, each
/// binding a token to a live option control:
/// <code>
/// &lt;controls:ControlExampleSubstitution Key="Spacing" Value="{x:Bind SpacingSlider.Value, Mode=OneWay}" /&gt;
/// </code>
/// In the running app the token is replaced as the user moves the slider, so a reader never sees
/// the raw <c>$(Spacing)</c>. Published verbatim, though, that same snippet is not valid XAML and
/// cannot be pasted into a project.
/// </para>
/// <para>
/// This resolver substitutes the value the control starts with, which is exactly what the gallery
/// renders when the page first loads. It is deliberately conservative: a token is replaced only
/// when the initial value can be read directly out of the markup. Anything that depends on running
/// code - converter functions such as <c>BoolToLowerString(x.IsOn)</c>, or a control that never
/// declares an initial value - is left as the original token rather than guessed at, because
/// publishing a value the gallery does not actually show would be worse than publishing none.
/// </para>
/// </remarks>
internal static class SubstitutionResolver
{
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>Matches ((SomeType)elementName.SelectedItem).Member.</summary>
    private static readonly Regex CastedSelectionRegex = new(
        @"^\(\([^)]+\)(?<element>[A-Za-z_][\w]*)\.(?<property>[A-Za-z_][\w]*)\)\.(?<member>[A-Za-z_][\w]*)$",
        RegexOptions.Compiled);

    /// <summary>
    /// Builds the token map for one <c>ControlExample</c>. Keys are token names without the
    /// <c>$( )</c> wrapper; only confidently resolved tokens are present.
    /// </summary>
    public static Dictionary<string, string> BuildMap(XElement controlExample, XElement pageRoot)
    {
        Dictionary<string, string> map = new(StringComparer.Ordinal);

        XElement? container = controlExample
            .Elements()
            .FirstOrDefault(e => e.Name.LocalName == "ControlExample.Substitutions");

        if (container is null)
        {
            return map;
        }

        foreach (XElement substitution in container.Elements().Where(e => e.Name.LocalName == "ControlExampleSubstitution"))
        {
            string? key = (string?)substitution.Attribute("Key");
            string? value = (string?)substitution.Attribute("Value");
            if (string.IsNullOrEmpty(key) || value is null)
            {
                continue;
            }

            // A disabled substitution renders as the empty string, not as its value, so the
            // condition has to be settled before the value matters. When it cannot be settled the
            // token is left alone: guessing wrong would either invent markup the gallery omits or
            // drop markup it shows.
            bool? enabled = ResolveIsEnabled(substitution, pageRoot);
            if (enabled is null)
            {
                continue;
            }

            if (enabled == false)
            {
                map[key] = string.Empty;
                continue;
            }

            string? resolved = ResolveValue(value, pageRoot);
            if (resolved is not null)
            {
                map[key] = resolved;
            }
        }

        return map;
    }

    /// <summary>
    /// Evaluates a substitution's <c>IsEnabled</c> condition. Absent means enabled; null means the
    /// condition depends on state this exporter cannot read.
    /// </summary>
    private static bool? ResolveIsEnabled(XElement substitution, XElement pageRoot)
    {
        string? raw = (string?)substitution.Attribute("IsEnabled");
        if (raw is null)
        {
            return true;
        }

        string? resolved = ResolveValue(raw, pageRoot);
        if (bool.TryParse(resolved?.Trim(), out bool enabled))
        {
            return enabled;
        }

        return ResolveUnsetBooleanDefault(raw, pageRoot);
    }

    /// <summary>
    /// Boolean properties whose documented default is false, so markup that never sets one is
    /// showing it as false rather than leaving it unknown.
    ///
    /// The list is explicit because the opposite case is common enough to matter: IsEnabled,
    /// IsTabStop and IsHitTestVisible all default to true, so a blanket "unset means false" rule
    /// would invert them.
    /// </summary>
    private static readonly HashSet<string> DefaultFalseBooleans = new(StringComparer.Ordinal)
    {
        "IsChecked", "IsOn", "IsSticky", "IsOpen",
    };

    /// <summary>
    /// Resolves a gate like <c>{x:Bind DisableButton.IsChecked.Value}</c> against a CheckBox that
    /// never sets IsChecked. The control starts unchecked, so the gallery renders that
    /// substitution as an empty string on load — and because these gates typically supply a whole
    /// attribute, leaving the token in place produces XAML that is not well-formed and gets
    /// discarded downstream. Resolving it is therefore both more accurate and what keeps the
    /// sample publishable.
    /// </summary>
    private static bool? ResolveUnsetBooleanDefault(string binding, XElement pageRoot)
    {
        string trimmed = binding.Trim();
        if (!trimmed.StartsWith("{x:Bind", StringComparison.Ordinal) || !trimmed.EndsWith('}'))
        {
            return null;
        }

        string body = trimmed[7..^1].Trim();
        int comma = body.IndexOf(',');
        string path = (comma < 0 ? body : body[..comma]).Trim();

        // A converter or cast depends on code this exporter does not run.
        if (path.Contains('(') || path.Contains(')'))
        {
            return null;
        }

        string[] segments = path.Split('.');
        if (segments.Length is < 2 or > 3)
        {
            return null;
        }

        // A nullable bool reads as IsChecked.Value in x:Bind but is still the IsChecked attribute.
        if (segments.Length == 3 && segments[2] != "Value")
        {
            return null;
        }

        if (!DefaultFalseBooleans.Contains(segments[1]))
        {
            return null;
        }

        XElement? target = FindNamedElement(pageRoot, segments[0]);
        if (target is null)
        {
            return null;
        }

        // Only an absent attribute means "left at its default". An attribute that is present but
        // did not resolve above is a binding of its own, and stays unknown.
        return target.Attribute(segments[1]) is null ? false : null;
    }

    /// <summary>
    /// Replaces every <c>$(Token)</c> in <paramref name="text"/> that appears in
    /// <paramref name="map"/>. Tokens with no entry are left exactly as they were.
    /// </summary>
    public static string Apply(string text, IReadOnlyDictionary<string, string> map)
    {
        if (map.Count == 0 || string.IsNullOrEmpty(text))
        {
            return text;
        }

        StringBuilder result = new(text.Length);
        int index = 0;

        while (index < text.Length)
        {
            int start = text.IndexOf("$(", index, StringComparison.Ordinal);
            if (start < 0)
            {
                result.Append(text, index, text.Length - index);
                break;
            }

            int end = text.IndexOf(')', start + 2);
            if (end < 0)
            {
                result.Append(text, index, text.Length - index);
                break;
            }

            string token = text[(start + 2)..end];
            result.Append(text, index, start - index);
            result.Append(map.TryGetValue(token, out string? replacement) ? replacement : text[start..(end + 1)]);
            index = end + 1;
        }

        return result.ToString();
    }

    /// <summary>
    /// Turns a substitution's <c>Value</c> into a literal, or returns null when it cannot be
    /// determined without running the app.
    /// </summary>
    private static string? ResolveValue(string value, XElement pageRoot)
    {
        // A plain literal needs no lookup. Its surrounding whitespace is significant: several
        // snippets rely on a value like ' IsSticky="True" ' supplying its own separating spaces.
        string trimmed = value.Trim();
        if (!trimmed.StartsWith('{'))
        {
            return value;
        }

        if (!trimmed.StartsWith("{x:Bind", StringComparison.Ordinal) || !trimmed.EndsWith('}'))
        {
            return null;
        }

        string body = trimmed[7..^1].Trim();
        int comma = body.IndexOf(',');
        string path = (comma < 0 ? body : body[..comma]).Trim();

        // A cast around a selector, ((ComboBoxItem)combo.SelectedItem).Content, is still just a
        // selection lookup once the cast is stripped, so it resolves like any other.
        Match cast = CastedSelectionRegex.Match(path);
        if (cast.Success)
        {
            XElement? selector = FindNamedElement(pageRoot, cast.Groups["element"].Value);
            XElement? item = selector is null ? null : FindSelectedItem(selector);
            if (item is null)
            {
                return null;
            }

            string member = cast.Groups["member"].Value;
            return member == "Content" ? ReadItemContent(item) : (string?)item.Attribute(member);
        }

        // Converter functions - BoolToLowerString(x.IsOn) - depend on code this exporter does not run.
        if (path.Contains('(') || path.Contains(')'))
        {
            return null;
        }

        string[] segments = path.Split('.');
        if (segments.Length < 2)
        {
            return null;
        }

        XElement? target = FindNamedElement(pageRoot, segments[0]);
        if (target is null)
        {
            return null;
        }

        return ResolveProperty(target, segments[1..]);
    }

    private static string? ResolveProperty(XElement target, string[] propertyPath)
    {
        string property = propertyPath[0];

        // A nullable bool reads as IsChecked.Value in x:Bind but is still the IsChecked attribute.
        if (propertyPath.Length == 2 && propertyPath[1] == "Value")
        {
            return (string?)target.Attribute(property);
        }

        if (propertyPath.Length > 1)
        {
            // Sub-properties such as BorderThickness.Top would need XAML type conversion to split
            // reliably, so they are left unresolved.
            return null;
        }

        if (property is "SelectedItem" or "SelectedValue")
        {
            return ResolveSelectedItem(target);
        }

        return (string?)target.Attribute(property);
    }

    /// <summary>
    /// Finds the initially selected item of a selector and returns its displayed text. Requires an
    /// explicit selection: a selector that declares none starts empty, and assuming the first item
    /// would publish something the gallery does not show.
    /// </summary>
    private static string? ResolveSelectedItem(XElement selector)
    {
        XElement? selected = FindSelectedItem(selector);
        return selected is null ? null : ReadItemContent(selected);
    }

    private static XElement? FindSelectedItem(XElement selector)
    {
        List<XElement> items = selector
            .Elements()
            .Where(e => !e.Name.LocalName.Contains('.'))
            .ToList();

        XElement? selected = items.FirstOrDefault(e => string.Equals((string?)e.Attribute("IsSelected"), "True", StringComparison.OrdinalIgnoreCase));
        if (selected is not null)
        {
            return selected;
        }

        string? selectedIndex = (string?)selector.Attribute("SelectedIndex");
        if (!int.TryParse(selectedIndex, out int index) || index < 0 || index >= items.Count)
        {
            return null;
        }

        return items[index];
    }

    private static string? ReadItemContent(XElement item)
    {
        string? content = (string?)item.Attribute("Content");
        if (!string.IsNullOrEmpty(content))
        {
            return content;
        }

        // <ComboBoxItem>Text</ComboBoxItem> puts the content in the element body instead.
        return item.HasElements ? null : NullIfWhiteSpace(item.Value.Trim());
    }

    private static XElement? FindNamedElement(XElement root, string name) =>
        root.Descendants().FirstOrDefault(e =>
            (string?)e.Attribute(XamlNamespace + "Name") == name || (string?)e.Attribute("Name") == name);

    private static string? NullIfWhiteSpace(string value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
