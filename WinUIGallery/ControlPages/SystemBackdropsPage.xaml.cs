//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Helper;
using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class SystemBackdropsPage : Page
    {
        public SystemBackdropsPage()
        {
            this.InitializeComponent();
        }

        private void createMicaWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new AppUIBasics.SamplePages.SampleSystemBackdropsWindow();
            newWindow.Activate();
        }

        private void createAcrylicWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new AppUIBasics.SamplePages.SampleSystemBackdropsWindow();
            newWindow.SetBackdrop(AppUIBasics.SamplePages.SampleSystemBackdropsWindow.BackdropType.DesktopAcrylic);
            newWindow.Activate();
        }
    }
}
