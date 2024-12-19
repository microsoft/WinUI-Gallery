//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.SamplePages;

namespace WinUIGallery.ControlPages
{
    public sealed partial class SystemBackdropsPage : Page
    {
        public SystemBackdropsPage()
        {
            InitializeComponent();
        }

        private void createBuiltInMicaWindow_Click(object sender, RoutedEventArgs e)
        {
            var micaWindow = new SampleBuiltInSystemBackdropsWindow();
            micaWindow.AllowedBackdrops = [SampleBuiltInSystemBackdropsWindow.BackdropType.None,
                SampleBuiltInSystemBackdropsWindow.BackdropType.Mica,
                SampleBuiltInSystemBackdropsWindow.BackdropType.MicaAlt];
            micaWindow.SetBackdrop(SampleBuiltInSystemBackdropsWindow.BackdropType.Mica);
            micaWindow.Activate();
        }

        private void createBuiltInAcrylicWindow_Click(object sender, RoutedEventArgs e)
        {
            var acrylicWindow = new SampleBuiltInSystemBackdropsWindow();
            acrylicWindow.AllowedBackdrops = [SampleBuiltInSystemBackdropsWindow.BackdropType.None,
                SampleBuiltInSystemBackdropsWindow.BackdropType.Acrylic];
            acrylicWindow.SetBackdrop(SampleBuiltInSystemBackdropsWindow.BackdropType.Acrylic);
            acrylicWindow.Activate();
        }

        private void createCustomMicaWindow_Click(object sender, RoutedEventArgs e)
        {
            var micaWindow = new SampleSystemBackdropsWindow();
            micaWindow.AllowedBackdrops = [SampleSystemBackdropsWindow.BackdropType.None,
                SampleSystemBackdropsWindow.BackdropType.Mica,
                SampleSystemBackdropsWindow.BackdropType.MicaAlt];
            micaWindow.SetBackdrop(SampleSystemBackdropsWindow.BackdropType.Mica);
            micaWindow.Activate();
        }

        private void createCustomDesktopAcrylicWindow_Click(object sender, RoutedEventArgs e)
        {
            var acrylicWindow = new SampleSystemBackdropsWindow();
            acrylicWindow.AllowedBackdrops = [SampleSystemBackdropsWindow.BackdropType.None,
                SampleSystemBackdropsWindow.BackdropType.Acrylic,
                SampleSystemBackdropsWindow.BackdropType.AcrylicThin];
            acrylicWindow.SetBackdrop(SampleSystemBackdropsWindow.BackdropType.Acrylic);
            acrylicWindow.Activate();
        }
    }
}
