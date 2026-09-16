// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Covers the C# scanner that decides whether a snippet hands the reader the types its XAML names.
///
/// The stakes are asymmetric, and the tests are written around that. Missing a real declaration
/// costs a sample its XAML, which is the old, safe behaviour. Inventing one that is not there
/// publishes a fragment with an import for a type the reader never receives — exactly the failure
/// the exporter exists to prevent — so the cases that could fabricate a declaration are pinned
/// hardest.
/// </summary>
[TestClass]
public sealed class CodeDeclarationsTests
{
    [TestMethod]
    public void DeclaredTypes_FindsEveryTypeForm()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            """
            public sealed class Widget { }
            internal struct Point { }
            public interface IThing { }
            public enum Kind { A, B }
            public record Person(string Name);
            public readonly record struct Pair(int X, int Y);
            public record class Boxed(int Value);
            """);

        CollectionAssert.AreEquivalent(
            new[] { "Widget", "Point", "IThing", "Kind", "Person", "Pair", "Boxed" },
            types.Keys.ToArray());
    }

    /// <summary>
    /// An identifier that merely ends in a keyword is not a declaration, and neither is a keyword
    /// used as a value.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresKeywordsThatAreNotDeclarations()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "MyClass thing = GetClass();\nsubclass.Update();");

        Assert.AreEqual(0, types.Count, "Nothing here declares a type.");
    }

    /// <summary>
    /// The compiler treats a comment as whitespace, so it separates tokens. Deleting one outright
    /// welds the tokens on either side together and hides the declaration between them.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_ReadsADeclarationSplitByABlockComment()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes("public class/* which one? */Widget { }");

        CollectionAssert.Contains(types.Keys.ToArray(), "Widget");
    }

    /// <summary>
    /// Prose in a comment describes a type; it does not hand one over. ItemsRepeater's layout
    /// sample is the real instance of this.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresATypeNamedOnlyInAComment()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "// See the class VariedImageSizeLayout in the repo.\n/* class AlsoFake { } */\npublic class Recipe { }");

        CollectionAssert.AreEquivalent(new[] { "Recipe" }, types.Keys.ToArray());
    }

    [TestMethod]
    public void DeclaredTypes_IgnoresATypeNamedOnlyInAStringLiteral()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "string a = \"class Fake { }\";\nstring b = @\"class AlsoFake { }\";\npublic class Real { }");

        CollectionAssert.AreEquivalent(new[] { "Real" }, types.Keys.ToArray());
    }

    /// <summary>
    /// Raw string literals are the dangerous case: their content is usually markup or code, so a
    /// scanner that walks off the opening delimiter lands in text that looks exactly like a
    /// declaration and fabricates one.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresATypeInsideAMultiLineRawStringLiteral()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "string snippet = \"\"\"\n    public class Fake { }\n    \"\"\";\n\npublic class Real { }");

        CollectionAssert.AreEquivalent(new[] { "Real" }, types.Keys.ToArray());
    }

    [TestMethod]
    public void DeclaredTypes_IgnoresATypeInsideASingleLineRawStringLiteral()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "string snippet = \"\"\"class Fake { }\"\"\";\npublic class Real { }");

        CollectionAssert.AreEquivalent(new[] { "Real" }, types.Keys.ToArray());
    }

    /// <summary>
    /// A raw literal whose content contains quotes is written with a longer delimiter. Treating any
    /// run of three as the terminator would end the literal early and expose the rest.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresATypeInsideARawStringLiteralContainingQuotes()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "string xaml = \"\"\"\"\n    <Page x:Class=\"\"\"Fake\"\"\" />\n    public class Fake { }\n    \"\"\"\";\n\npublic class Real { }");

        CollectionAssert.AreEquivalent(new[] { "Real" }, types.Keys.ToArray());
    }

    /// <summary>
    /// Each declaration is tagged with the namespace it sits in, not with whichever namespace the
    /// file happens to open with. Publishing the first one would name a namespace that does not
    /// contain the type.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_TagsEachTypeWithItsOwnNamespace()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace A\n{\n    public class Other { }\n}\n\nnamespace B\n{\n    public class Foo { }\n}");

        Assert.AreEqual("A", types["Other"]);
        Assert.AreEqual("B", types["Foo"]);
    }

    [TestMethod]
    public void DeclaredTypes_TagsTypesUnderAFileScopedNamespace()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace Contoso.Sample;\n\npublic class Widget { }\npublic class Gadget { }");

        Assert.AreEqual("Contoso.Sample", types["Widget"]);
        Assert.AreEqual("Contoso.Sample", types["Gadget"]);
    }

    /// <summary>
    /// Types declared outside any namespace get the stand-in the gallery's own snippets already
    /// write, so the import and the code agree on the name a reader has to replace.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_UsesThePlaceholderForTypesOutsideAnyNamespace()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes("public class Category { }");

        Assert.AreEqual(CodeDeclarations.PlaceholderNamespace, types["Category"]);
    }

    /// <summary>
    /// A type declared before the file's first namespace belongs to no namespace, even though a
    /// namespace appears later in the file.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_DoesNotBackdateANamespaceOntoAnEarlierType()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "public class Early { }\n\nnamespace Later\n{\n    public class Late { }\n}");

        Assert.AreEqual(CodeDeclarations.PlaceholderNamespace, types["Early"]);
        Assert.AreEqual("Later", types["Late"]);
    }
}
