using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class AppWindowTitleBarWindow : Window
{
    public AppWindowTitleBarWindow(Color BackgroundColor, Color ForegroundColor, Color ButtonBackgroundColor, Color ButtonForegroundColor,
        Color ButtonHoverBackgroundColor, Color ButtonHoverForegroundColor, Color ButtonInactiveBackgroundColor, Color ButtonInactiveForegroundColor,
        Color InactiveBackgroundColor, Color InactiveForegroundColor, Color ButtonPressedBackgroundColor, Color ButtonPressedForegroundColor)
    {
        InitializeComponent();

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsResizable = false;

        AppWindow.SetPresenter(presenter);
        AppWindow.Resize(new Windows.Graphics.SizeInt32(400, 400));
        AppWindow.TitleBar.BackgroundColor = BackgroundColor;
        AppWindow.TitleBar.ForegroundColor = ForegroundColor;
        AppWindow.TitleBar.ButtonBackgroundColor = ButtonBackgroundColor;
        AppWindow.TitleBar.ButtonForegroundColor = ButtonForegroundColor;
        AppWindow.TitleBar.ButtonHoverBackgroundColor = ButtonHoverBackgroundColor;
        AppWindow.TitleBar.ButtonHoverForegroundColor = ButtonHoverForegroundColor;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = ButtonInactiveBackgroundColor;
        AppWindow.TitleBar.ButtonInactiveForegroundColor = ButtonInactiveForegroundColor;
        AppWindow.TitleBar.InactiveBackgroundColor = InactiveBackgroundColor;
        AppWindow.TitleBar.InactiveForegroundColor = InactiveForegroundColor;
        AppWindow.TitleBar.ButtonPressedBackgroundColor = ButtonPressedBackgroundColor;
        AppWindow.TitleBar.ButtonPressedForegroundColor = ButtonPressedForegroundColor;
    }
}
