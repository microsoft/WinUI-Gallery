// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
/// exact failure the exporter exists to prevent.
///
/// The work is handed to the compiler's own parser rather than to a pattern. Deciding what a piece
/// of C# declares means knowing where comments, literals and directives begin and end, and C# has
/// five lexical forms of string — quoted, verbatim, raw, interpolated, and interpolated raw — that
/// nest inside one another. A scanner modelling all but one of them does not fail loudly on the one
/// it missed: it reads that literal's contents as source and invents a declaration out of them.
/// Roslyn models the language, so the whole class of mistake is closed rather than enumerated.
/// </summary>
internal static class CodeDeclarations
{
    /// <summary>
    /// Namespace reported for types a snippet declares outside any namespace. The gallery's own
    /// snippets already use this name as their stand-in (see Samples\Binding\ConverterBinding.txt
    /// and Samples\TreeView\TreeviewItemtemplateselector.txt), so a reader meets the same
    /// placeholder in the import that they meet in the code.
    /// </summary>
    public const string PlaceholderNamespace = "YourNamespace";

    /// <summary>
    /// Snippets are fragments, not compilable files, so they are parsed as script: a bare statement
    /// or a member sitting on its own is then a normal thing to meet rather than something that
    /// derails the parse and takes the declarations after it down with it.
    ///
    /// No preprocessor symbols are defined. The exporter cannot know what the reader will build
    /// with, and <see cref="ConditionalRegions"/> discards every branch rather than trusting the
    /// one this choice happens to leave enabled.
    /// </summary>
    private static readonly CSharpParseOptions ParseOptions =
        new(LanguageVersion.Latest, kind: SourceCodeKind.Script);

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
    /// A type declared inside a member body is not reachable from XAML at all.
    ///
    /// A simple name declared in two namespaces is dropped rather than resolved to either. XAML
    /// asks for "local:Foo" and the snippet gives no way to say which Foo that is, so publishing
    /// one of them would be a coin toss printed as an import. Repeating a name within one namespace
    /// — a partial class split across the snippet — says nothing contradictory and is kept.
    ///
    /// A "file" type is excluded for the same reason nesting is. It exists only for the source file
    /// that declares it, and the code XAML compiles into is a different file, so no import can bring
    /// it within reach of "local:Foo".
    /// </summary>
    public static Dictionary<string, string> DeclaredTypes(string? code)
    {
        Dictionary<string, string> types = new(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(code))
        {
            return types;
        }

        SyntaxNode root = CSharpSyntaxTree.ParseText(code, ParseOptions).GetRoot();
        List<TextSpan> conditional = ConditionalRegions(root);
        HashSet<string> ambiguous = new(StringComparer.Ordinal);

        foreach (BaseTypeDeclarationSyntax declaration in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
        {
            if (!IsTopLevel(declaration) || IsFileLocal(declaration) || IsConditional(conditional, declaration))
            {
                continue;
            }

            string name = declaration.Identifier.ValueText;
            if (ambiguous.Contains(name))
            {
                continue;
            }

            string ns = NamespaceOf(declaration);
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
    /// True when the declaration sits directly in a namespace or at the top level of the snippet
    /// rather than inside another type or a member body. Script parsing puts top-level declarations
    /// under the compilation unit, so those two parents are the whole of namespace-or-global scope.
    /// </summary>
    private static bool IsTopLevel(BaseTypeDeclarationSyntax declaration) =>
        declaration.Parent is BaseNamespaceDeclarationSyntax or CompilationUnitSyntax;

    /// <summary>
    /// True when the declaration carries the "file" modifier, which confines the type to the one
    /// source file holding it. A reader pasting the snippet gets a type their XAML cannot name: the
    /// markup compiles into generated code of its own, and "using:" imports a namespace rather than
    /// lifting file scope. Reporting it would publish exactly the import that cannot work.
    /// </summary>
    private static bool IsFileLocal(BaseTypeDeclarationSyntax declaration) =>
        declaration.Modifiers.Any(SyntaxKind.FileKeyword);

    /// <summary>
    /// The namespace enclosing <paramref name="declaration"/>, joined outward in, or
    /// <see cref="PlaceholderNamespace"/> when nothing encloses it.
    ///
    /// One walk covers both namespace forms. A file-scoped namespace and a block-scoped one differ
    /// in how far they reach, and the tree has already worked that out — which is the whole of what
    /// tracking brace depth by hand was for.
    /// </summary>
    private static string NamespaceOf(SyntaxNode declaration)
    {
        List<string> enclosing = [];

        for (SyntaxNode? node = declaration.Parent; node is not null; node = node.Parent)
        {
            if (node is BaseNamespaceDeclarationSyntax ns)
            {
                enclosing.Insert(0, ClrNameOf(ns.Name));
            }
        }

        return enclosing.Count == 0 ? PlaceholderNamespace : string.Join('.', enclosing);
    }

    /// <summary>
    /// The CLR spelling of a namespace name, built from its identifiers rather than its source
    /// text.
    ///
    /// The two differ, and the difference is published. Source text carries whatever sits between
    /// the tokens — "namespace Contoso /* why */ . Models" is valid C# — and it keeps the "@" that
    /// escapes a keyword, though "@class" names the namespace "class". Emitting either spelling in
    /// a "using:" import produces a line that cannot resolve. Taking each identifier's ValueText
    /// drops the trivia and the escape together, which is the name the compiler works with.
    /// </summary>
    private static string ClrNameOf(NameSyntax name) =>
        string.Join(
            '.',
            name.DescendantTokens()
                .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
                .Select(token => token.ValueText));

    /// <summary>
    /// The spans covered by #if/#endif pairs, outermost only, taking in the directives themselves.
    ///
    /// Whether such a region reaches the compiler depends on symbols the exporter cannot see, so a
    /// type declared inside one is not a type the reader is guaranteed to receive. That holds for
    /// every branch, not only the ones this parse disabled: with no symbols defined the "#else" arm
    /// of "#if DEBUG" is live as far as the parser is concerned, yet a reader building with DEBUG
    /// gets the other one. Counting either would let the exporter publish an import for a type the
    /// compiler drops, while ignoring a region that would in fact have compiled costs the sample
    /// nothing worse than its XAML.
    ///
    /// #region and #pragma are different syntax nodes and pass through untouched: they group and
    /// annotate code, they do not decide whether it exists.
    /// </summary>
    private static List<TextSpan> ConditionalRegions(SyntaxNode root)
    {
        List<TextSpan> regions = [];
        int depth = 0;
        int start = 0;

        foreach (DirectiveTriviaSyntax directive in
            root.DescendantNodesAndSelf(descendIntoTrivia: true).OfType<DirectiveTriviaSyntax>())
        {
            if (directive is IfDirectiveTriviaSyntax)
            {
                if (depth == 0)
                {
                    start = directive.FullSpan.Start;
                }

                depth++;
            }
            else if (directive is EndIfDirectiveTriviaSyntax && depth > 0)
            {
                depth--;
                if (depth == 0)
                {
                    regions.Add(TextSpan.FromBounds(start, directive.FullSpan.End));
                }
            }
        }

        // A snippet that opens a region and never closes it is a fragment, not a mistake, and
        // everything after the directive is still conditional.
        if (depth > 0)
        {
            regions.Add(TextSpan.FromBounds(start, root.FullSpan.End));
        }

        return regions;
    }

    private static bool IsConditional(List<TextSpan> regions, SyntaxNode declaration) =>
        regions.Any(region => region.Contains(declaration.SpanStart));
}
