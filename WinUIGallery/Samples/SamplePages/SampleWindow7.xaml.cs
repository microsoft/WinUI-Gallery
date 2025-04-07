using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow7 : Window
{
    private AppWindow appWindow;
    private CompactOverlayPresenter presenter;

    public SampleWindow7(string InitialSize)
    {
        this.InitializeComponent();

        appWindow = GetAppWindowForCurrentWindow();
        appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");

        presenter = CompactOverlayPresenter.Create();
        presenter.InitialSize = InitialSize switch
        {
            "Small" => CompactOverlaySize.Small,
            "Medium" => CompactOverlaySize.Medium,
            "Large" => CompactOverlaySize.Large,
            _ => presenter.InitialSize
        };

        appWindow.SetPresenter(presenter);
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }
}
