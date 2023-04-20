using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AppUIBasics.Common;

namespace WinUIGallery.DesktopWap.DataModel
{
    public class IconData
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public string Character => char.ConvertFromUtf32(Convert.ToInt32(Code, 16));
        public string CodeGlyph => "\\u" + Code;
        public string TextGlyph => "&#x" + Code + ";";
    }

    internal class IconsDataSource
    {
        public static IconsDataSource Instance { get; } = new();

        public static List<IconData> Icons => Instance.icons;

        private List<IconData> icons = new();

        private IconsDataSource() { }

        public async Task<List<IconData>> LoadIcons()
        {
            if (icons.Count != 0)
            {
                return icons;
            }
            var jsonFile = await FileLoader.GetJsonFile("DataModel/IconsData.json");
            using FileStream openStream = File.OpenRead(jsonFile.Path);
            
            if (icons.Count == 0)
            {
                icons = await JsonSerializer.DeserializeAsync<List<IconData>>(openStream, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                });
            }
            return icons;
        }
    }
}
