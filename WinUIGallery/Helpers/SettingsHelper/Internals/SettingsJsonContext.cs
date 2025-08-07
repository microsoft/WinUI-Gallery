using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WinUIGallery.Helpers;

[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(Dictionary<string, System.Text.Json.JsonElement>))]
[JsonSerializable(typeof(Vector2Data))]
[JsonSerializable(typeof(Vector3Data))]
[JsonSerializable(typeof(SettingsHelper))]
internal partial class SettingsJsonContext : JsonSerializerContext
{
}