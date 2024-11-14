// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WinUIGallery
{
    public class DynamicRuntime
    {
        public static string WindowsAppRuntimeVersion { get; private set; }
        public static string WindowsAppRuntimeVersionFriendly { get; private set; }
        public static string WindowsAppRuntimeBaseDirectory { get; private set; }

        // This is a ModuleInitializer so that it will run very early in app startup.
        [global::System.Runtime.CompilerServices.ModuleInitializer]
        internal static void AccessWindowsAppSDK()
        {
            // Decide which version of WindowsAppRuntime to use.
            string versionA = "1.5.241001000";
            string versionB = "1.6.240923002";

            // Show win32 message box with yes/no option
            var result = NativeMethods.MessageBox(IntPtr.Zero, "Use newer version?", "Use newer version?", 4 /*MB_YESNO*/);

            string version = (result == 6 /*YES*/ ? versionB : versionA);

            WindowsAppRuntimeVersion = "Microsoft.WindowsAppSDK." + version;
            WindowsAppRuntimeVersionFriendly = "Windows App SDK " + version.Substring(0, 3);

            WindowsAppRuntimeBaseDirectory = AppContext.BaseDirectory + WindowsAppRuntimeVersion + "\\";

            // Set base directory env var for PublishSingleFile support.
            // This will tell UndockedRegFreeWinRT where to find the implementation DLLs.
            Environment.SetEnvironmentVariable(
                "MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY",
                WindowsAppRuntimeBaseDirectory);

            // This is a .net API that allows us to customize the DLL resolution process.
            // We'll use it to point at the right WindowsAppRuntime directory.
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DynamicRuntimeDllResolver);
            NativeMethods.WindowsAppRuntime_EnsureIsLoaded();
        }

        private static IntPtr DynamicRuntimeDllResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // We'll first look for the DLL in the WindowsAppRuntime directory.  If it's there, we'll resolve to that DLL.
            // If not, we'll fallback to the "normal" search order.
            var libraryPath = Path.Combine(WindowsAppRuntimeBaseDirectory, libraryName);
            if (File.Exists(libraryPath))
            {
                return NativeLibrary.Load(libraryPath, assembly, DllImportSearchPath.LegacyBehavior);
            }
            return NativeLibrary.Load(libraryName, assembly, searchPath);
        }

        internal static class NativeMethods
        {
            [DllImport("Microsoft.WindowsAppRuntime.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            internal static extern int WindowsAppRuntime_EnsureIsLoaded();

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int MessageBox(IntPtr hWnd, string text, string caption, int options);
        }
    }
}
