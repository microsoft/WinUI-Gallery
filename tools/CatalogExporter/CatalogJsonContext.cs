// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace WinUIGallery.CatalogExporter;

/// <summary>
/// Source-generated metadata for reading ControlInfoData.json, matching the pattern the gallery
/// and the navigation source generator already use for the same file
/// (<c>WinUIGallery/Models/ControlInfoData.cs</c> and
/// <c>WinUIGallery.SourceGenerator/ControlInfoData.cs</c>).
/// </summary>
/// <remarks>
/// Serializer behaviour deliberately stays on <c>CatalogGenerator.ReadOptions</c> rather than on a
/// <c>JsonSourceGenerationOptions</c> attribute here. Declaring it in both places would create two
/// definitions of the same thing that are free to drift, and the options object is the one the call
/// sites pass and the conformance tests inspect.
/// </remarks>
[JsonSerializable(typeof(ControlInfoRoot))]
internal partial class CatalogReadContext : JsonSerializerContext
{
}

/// <summary>
/// Source-generated metadata for writing catalog/windows-samples.json.
/// </summary>
/// <remarks>
/// As with <see cref="CatalogReadContext"/>, the naming policy, indentation, and null handling
/// remain on <see cref="CatalogGenerator.WriteOptions"/>. That matters more here: the emitted
/// field names are part of a published contract, and <c>ContractConformanceTests</c> derives the
/// names it expects from that options object, so keeping it authoritative is what lets the test
/// verify the writer rather than a copy of the writer's configuration.
/// </remarks>
[JsonSerializable(typeof(SampleIndex))]
internal partial class CatalogWriteContext : JsonSerializerContext
{
}
