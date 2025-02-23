using System;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class ModalWindow : Window
{
    private AppWindow appWindow;

    public ModalWindow()
    {
        this.InitializeComponent();
        appWindow = GetAppWindowForCurrentWindow();
        appWindow.Resize(new Windows.Graphics.SizeInt32(400,300));

        OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();

        // Set this modal window's owner (the main application window).
        SetOwnership(appWindow, App.StartupWindow);

        // Make the window modal (blocks interaction with the owner window until closed).
        presenter.IsModal = true;

        // Apply the presenter settings to the AppWindow.
        appWindow.SetPresenter(presenter);

        // Show the modal window.
        appWindow.Show();

        Closed += ModalWindow_Closed;
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }

    // Sets the owner window of the modal window.
    private void SetOwnership(AppWindow childAppWindow, Window ownerWindow)
    {
        // Get the HWND (window handle) of the owner window (main window).
        IntPtr parentHwnd = WindowNative.GetWindowHandle(ownerWindow);

        // Get the HWND of the AppWindow (modal window).
        IntPtr childHwnd = Win32Interop.GetWindowFromWindowId(childAppWindow.Id);

        // Set the owner window using SetWindowLongPtr for 64-bit systems
        // or SetWindowLong for 32-bit systems.
        if (IntPtr.Size == 8) // Check if the system is 64-bit
        {
            SetWindowLongPtr(childHwnd, -8, parentHwnd); // -8 = GWLP_HWNDPARENT
        }
        else // 32-bit system
        {
            SetWindowLong(childHwnd, -8, parentHwnd);
        }
    }

    // Import the Windows API function SetWindowLongPtr for modifying window properties on 64-bit systems.
    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    // Import the Windows API function SetWindowLong for modifying window properties on 32-bit systems.
    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private void ModalWindow_Closed(object sender, WindowEventArgs args)
    {
        // Reactivate the main application window when the modal window closes.
        App.StartupWindow.Activate();
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
