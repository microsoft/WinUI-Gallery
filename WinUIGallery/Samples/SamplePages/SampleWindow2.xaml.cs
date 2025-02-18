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

public sealed partial class SampleWindow2 : Window
{
    OverlappedPresenter presenter;

    public SampleWindow2(bool IsAlwaysOnTop, bool IsMaximizable, bool IsMinimizable, bool IsResizable, bool HasBorder, bool HasTitleBar)
    {
        this.InitializeComponent();

        AppWindow appWindow = GetAppWindowForCurrentWindow();

        presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = IsAlwaysOnTop;
        presenter.IsMaximizable = IsMaximizable;
        presenter.IsMinimizable = IsMinimizable;
        presenter.IsResizable = IsResizable;
        presenter.SetBorderAndTitleBar(HasBorder,HasTitleBar);

        appWindow.SetPresenter(presenter);
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }

    private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
    {
        presenter.Maximize();
    }

    private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
    {
        presenter.Minimize();
    }

    private void RestoreBtn_Click(object sender, RoutedEventArgs e)
    {
        presenter.Restore();
    }
}
