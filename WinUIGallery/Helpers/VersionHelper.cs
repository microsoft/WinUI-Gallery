using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WASDK = Microsoft.WindowsAppSDK;

namespace WinUIGallery.Helpers;
internal static class VersionHelper
{
    public static string WinAppSdkDetails
    {
        // TODO: restore patch number and version tag when WinAppSDK supports them both
        get => string.Format("Windows App SDK {0}.{1}",
            WASDK.Release.Major, WASDK.Release.Minor);
    }

    public static string WinAppSdkRuntimeDetails
    {
        get
        {
            try
            {
                // Retrieve Windows App Runtime version info dynamically
                IEnumerable<FileVersionInfo> windowsAppRuntimeVersion =
                    from module in Process.GetCurrentProcess().Modules.OfType<ProcessModule>()
                    where module.FileName.EndsWith("Microsoft.WindowsAppRuntime.Insights.Resource.dll")
                    select FileVersionInfo.GetVersionInfo(module.FileName);
                return WinAppSdkDetails + ", Windows App Runtime " + windowsAppRuntimeVersion.First().FileVersion;
            }
            catch
            {
                return WinAppSdkDetails + $", Windows App Runtime {WASDK.Runtime.Version.Major}.{WASDK.Runtime.Version.Minor}";
            }
        }
    }
}
