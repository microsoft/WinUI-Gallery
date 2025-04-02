using System;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow4 : Window
{
    private AppWindow appWindow;
    private OverlappedPresenter presenter;

    public SampleWindow4(int MinWidth, int MinHeight, int MaxWidth, int MaxHeight)
    {
        this.InitializeComponent();

        appWindow = GetAppWindowForCurrentWindow();
        appWindow.Resize(new Windows.Graphics.SizeInt32(800, 500));
        presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = MinWidth;
        presenter.PreferredMinimumHeight = MinHeight;
        presenter.PreferredMaximumWidth = MaxWidth;
        presenter.PreferredMaximumHeight = MaxHeight;
        presenter.IsMaximizable = false;
        appWindow.SetPresenter(presenter);
        appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
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
