using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow2 : Window
{
    private AppWindow appWindow;

    public SampleWindow2()
    {
        this.InitializeComponent();
        appWindow = GetAppWindowForCurrentWindow();

        // Center the window on the screen.
        CenterWindow(appWindow);
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }

    // Centers the given AppWindow on the screen based on the available display area.
    private void CenterWindow(AppWindow appWindow)
    {
        if (appWindow == null) return; // Ensure the AppWindow instance is valid.

        // Get the display area associated with the window.
        DisplayArea displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Nearest);
        if (displayArea == null) return; // Ensure the display area is valid.

        // Calculate the centered position within the work area.
        RectInt32 workArea = displayArea.WorkArea;
        PointInt32 centeredPosition = new PointInt32(
            (workArea.Width - appWindow.Size.Width) / 2,
            (workArea.Height - appWindow.Size.Height) / 2
        );

        appWindow.Move(centeredPosition); // Move the window to the calculated position.
    }
}
