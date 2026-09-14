// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Pins the bundle format rules the exporter shares with ControlExample.ParseSampleCodeSections.
/// The catalog's promise is "this is the code the gallery shows", so any divergence between the
/// two parsers publishes something users never see. These tests encode the rules that are easy
/// to get subtly wrong.
/// </summary>
[TestClass]
public sealed class SampleBundleParserTests
{
    [TestMethod]
    public void Parse_ReadsHeaderXamlAndCSharpSections()
    {
        SampleBundle bundle = SampleBundleParser.Parse("--- header\nA button.\n--- xaml\n<Button/>\n--- c#\nvar x = 1;\n");

        Assert.AreEqual("A button.", bundle.Header);
        Assert.AreEqual("<Button/>", bundle.Xaml);
        Assert.AreEqual("var x = 1;", bundle.CSharp);
    }

    [TestMethod]
    public void Parse_OmitsSectionsThatAreNotPresent()
    {
        SampleBundle bundle = SampleBundleParser.Parse("--- header\nHeader only.\n");

        Assert.AreEqual("Header only.", bundle.Header);
        Assert.IsNull(bundle.Xaml);
        Assert.IsNull(bundle.CSharp);
    }

    [TestMethod]
    public void Parse_RequiresTheTrailingSpaceInTheSectionMarker()
    {
        // The app matches on "--- " exactly, so "---xaml" is body text, not a new section. Getting
        // this wrong would silently split content that the gallery renders as a single block.
        SampleBundle bundle = SampleBundleParser.Parse("--- xaml\n<Grid>\n---xaml\n</Grid>\n");

        Assert.AreEqual("<Grid>\n---xaml\n</Grid>", bundle.Xaml);
    }

    [TestMethod]
    public void Parse_TrimsSurroundingBlankLinesButKeepsInteriorOnes()
    {
        SampleBundle bundle = SampleBundleParser.Parse("--- xaml\n\n<A/>\n\n<B/>\n\n");

        Assert.AreEqual("<A/>\n\n<B/>", bundle.Xaml);
    }

    [TestMethod]
    public void Parse_NormalizesCarriageReturns()
    {
        SampleBundle bundle = SampleBundleParser.Parse("--- xaml\r\n<A/>\r\n<B/>\r\n");

        Assert.AreEqual("<A/>\n<B/>", bundle.Xaml);
        Assert.IsFalse(bundle.Xaml!.Contains('\r'), "Published code must use LF line endings only.");
    }

    [TestMethod]
    public void Parse_MatchesSectionNamesCaseInsensitively()
    {
        SampleBundle bundle = SampleBundleParser.Parse("--- Header\nH\n--- XAML\n<A/>\n--- C#\nvar x = 1;\n");

        Assert.AreEqual("H", bundle.Header);
        Assert.AreEqual("<A/>", bundle.Xaml);
        Assert.AreEqual("var x = 1;", bundle.CSharp);
    }

    [TestMethod]
    public void Parse_IgnoresUnknownSections()
    {
        SampleBundle bundle = SampleBundleParser.Parse("--- notes\nInternal note.\n--- xaml\n<A/>\n");

        Assert.AreEqual("<A/>", bundle.Xaml);
        Assert.IsNull(bundle.Header);
        Assert.IsNull(bundle.CSharp);
    }

    [TestMethod]
    public void Parse_IgnoresContentBeforeTheFirstSection()
    {
        SampleBundle bundle = SampleBundleParser.Parse("stray text\n--- xaml\n<A/>\n");

        Assert.AreEqual("<A/>", bundle.Xaml);
    }

    [TestMethod]
    public void Parse_KeepsTheLastOccurrenceOfARepeatedSection()
    {
        SampleBundle bundle = SampleBundleParser.Parse("--- xaml\n<First/>\n--- xaml\n<Second/>\n");

        Assert.AreEqual("<Second/>", bundle.Xaml);
    }
}
