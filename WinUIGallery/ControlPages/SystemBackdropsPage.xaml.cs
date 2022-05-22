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
using System.Runtime.InteropServices;

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
            setWindowSize(newWindow, 800, 600);
            newWindow.Activate();
        }

        private void createAcrylicWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new AppUIBasics.SamplePages.SampleSystemBackdropsWindow();
            newWindow.SetBackdrop(AppUIBasics.SamplePages.SampleSystemBackdropsWindow.BackdropType.DesktopAcrylic);
            setWindowSize(newWindow, 800, 600);

            newWindow.Activate();
        }

        private void setWindowSize(Window window,int width, int height)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var winID = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(winID);

            var dpi = GetDpiForWindow(hwnd);
            var scalingFactor = dpi / 96d;
            int widthInPixels = (int)(width * scalingFactor);
            int heighInPixels = (int)(height * scalingFactor);
            appWindow.Resize(new Windows.Graphics.SizeInt32(widthInPixels, heighInPixels));

        }
        [DllImport("user32.dll")]
        internal static extern int GetDpiForWindow(IntPtr hWnd);        
    }
}
