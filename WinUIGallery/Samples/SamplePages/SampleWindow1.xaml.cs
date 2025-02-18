using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow1 : Window
{
    public SampleWindow1()
    {
        this.InitializeComponent();
    }

    public SampleWindow1(string WindowTitle, Int32 Width, Int32 Height, Int32 X, Int32 Y)
    {
        this.InitializeComponent();

        AppWindow appWindow = GetAppWindowForCurrentWindow();

        // Set the window title
        appWindow.Title = WindowTitle;

        // Set the window size (including borders)
        appWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));

        // Set the window position on screen
        appWindow.Move(new Windows.Graphics.PointInt32(X, Y));

        // Set the window icon
        appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }
}
