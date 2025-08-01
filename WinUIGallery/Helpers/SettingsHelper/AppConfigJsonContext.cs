using System.Text.Json.Serialization;

namespace WinUIGallery.Helpers;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfig))]
internal partial class AppConfigJsonContext : JsonSerializerContext
{
}
