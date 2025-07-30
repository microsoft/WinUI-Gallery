// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WinUIGallery.Models;

namespace WinUIGallery.Helpers;
internal partial class IconsDataSource
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
        var jsonText = await FileLoader.LoadText("Samples/Data/IconsData.json");
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
