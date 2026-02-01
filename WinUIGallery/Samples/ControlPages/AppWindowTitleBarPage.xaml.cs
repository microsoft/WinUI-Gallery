// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace WinUIGallery.ControlPages;

public sealed partial class AppWindowTitleBarPage : Page
{
    public AppWindowTitleBarPage()
    {
        InitializeComponent();
    }

    private void ColorSelector_ColorChanged(Controls.ColorSelector obj)
    {
        Debug.WriteLine("ColorSelector_ColorChanged");
    }
}
