// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Helpers;

namespace WinUIGallery.Controls;

public sealed partial class HomePageHeader : UserControl
{
    public string WinAppSdkDetails => VersionHelper.WinAppSdkDetails;

    public HomePageHeader()
    {
        InitializeComponent();
    }
}
