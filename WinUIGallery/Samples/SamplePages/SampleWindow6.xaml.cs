using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow6 : Window
{
    private AppWindow appWindow;

    public SampleWindow6()
    {
        this.InitializeComponent();
        appWindow = GetAppWindowForCurrentWindow();
        appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        appWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        // Set the window to Full-Screen mode
        appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
