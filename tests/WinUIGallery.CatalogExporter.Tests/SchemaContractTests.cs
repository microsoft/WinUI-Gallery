// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Keeps the published JSON Schemas honest about what the exporter actually emits.
///
/// Both schemas set "additionalProperties": false, so a field added to a model but not to its
/// schema turns every generated file into a schema violation for consumers - and nothing else in
/// the build would catch it, because the up-to-date tests only compare generated output against
/// committed output. These tests compare the models to the schemas directly.
/// </summary>
[TestClass]
public sealed class SchemaContractTests
{
    private static string RepoRoot => CatalogGenerator.FindRepoRoot(AppContext.BaseDirectory);

    private static JsonElement Schema(string fileName)
    {
        string path = Path.Combine(RepoRoot, "catalog", fileName);
        Assert.IsTrue(File.Exists(path), $"{path} is missing.");
        return JsonDocument.Parse(File.ReadAllText(path)).RootElement.Clone();
    }

    /// <summary>The property names <see cref="CatalogGenerator.WriteOptions"/> will emit for a type.</summary>
    private static string[] SerializedNames(Type type) => type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
            ?? CatalogGenerator.WriteOptions.PropertyNamingPolicy!.ConvertName(p.Name))
        .OrderBy(n => n, StringComparer.Ordinal)
        .ToArray();

    private static string[] DeclaredProperties(JsonElement objectSchema) => objectSchema
        .GetProperty("properties")
        .EnumerateObject()
        .Select(p => p.Name)
        .OrderBy(n => n, StringComparer.Ordinal)
        .ToArray();

    private static void AssertDeclaresExactly(Type type, JsonElement objectSchema, string what)
    {
        CollectionAssert.AreEqual(
            SerializedNames(type),
            DeclaredProperties(objectSchema),
            $"{what} does not declare exactly the properties {type.Name} emits. Because the schema sets \"additionalProperties\": false, an undeclared property makes every generated file invalid for consumers.");
    }

    [TestMethod]
    public void ManifestSchema_DeclaresExactlyWhatTheModelsEmit()
    {
        JsonElement schema = Schema("windows-samples.schema.json");
        JsonElement sample = schema.GetProperty("definitions").GetProperty("sample");

        AssertDeclaresExactly(typeof(CatalogManifest), schema, "windows-samples.schema.json root");
        AssertDeclaresExactly(typeof(CatalogSample), sample, "The 'sample' definition");
        AssertDeclaresExactly(typeof(CatalogScenario), sample.GetProperty("properties").GetProperty("scenarios").GetProperty("items"), "The scenario object");
        AssertDeclaresExactly(typeof(CatalogGroupRef), sample.GetProperty("properties").GetProperty("group"), "The sample 'group' object");
        AssertDeclaresExactly(typeof(CatalogSource), sample.GetProperty("properties").GetProperty("source"), "The sample 'source' object");
        AssertDeclaresExactly(typeof(CatalogDocLink), sample.GetProperty("properties").GetProperty("docs").GetProperty("items"), "The sample 'docs' item");
        AssertDeclaresExactly(typeof(CatalogRepository), schema.GetProperty("properties").GetProperty("repository"), "The 'repository' object");
        AssertDeclaresExactly(typeof(CatalogDefaults), schema.GetProperty("properties").GetProperty("defaults"), "The 'defaults' object");
    }

    [TestMethod]
    public void CodeSchema_DeclaresExactlyWhatTheModelsEmit()
    {
        JsonElement schema = Schema("windows-samples.code.schema.json");

        AssertDeclaresExactly(typeof(CatalogCodeManifest), schema, "windows-samples.code.schema.json root");
        AssertDeclaresExactly(typeof(CatalogScenarioCode), schema.GetProperty("properties").GetProperty("scenarios").GetProperty("items"), "The scenario code object");
    }

    [TestMethod]
    public void DeclaredSchemaVersions_MatchTheModelDefaults()
    {
        // The schemas pin schemaVersion with "const", so a bump has to be made in both places.
        Assert.AreEqual(
            new CatalogManifest().SchemaVersion,
            Schema("windows-samples.schema.json").GetProperty("properties").GetProperty("schemaVersion").GetProperty("const").GetInt32());

        Assert.AreEqual(
            new CatalogCodeManifest().SchemaVersion,
            Schema("windows-samples.code.schema.json").GetProperty("properties").GetProperty("schemaVersion").GetProperty("const").GetInt32());
    }
}
