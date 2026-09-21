// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Covers the fragment scanner at the level <see cref="CatalogGeneratorTests"/> cannot reach.
///
/// Malformed markup is checked here rather than through the generator because the generator rejects
/// a fragment that will not parse before prefix resolution ever runs. The scanner still has to
/// handle it: its salvage paths exist for exactly the input nobody anticipated, and a path that
/// silently reads nothing is how a fragment gets published without the import it needed.
/// </summary>
[TestClass]
public sealed class XamlFragmentTests
{
    /// <summary>
    /// An unclosed quote makes the rest of the fragment one span. That span is no longer a single
    /// attribute's value, so the exemption granted to xmlns values — a namespace URI names no type
    /// — must not follow it: the markup it swallowed can name plenty.
    /// </summary>
    [TestMethod]
    public void UnresolvedPrefixes_ReadsReferencesPastAnUnclosedNamespaceValue()
    {
        // The delimiter is a single quote with no partner anywhere after it, which is what forces
        // the salvage path: a stray double quote later in the fragment would pair with it instead
        // and the span would end as an ordinary value.
        string xaml =
            """
            <StackPanel xmlns:local='using:Foo>
              <TextBlock Text="{Binding Path=(attached:Badge.Count)}" />
            </StackPanel>
            """;

        List<string> unresolved = XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>());

        CollectionAssert.Contains(
            unresolved,
            "attached",
            "A reference after an unclosed xmlns value was never read, so the fragment would publish without its import.");
    }

    /// <summary>
    /// The ordinary case the exemption is for: a declaration naming a scheme other than "using" is
    /// the line that binds the prefix, not a reference to one nothing declares.
    /// </summary>
    [TestMethod]
    public void UnresolvedPrefixes_IgnoresTheSchemeInANamespaceDeclaration()
    {
        string xaml = """<StackPanel xmlns:sys="clr-namespace:System"><sys:String>Hi</sys:String></StackPanel>""";

        List<string> unresolved = XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>());

        Assert.AreEqual(0, unresolved.Count, string.Join(", ", unresolved));
    }

    /// <summary>
    /// The exemption is for xmlns itself, not for any attribute whose name merely ends in it.
    /// </summary>
    [TestMethod]
    public void UnresolvedPrefixes_StillReadsAnAttributeNamedLikeXmlns()
    {
        string xaml = """<Control myxmlns="local:Thing" />""";

        List<string> unresolved = XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>());

        CollectionAssert.Contains(unresolved, "local");
    }

    /// <summary>
    /// XML resolves a character reference before XAML reads the value, so a prefix spelled with one
    /// binds exactly as the literal spelling does. Scanning the raw text sees no colon and would
    /// publish the fragment with no import for it.
    /// </summary>
    [TestMethod]
    public void UnresolvedPrefixes_ReadsAPrefixWrittenWithACharacterReference()
    {
        string xaml = """<DataTemplate x:DataType="local&#58;Contact" />""";

        List<string> unresolved = XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>());

        CollectionAssert.Contains(
            unresolved,
            "local",
            "A prefix written as a character reference was never seen.");
    }

    /// <summary>The hexadecimal spelling of the same reference, and a named one alongside it.</summary>
    [TestMethod]
    public void UnresolvedPrefixes_ReadsAPrefixWrittenWithAHexCharacterReference()
    {
        string xaml = """<DataTemplate x:DataType="local&#x3A;Contact" Tag="a &amp; b" />""";

        CollectionAssert.Contains(
            XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>()),
            "local");
    }

    /// <summary>
    /// An unknown or unterminated reference is left standing rather than guessed at. That can only
    /// produce a reference too many, which withholds a fragment instead of publishing a broken one.
    /// </summary>
    [TestMethod]
    public void UnresolvedPrefixes_LeavesAnUnknownReferenceAlone()
    {
        string xaml = """<Control Tag="local&nosuch;Thing &# a" />""";

        List<string> unresolved = XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>());

        Assert.AreEqual(0, unresolved.Count, string.Join(", ", unresolved));
    }

    /// <summary>
    /// A bare prefixed attribute binds its namespace without naming a type — conditional XAML
    /// writes exactly that. The prefix still needs an import, but nothing in the markup says which
    /// type would satisfy it, so a class in the snippet's C# that happens to share the attribute's
    /// name must not be taken as the answer.
    /// </summary>
    [TestMethod]
    public void ResolvePrefixesFromCode_DoesNotResolveABarePrefixedAttribute()
    {
        string xaml = """<Button newExp:Background="Green" Content="Hi" />""";

        List<string> unresolved = XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>());
        CollectionAssert.Contains(unresolved, "newExp", "The prefix still has to be reported as needing an import.");

        Dictionary<string, string> resolved = XamlFragment.ResolvePrefixesFromCode(
            xaml,
            unresolved,
            "namespace Contoso;\n\npublic class Background { }");

        Assert.AreEqual(0, resolved.Count, "An attribute name was mistaken for the type that satisfies its prefix.");
    }

    /// <summary>An attached property does name a type: the owner half has to exist.</summary>
    [TestMethod]
    public void ResolvePrefixesFromCode_ResolvesAnAttachedPropertyOwner()
    {
        string xaml = """<Button local:Badge.Count="3" />""";

        Dictionary<string, string> resolved = XamlFragment.ResolvePrefixesFromCode(
            xaml,
            XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>()),
            "namespace Contoso;\n\npublic class Badge { }");

        Assert.AreEqual("using:Contoso", resolved["local"]);
    }

    /// <summary>
    /// A QName-valued attribute asks for the whole dotted name. Reading only the first segment
    /// finds the outer type declared, synthesizes an import for its namespace, and publishes markup
    /// whose actual request — the nested type — still cannot resolve.
    /// </summary>
    [TestMethod]
    public void ResolvePrefixesFromCode_DoesNotResolveAnUndeclaredNestedType()
    {
        string xaml = """<DataTemplate x:DataType="local:Container.Item" />""";

        List<string> unresolved = XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>());
        CollectionAssert.Contains(unresolved, "local", "The prefix still has to be reported as needing an import.");

        Dictionary<string, string> resolved = XamlFragment.ResolvePrefixesFromCode(
            xaml,
            unresolved,
            "namespace Contoso;\n\npublic class Container { }");

        Assert.AreEqual(
            0,
            resolved.Count,
            "An import was synthesized from the outer type for a nested type nothing declares.");
    }

    /// <summary>
    /// Attached-property syntax inside a binding path is the other reading of a dotted name, and
    /// there the owner is what has to exist. That behaviour is unchanged.
    /// </summary>
    [TestMethod]
    public void ResolvePrefixesFromCode_StillResolvesAnAttachedPropertyInABindingPath()
    {
        string xaml = """<TextBlock Text="{Binding Path=(local:Badge.Count)}" />""";

        Dictionary<string, string> resolved = XamlFragment.ResolvePrefixesFromCode(
            xaml,
            XamlFragment.UnresolvedPrefixes(xaml, new Dictionary<string, string>()),
            "namespace Contoso;\n\npublic class Badge { }");

        Assert.AreEqual("using:Contoso", resolved["local"]);
    }
}
