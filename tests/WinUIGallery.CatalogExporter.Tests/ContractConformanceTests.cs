// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Keeps the generated index conformant with the shared contract published by microsoft/winappCli
/// at docs/winui-sample-index.schema.json.
///
/// The contract is owned by another repository, so there is no schema file here to diff against.
/// What these tests protect instead is the part a consumer actually depends on: that the field
/// names we emit are the contract's field names, and that the guarantees its reader relies on
/// hold. A typo such as "xmlnsImport" would still produce valid JSON and a passing up-to-date
/// test, and would simply arrive as nothing on the other side.
/// </summary>
[TestClass]
public sealed class ContractConformanceTests
{
    private static string RepoRoot => CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);

    /// <summary>
    /// Every property name the contract defines, transcribed from
    /// docs/winui-sample-index.schema.json. Extras are allowed by the schema
    /// ("additionalProperties": true), which is why the gallery-specific fields below are listed
    /// separately rather than folded in.
    /// </summary>
    private static readonly string[] ContractDocumentProperties =
        ["schemaVersion", "source", "generatedAtUtc", "controls"];

    private static readonly string[] ContractControlProperties =
    [
        "id", "name", "description", "details", "apiNamespace", "nugetPackage",
        "relatedControls", "xmlnsImports", "usings", "keywords", "curatedKeywords", "docs", "samples",
    ];

    private static readonly string[] ContractSampleProperties =
        ["header", "xaml", "code", "language", "details", "xmlnsImports"];

    private static readonly string[] ContractDocLinkProperties = ["title", "uri"];

    /// <summary>
    /// Fields this repository adds on top of the contract. They are grouped under "gallery" (plus
    /// the document-level provenance) precisely so that this list stays short and a new field
    /// cannot be mistaken for part of the shared contract.
    /// </summary>
    private static readonly string[] GalleryDocumentExtras = ["$schema", "generator", "controlCount"];

    private static readonly string[] GalleryObjectExtras = ["gallery"];

    /// <summary>The property names the writer will emit for a type.</summary>
    private static string[] SerializedNames(Type type) => type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
            ?? CatalogGenerator.WriteOptions.PropertyNamingPolicy!.ConvertName(p.Name))
        .ToArray();

    private static void AssertOnlyKnownProperties(Type type, string[] contract, string[] extras, string what)
    {
        string[] allowed = [.. contract, .. extras];
        string[] unknown = SerializedNames(type)
            .Where(n => !allowed.Contains(n, StringComparer.Ordinal))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        Assert.AreEqual(
            0,
            unknown.Length,
            $"{what} emits {string.Join(", ", unknown)}, which the shared contract does not define. "
                + "A consumer ignores unknown fields, so this would be published and silently dropped. "
                + "Either use the contract's name, or add it under the 'gallery' object with the others.");
    }

    [TestMethod]
    public void EmittedPropertyNames_AreTheContractsNames()
    {
        AssertOnlyKnownProperties(typeof(SampleIndex), ContractDocumentProperties, GalleryDocumentExtras, "The index root");
        AssertOnlyKnownProperties(typeof(IndexControl), ContractControlProperties, GalleryObjectExtras, "A control");
        AssertOnlyKnownProperties(typeof(IndexSample), ContractSampleProperties, GalleryObjectExtras, "A sample");
        AssertOnlyKnownProperties(typeof(IndexDocLink), ContractDocLinkProperties, [], "A docs entry");
    }

    [TestMethod]
    public void RequiredContractFields_ArePresentOnEveryEntry()
    {
        // The schema requires schemaVersion and controls at the document level, and id and samples
        // on each control. Everything else is optional, so these are the only fields whose absence
        // makes the file unreadable rather than merely sparse.
        SampleIndex index = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot }).Index;

        Assert.AreEqual(1, index.SchemaVersion, "version 1 is the only value the contract accepts");
        Assert.AreEqual("gallery", index.Source, "the consumer keys its source-specific behaviour off this value");
        Assert.IsTrue(index.Controls.Count > 0);

        foreach (IndexControl control in index.Controls)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(control.Id), $"{control.Gallery.UniqueId} has no id");
            Assert.IsNotNull(control.Samples, $"{control.Id} must declare a samples array, even when empty");
        }
    }

    [TestMethod]
    public void ControlIds_AreUniqueAndUrlSafe()
    {
        // The consumer derives its own sample ids as "{control id}-{n}", so these end up in user-
        // visible identifiers. A duplicate would merge two gallery pages into one.
        SampleIndex index = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot }).Index;

        string[] duplicates = index.Controls
            .GroupBy(c => c.Id, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        Assert.AreEqual(0, duplicates.Length, "Duplicate control ids: " + string.Join(", ", duplicates));

        string[] unsafeIds = index.Controls
            .Where(c => !c.Id.All(ch => char.IsAsciiLetterOrDigit(ch) || ch is '-' or '_'))
            .Select(c => c.Id)
            .ToArray();

        Assert.AreEqual(0, unsafeIds.Length, "Control ids must be URL-safe: " + string.Join(", ", unsafeIds));
    }

    [TestMethod]
    public void EveryPublishedXamlFragment_SurvivesTheConsumersParser()
    {
        // This is the guarantee the whole export depends on. The consumer parses each fragment and
        // discards whatever fails, without reporting it, so anything that slips through here is a
        // sample that appears to ship and never arrives.
        SampleIndex index = CatalogGenerator.Generate(new CatalogGenerationOptions { RepoRoot = RepoRoot }).Index;

        string[] malformed = index.Controls
            .SelectMany(c => c.Samples.Select(s => (Control: c, Sample: s)))
            .Where(x => x.Sample.Xaml is not null && !XamlFragment.IsWellFormed(x.Sample.Xaml))
            .Select(x => $"{x.Control.Id}/{x.Sample.Gallery.Snippet}")
            .ToArray();

        Assert.AreEqual(0, malformed.Length, "These published fragments would be silently dropped: " + string.Join(", ", malformed));
    }

    [TestMethod]
    public void SchemaUrl_PointsAtThePublishedContract()
    {
        // The file is self-describing: a reader who finds it should be able to reach the contract
        // that defines it without knowing this repository's history.
        Assert.AreEqual(
            "https://raw.githubusercontent.com/microsoft/winappCli/main/docs/winui-sample-index.schema.json",
            new SampleIndex().Schema);
    }

    [TestMethod]
    public void Index_DoesNotEmbedAGenerationTimestamp()
    {
        // The contract offers generatedAtUtc, but the file is committed and a CI check regenerates
        // it to prove it is current. A timestamp would make every regeneration differ and turn
        // that check into noise, so it is deliberately not emitted.
        string json = CatalogGenerator.Serialize(new SampleIndex());

        Assert.IsFalse(
            JsonDocument.Parse(json).RootElement.TryGetProperty("generatedAtUtc", out _),
            "a generation timestamp would make the committed file differ on every run");
    }
}
