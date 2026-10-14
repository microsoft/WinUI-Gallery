// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Globalization;
using WinUIGallery.Helpers;

namespace WinUIGallery.Controls;

public sealed partial class HomePageHeader : UserControl
{
    public string WinAppSdkVersion => VersionHelper.WinAppSdkVersion;

    public string WinAppSdkChannel => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(VersionHelper.WinAppSdkChannel);

    public bool HasWinAppSdkChannel => !string.IsNullOrEmpty(VersionHelper.WinAppSdkChannel);

    public string WinAppSdkChannelDescription => VersionHelper.WinAppSdkChannel switch
    {
        "experimental" => "This app uses an experimental release of the Windows App SDK to showcase upcoming features. Some of the APIs it demonstrates are experimental and might change or be removed in a future release.",
        "preview" => "This app uses a preview release of the Windows App SDK to showcase upcoming features. Some of the APIs it demonstrates are in preview and might change before the stable release.",
        _ => "This app uses a pre-release version of the Windows App SDK. Some of the APIs it demonstrates might change before the stable release.",
    };

    public HomePageHeader()
    {
        InitializeComponent();
    }
}
