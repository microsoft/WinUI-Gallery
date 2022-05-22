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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace AppUIBasics.ControlPages
{
    public sealed partial class CreateMultipleWindowsPage : Page
    {
        public CreateMultipleWindowsPage()
        {
            this.InitializeComponent();
        }

        private void List_GotFocus(object sender, RoutedEventArgs e)
        {
            Control1.StartBringIntoView();
        }

        private void createNewWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = WindowHelper.CreateWindow();
            var rootPage = new NavigationRootPage();
            rootPage.RequestedTheme = ThemeHelper.RootTheme;
            newWindow.Content = rootPage;
            newWindow.Activate();

            var targetPageType = typeof(NewControlsPage);
            string targetPageArguments = string.Empty;
            rootPage.Navigate(targetPageType, targetPageArguments);
        }
        private void createNewWindowSample1_Click(object sender, RoutedEventArgs e)
        {
            // Create a new window and set the title and the content
            Window window = new()
            {
                Title = $"Window [{DateTime.Now.ToLongTimeString()}]",
                Content = new TextBlock()
                {
                    Text = "This is a top level Window",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            SetWindowSize(window,640, 480);

            //Activate shows the Window on the screen
            window.Activate();
        }

        private static void SetWindowSize(Window window, int height, int width)
        {
            // To set the Size, you need to wrap the Xaml Window into AppWindow
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowsId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowsId);
            // This is the size of the window is in Pixels, WinUI uses effective pixels
            appWindow.Resize(new Windows.Graphics.SizeInt32(height, width));
        }
    }
}
