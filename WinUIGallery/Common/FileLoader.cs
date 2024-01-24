using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using WinUIGallery.Helper;
using System.IO;
using System.Reflection;

namespace WinUIGallery.Common
{
    internal class FileLoader
    {
        public static async Task<string> LoadText(string relativeFilePath)
        {
            StorageFile file = null;
            if (!NativeHelper.IsAppPackaged)
            {
                var sourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativeFilePath));
                file = await StorageFile.GetFileFromPathAsync(sourcePath);

            }
            else
            {
                Uri sourceUri = new Uri("ms-appx:///" + relativeFilePath);
                file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);

            }
            return await FileIO.ReadTextAsync(file);
        }

        public static async Task<IList<string>> LoadLines(string relativeFilePath)
        {
            string fileContents = await LoadText(relativeFilePath);
            return fileContents.Split(Environment.NewLine).ToList();
        }
    }
}
