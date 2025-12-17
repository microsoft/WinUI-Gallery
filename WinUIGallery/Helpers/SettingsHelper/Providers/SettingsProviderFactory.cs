using Microsoft.Windows.Storage;
using System;
using System.IO;

namespace WinUIGallery.Helpers;

public static partial class SettingsProviderFactory
{
    public static ISettingsProvider CreateProvider()
    {
        if (NativeMethods.IsAppPackaged)
        {
            return new ApplicationDataSettingsProvider(ApplicationData.GetDefault().LocalSettings);
        }
        else
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ProcessInfoHelper.ProductName
            );

            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, "AppConfig.json");
            return new JsonSettingsProvider(filePath);
        }
    }
}
