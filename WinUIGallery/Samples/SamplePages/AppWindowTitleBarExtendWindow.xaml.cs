using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class AppWindowTitleBarExtendWindow : Window
{
    public AppWindowTitleBarExtendWindow(bool ExtendsContentIntoTitleBar)
    {
        InitializeComponent();

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsResizable = false;

        AppWindow.SetPresenter(presenter);
        AppWindow.Resize(new Windows.Graphics.SizeInt32(600, 400));

        AppWindow.TitleBar.ExtendsContentIntoTitleBar = ExtendsContentIntoTitleBar;
    }
}
