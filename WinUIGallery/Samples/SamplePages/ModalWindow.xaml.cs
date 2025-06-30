// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class ModalWindow : Window
{
    public ModalWindow()
    {
        this.InitializeComponent();
        AppWindow.Resize(new Windows.Graphics.SizeInt32(400, 300));
        AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();

        // Set this modal window's owner (the main application window).
        // The main window can be retrieved from App.xaml.cs if it's set as a static property.
        SetWindowOwner(owner: App.StartupWindow);

        // Make the window modal (blocks interaction with the owner window until closed).
        presenter.IsModal = true;

        // Apply the presenter settings to the AppWindow.
        AppWindow.SetPresenter(presenter);

        // Show the modal window.
        AppWindow.Show();

        Closed += ModalWindow_Closed;
    }

    // Sets the owner window of the modal window.
    private void SetWindowOwner(Window owner)
    {
        // Get the HWND (window handle) of the owner window (main window).
        IntPtr ownerHwnd = WindowNative.GetWindowHandle(owner);

        // Get the HWND of the AppWindow (modal window).
        IntPtr ownedHwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);

        // Set the owner window using SetWindowLongPtr for 64-bit systems
        // or SetWindowLong for 32-bit systems.
        if (IntPtr.Size == 8) // Check if the system is 64-bit
        {
            SetWindowLongPtr(ownedHwnd, -8, ownerHwnd); // -8 = GWLP_HWNDPARENT
        }
        else // 32-bit system
        {
            SetWindowLong(ownedHwnd, -8, ownerHwnd); // -8 = GWL_HWNDPARENT
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
