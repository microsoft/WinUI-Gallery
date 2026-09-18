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
}
