using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace WinUIGallery.SourceGenerator;

internal partial class Root
{
    public ObservableCollection<ControlInfoDataGroup> Groups { get; set; } = [];
}
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Root))]
internal partial class RootContext : JsonSerializerContext
{
}

internal partial class ControlInfoDataItem
{
    public string UniqueId { get; set; } = string.Empty;    
}

internal partial class ControlInfoDataGroup
{
    public string UniqueId { get; set; } = string.Empty;
    public ObservableCollection<ControlInfoDataItem> Items { get; set; } = [];
}
