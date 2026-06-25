using Microsoft.Windows.Storage;

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
            return new ApplicationDataSettingsProvider(ApplicationData.GetForUnpackaged(ProcessInfoHelper.Publisher, ProcessInfoHelper.ProductName).LocalSettings);
        }
    }
}
