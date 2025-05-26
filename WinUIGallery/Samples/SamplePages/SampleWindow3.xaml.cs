using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow3 : Window
{
    public SampleWindow3(bool IsAlwaysOnTop, bool IsMaximizable, bool IsMinimizable, bool IsResizable, bool HasBorder, bool HasTitleBar)
    {
        this.InitializeComponent();
        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = IsAlwaysOnTop;
        presenter.IsMaximizable = IsMaximizable;
        presenter.IsMinimizable = IsMinimizable;
        presenter.IsResizable = IsResizable;
        presenter.SetBorderAndTitleBar(HasBorder, HasTitleBar);
        AppWindow.SetPresenter(presenter);
        AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        SizeChanged += SampleWindow3_SizeChanged;
    }

    private void MaximizeRestoreBtn_Click(object sender, RoutedEventArgs e)
    {
        OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
        if (presenter.State == OverlappedPresenterState.Maximized)
        {
            presenter.Restore();
        }
        else
        {
            presenter.Maximize();
        }
    }

    private void SampleWindow3_SizeChanged(object sender, WindowSizeChangedEventArgs e)
    {
        OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
        MaximizeRestoreBtn.Content = presenter.State == OverlappedPresenterState.Maximized ? "Restore" : "Maximize";
    }

    private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
    {
        OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
        presenter.Minimize();
    }

    private void RestoreBtn_Click(object sender, RoutedEventArgs e)
    {
        OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
        presenter.Minimize();
        Task.Delay(3000).ContinueWith(t => presenter.Restore());
    }

    private void CloseBtn_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
