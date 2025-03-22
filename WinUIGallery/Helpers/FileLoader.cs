using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Reflection;

namespace WinUIGallery.Helpers;

internal class FileLoader
{
    public static async Task<string> LoadText(string relativeFilePath)
    {
        StorageFile file;
        if (!NativeHelper.IsAppPackaged)
        {
            var sourcePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), relativeFilePath));
            file = await StorageFile.GetFileFromPathAsync(sourcePath);

        }
        else
        {
            Uri sourceUri = new("ms-appx:///" + relativeFilePath);
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
