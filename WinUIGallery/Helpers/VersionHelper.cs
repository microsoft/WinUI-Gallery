// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.WindowsAppSDK;

namespace WinUIGallery.Helpers;

internal static partial class VersionHelper
{
    // Use SDK-generated version info; self-contained apps don't deploy the framework's Insights resource DLL.
    public static string WinAppSdkVersion => $"Windows App SDK {Release.Major}.{Release.Minor}";

    // Empty for stable releases; otherwise, for example, "preview" or "experimental".
    public static string WinAppSdkChannel => Release.Channel;

    public static string WinAppSdkDetails =>
        WinAppSdkVersion +
        (string.IsNullOrEmpty(WinAppSdkChannel) ? string.Empty : $" ({WinAppSdkChannel})");

    public static string WinAppSdkRuntimeDetails =>
        WinAppSdkDetails + $", Windows App Runtime {Microsoft.WindowsAppSDK.Runtime.Version.DotQuadString}";
}
