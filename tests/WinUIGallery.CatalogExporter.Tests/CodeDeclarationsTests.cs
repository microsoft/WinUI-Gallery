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

    /// <summary>
    /// A conditional region may or may not survive compilation, and the exporter has no way to
    /// know which. Counting what it declares would hand a reader an import for a type the compiler
    /// removes, so nothing inside one counts.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresATypeDeclaredInsideAConditionalRegion()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "public class Real { }\n\n#if false\npublic class Excluded { }\n#endif");

        Assert.IsTrue(types.ContainsKey("Real"));
        Assert.IsFalse(types.ContainsKey("Excluded"), "A type the compiler may drop was reported as declared.");
    }

    /// <summary>The branch a symbol would have selected is unknowable too, so neither arm counts.</summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresBothArmsOfAConditional()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "#if DEBUG\npublic class Instrumented { }\n#else\npublic class Plain { }\n#endif\n\npublic class Always { }");

        Assert.IsFalse(types.ContainsKey("Instrumented"));
        Assert.IsFalse(types.ContainsKey("Plain"));
        Assert.IsTrue(types.ContainsKey("Always"), "Code after the region stopped being read.");
    }

    /// <summary>
    /// A nested conditional must not close the outer one early and let the rest of it be read as
    /// unconditional code.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresTypesAfterANestedConditionalCloses()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "#if OUTER\n#if INNER\npublic class Inner { }\n#endif\npublic class StillInside { }\n#endif\n\npublic class Outside { }");

        Assert.IsFalse(types.ContainsKey("Inner"));
        Assert.IsFalse(types.ContainsKey("StillInside"));
        Assert.IsTrue(types.ContainsKey("Outside"));
    }

    /// <summary>#region groups code, it does not gate it, so what it holds is still delivered.</summary>
    [TestMethod]
    public void DeclaredTypes_ReadsATypeInsideARegionDirective()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "#region Models\npublic class Recipe { }\n#endregion");

        Assert.IsTrue(types.ContainsKey("Recipe"), "A #region was mistaken for a conditional.");
    }

    /// <summary>
    /// The text of a conditional region drops out, but its line breaks do not, so a namespace that
    /// opens before one still covers what comes after it.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_KeepsTheEnclosingNamespaceAcrossAConditionalRegion()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace Contoso\n{\n#if DEBUG\n    public class Probe { }\n#endif\n    public class Shipping { }\n}");

        Assert.AreEqual("Contoso", types["Shipping"]);
    }

    /// <summary>
    /// A nested type is reached as "Container.Item", so an import naming the namespace alone does
    /// not bring "Item" into scope. Reporting it would publish a fragment whose "local:Item" still
    /// cannot resolve.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresATypeNestedInsideAnotherType()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace N;\n\npublic class Container\n{\n    public class Item { }\n}");

        Assert.AreEqual("N", types["Container"]);
        Assert.IsFalse(types.ContainsKey("Item"), "A nested type was reported as if it sat in the namespace.");
    }

    /// <summary>Nesting under a block-scoped namespace is no different.</summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresANestedTypeUnderABlockScopedNamespace()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace N\n{\n    public class Container\n    {\n        public enum Kind { A }\n    }\n}");

        CollectionAssert.AreEquivalent(new[] { "Container" }, types.Keys.ToArray());
        Assert.AreEqual("N", types["Container"]);
    }

    /// <summary>A type declared in a member body is not reachable from XAML at all.</summary>
    [TestMethod]
    public void DeclaredTypes_IgnoresATypeDeclaredInsideAMethodBody()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace N;\n\npublic class Page\n{\n    private void Load()\n    {\n        class Local { }\n    }\n}");

        Assert.IsFalse(types.ContainsKey("Local"));
    }

    /// <summary>
    /// A container closing before the next declaration must not leave it looking nested: sibling
    /// types after a type body are still namespace-level.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_ReadsSiblingTypesAfterANestedOne()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace N;\n\npublic class First\n{\n    private class Hidden { }\n}\n\npublic class Second { }");

        CollectionAssert.AreEquivalent(new[] { "First", "Second" }, types.Keys.ToArray());
        Assert.AreEqual("N", types["Second"]);
    }

    /// <summary>
    /// Nested namespaces do nest, and a type directly inside the inner one is still delivered under
    /// the joined name.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_ReadsATypeDirectlyInsideANestedNamespace()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace A\n{\n    namespace B\n    {\n        public class Foo { }\n    }\n}");

        Assert.AreEqual("A.B", types["Foo"]);
    }

    /// <summary>
    /// "local:Foo" names one type, and a snippet declaring Foo in two namespaces does not say which.
    /// Keeping whichever came last would print a coin toss as an import.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_DropsANameDeclaredInTwoNamespaces()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace A\n{\n    public class Foo { }\n}\n\nnamespace B\n{\n    public class Foo { }\n    public class Bar { }\n}");

        Assert.IsFalse(types.ContainsKey("Foo"), "An ambiguous name resolved to one of its namespaces.");
        Assert.AreEqual("B", types["Bar"], "An unrelated type was lost with the ambiguous one.");
    }

    /// <summary>
    /// The drop has to survive a third declaration, which would otherwise look like a fresh,
    /// unambiguous one and put the name back.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_KeepsANameDroppedAfterAThirdDeclaration()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace A\n{\n    public class Foo { }\n}\n\nnamespace B\n{\n    public class Foo { }\n}\n\nnamespace A\n{\n    public class Foo { }\n}");

        Assert.IsFalse(types.ContainsKey("Foo"));
    }

    /// <summary>
    /// A partial type split across a snippet declares the same name twice in one namespace. That
    /// says nothing contradictory, so the name is still delivered.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_KeepsANameDeclaredTwiceInOneNamespace()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "namespace Contoso;\n\npublic partial class Widget { }\n\npublic partial class Widget { }");

        Assert.AreEqual("Contoso", types["Widget"]);
    }

    /// <summary>
    /// A name declared both inside a namespace and outside every namespace is ambiguous too: the
    /// placeholder is a namespace like any other as far as the published import is concerned.
    /// </summary>
    [TestMethod]
    public void DeclaredTypes_DropsANameSharedWithTheGlobalScope()
    {
        Dictionary<string, string> types = CodeDeclarations.DeclaredTypes(
            "public class Foo { }\n\nnamespace Contoso\n{\n    public class Foo { }\n}");

        Assert.IsFalse(types.ContainsKey("Foo"));
    }
}
