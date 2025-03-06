using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WinUIGallery.Models;

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
