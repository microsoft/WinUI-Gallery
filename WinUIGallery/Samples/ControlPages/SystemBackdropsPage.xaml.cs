// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class SystemBackdropsPage : Page
{
    public SystemBackdropsPage()
    {
        InitializeComponent();
    }

    private void createBuiltInWindow_Click(object sender, RoutedEventArgs e)
    {
        var buildInBackdropsWindow = new SampleBuiltInSystemBackdropsWindow();
        buildInBackdropsWindow.SetBackdrop(SampleBuiltInSystemBackdropsWindow.BackdropType.Mica);
        buildInBackdropsWindow.Activate();
    }

    private void createCustomMicaWindow_Click(object sender, RoutedEventArgs e)
    {
        var micaWindow = new SampleSystemBackdropsWindow
        {
            AllowedBackdrops = [
                SampleSystemBackdropsWindow.BackdropType.Mica,
                SampleSystemBackdropsWindow.BackdropType.MicaAlt,
                SampleSystemBackdropsWindow.BackdropType.None
            ]
        };
        micaWindow.Activate();
    }

    private void createCustomDesktopAcrylicWindow_Click(object sender, RoutedEventArgs e)
    {
        var acrylicWindow = new SampleSystemBackdropsWindow
        {
            AllowedBackdrops = [
                SampleSystemBackdropsWindow.BackdropType.Acrylic,
                SampleSystemBackdropsWindow.BackdropType.AcrylicThin,
                SampleSystemBackdropsWindow.BackdropType.None
            ]
        };
        acrylicWindow.Activate();
    }
}
