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
            // C#/WinRT apps won't always be packaged, but will always be full trust, so we can just directly load the file.
#if USING_CSWINRT
            string projectRoot = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
            return await File.ReadAllTextAsync(Path.Combine(projectRoot, relativeFilePath));
#else
            Uri sourceUri = new Uri("ms-appx:///" + relativeFilePath);
            var file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);
            return await FileIO.ReadTextAsync(file);
#endif
        }

        public static async Task<IList<string>> LoadLines(string relativeFilePath)
        {
            string fileContents = await LoadText(relativeFilePath);
            return fileContents.Split(Environment.NewLine).ToList();
        }
    }
}
