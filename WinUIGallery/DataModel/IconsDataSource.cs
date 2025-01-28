using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WinUIGallery.Common;

namespace WinUIGallery.Data;

public class IconData
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string[] Tags { get; set; } = [];

    public string Character => char.ConvertFromUtf32(Convert.ToInt32(Code, 16));
    public string CodeGlyph => "\\u" + Code;
    public string TextGlyph => "&#x" + Code + ";";
}
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<IconData>))]
internal partial class IconDataListContext : JsonSerializerContext
{
}

internal class IconsDataSource
{
    public static IconsDataSource Instance { get; } = new();

    public static List<IconData> Icons => Instance.icons;

    private List<IconData> icons = new();

    private IconsDataSource() { }

    public object _lock = new();

    public async Task<List<IconData>> LoadIcons()
    {
        lock (_lock)
        {
            if (icons.Count != 0)
            {
                return icons;
            }
        }
        var jsonText = await FileLoader.LoadText("DataModel/IconsData.json");
        lock (_lock)
        {
            if (icons.Count == 0)
            {
                icons = JsonSerializer.Deserialize(jsonText, typeof(List<IconData>), IconDataListContext.Default) as List<IconData>;
            }
            return icons;
        }
    }
}
