using System;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow3 : Window
{
    private AppWindow appWindow;
    private OverlappedPresenter presenter;

    public SampleWindow3(bool IsAlwaysOnTop, bool IsMaximizable, bool IsMinimizable, bool IsResizable, bool HasBorder, bool HasTitleBar)
    {
        this.InitializeComponent();
        appWindow = GetAppWindowForCurrentWindow();
        presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = IsAlwaysOnTop;
        presenter.IsMaximizable = IsMaximizable;
        presenter.IsMinimizable = IsMinimizable;
        presenter.IsResizable = IsResizable;
        presenter.SetBorderAndTitleBar(HasBorder,HasTitleBar);
        appWindow.SetPresenter(presenter);
        appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
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
        presenter.Minimize();
        Task.Delay(3000).ContinueWith(t => presenter.Restore());
    }

    private void CloseBtn_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
