using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow1 : Window
{
    private AppWindow appWindow;

    public SampleWindow1()
    {
        InitializeComponent();
    }

    public SampleWindow1(string WindowTitle, int Width, int Height, int X, int Y)
    {
        InitializeComponent();

        // Retrieve the AppWindow instance for the current window
        appWindow = GetAppWindowForCurrentWindow();

        // Set the window title
        appWindow.Title = WindowTitle;

        // Set the window size (including borders)
        appWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));

        // Set the window position on screen
        appWindow.Move(new Windows.Graphics.PointInt32(X, Y));

        // Set the window icon
        appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
    }

    //Returrns the AppWindow instance associated with the current window.
    private AppWindow GetAppWindowForCurrentWindow()
    {
        // Get the native window handle
        IntPtr hWnd = WindowNative.GetWindowHandle(this);

        // Retrieve the WindowId from the window handle
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);

        // Return the AppWindow instance associated with the given WindowId
        return AppWindow.GetFromWindowId(myWndId);
    }

    private void Show_Click(object sender, RoutedEventArgs e) => appWindow.Show();

    private void Hide_Click(object sender, RoutedEventArgs e) => appWindow.Hide();

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
