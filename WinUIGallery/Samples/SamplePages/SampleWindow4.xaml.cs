using System;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow4 : Window
{
    private AppWindow appWindow;

    public SampleWindow4()
    {
        this.InitializeComponent();

        appWindow = GetAppWindowForCurrentWindow();
        appWindow.Resize(new Windows.Graphics.SizeInt32(400, 300));

        // Create an OverlappedPresenter configured for a dialog window.
        // This ensures the window behaves like a modal dialog with appropriate styling.
        OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();

        // Set the owner of the modal window to ensure it remains linked to the main window.
        SetOwner(appWindow);

        // Enable modal behavior, preventing interaction with the main window until this window is closed.
        presenter.IsModal = true;

        // Apply the OverlappedPresenter configuration to the AppWindow.
        appWindow.SetPresenter(presenter);

        // Show the modal AppWindow.
        appWindow.Show();

    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }

    private void SetOwner(AppWindow childAppWindow)
    {
        // Get HWND of the main window
        // The main window can be retrieved from App.xaml.cs if it's set as a static property.
        IntPtr parentHwnd = WindowNative.GetWindowHandle(App.StartupWindow);

        // Get HWND of the AppWindow
        IntPtr childHwnd = Win32Interop.GetWindowFromWindowId(childAppWindow.Id);

        // Set the owner using SetWindowLongPtr
        SetWindowLongPtr(childHwnd, -8, parentHwnd); // -8 = GWLP_HWNDPARENT
    }

    // Imports the SetWindowLongPtr function from user32.dll, allowing modification of window properties.
    // This is used to set the owner of the child AppWindow by changing its parent window handle.
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
