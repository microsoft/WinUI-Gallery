//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using Microsoft;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppUIBasics.ControlPages
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomTitlebarPage : Page
    {
        private Windows.UI.Color currentBgColor = Colors.Transparent;
        private Windows.UI.Color currentFgColor = Colors.Black;

        public CustomTitlebarPage()
        {
            this.InitializeComponent();
            UpdateTitlebarColor();
            UpdateButtonText();
        }


        [ComImport, Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IWindowNative
        {
            IntPtr WindowHandle { get; }
        };

        private void SetTitlebar(UIElement titlebar)
        {
            var window = App.StartupWindow;
            if (!window.ExtendsContentIntoTitleBar)
            {
                window.ExtendsContentIntoTitleBar = true;
                window.SetTitleBar(titlebar);
            }
            else
            {
                window.ExtendsContentIntoTitleBar = false;
                window.SetTitleBar(null);

            }
            UpdateButtonText();
            UpdateTitlebarColor();
        }

        private void UpdateButtonText()
        {
            var window = App.StartupWindow;
            if (window.ExtendsContentIntoTitleBar)
            {
                customTitlebar.Content = "Reset to system Titlebar";
                defaultTitlebar.Content = "Reset to system Titlebar";
            }
            else
            {
                customTitlebar.Content = "Set Custom Titlebar";
                defaultTitlebar.Content = "Set Fallback Custom Titlebar";
            }

        }

        private void BgColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = ((Microsoft.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

            BackgroundColorElement.Background = new SolidColorBrush(color);

            currentBgColor = color;
            UpdateTitlebarColor();

        }

        private void FgColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = ((Microsoft.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

            ForegroundColorElement.Background = new SolidColorBrush(color);

            currentFgColor = color;
            UpdateTitlebarColor();

        }


        private void UpdateTitlebarColor()
        {
            var res = Microsoft.UI.Xaml.Application.Current.Resources;
            res["WindowCaptionBackground"] = currentBgColor;
            //res["WindowCaptionBackgroundDisabled"] = currentBgColor;
            res["WindowCaptionForeground"] = currentFgColor;
            //res["WindowCaptionForegroundDisabled"] = currentFgColor;

            // to trigger repaint tracking task id 38044406
            var native = App.StartupWindow.As<IWindowNative>();
            var hwnd = native.WindowHandle;
            var activeWindow = Win32.GetActiveWindow();
            if (hwnd == activeWindow)
            {
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_INACTIVE, IntPtr.Zero);
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_ACTIVE, IntPtr.Zero);
            }
            else
            {
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_ACTIVE, IntPtr.Zero);
                Win32.SendMessage(hwnd, Win32.WM_ACTIVATE, Win32.WA_INACTIVE, IntPtr.Zero);
            }
        }

        private void customTitlebar_Click(object sender, RoutedEventArgs e)
        {
            SetTitlebar(App.appTitlebar);
        }
        private void defaultTitlebar_Click(object sender, RoutedEventArgs e)
        {
            SetTitlebar(null);
        }
    }
}
