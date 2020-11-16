using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AppUIBasics.Common
{
    internal class FileLoader
    {
        public static async Task<string> LoadText(string relativeFilePath)
        {
            Uri sourceUri = new Uri("ms-appx:///" + relativeFilePath);
            var file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);
            return await FileIO.ReadTextAsync(file);
        }

        public static async Task<IList<string>> LoadLines(string relativeFilePath)
        {
            string fileContents = await LoadText(relativeFilePath);
            return fileContents.Split(Environment.NewLine).ToList();
        }
    }
}
