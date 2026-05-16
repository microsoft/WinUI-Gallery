// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow4 : Window
{
    public SampleWindow4(int MinWidth, int MinHeight, int MaxWidth, int MaxHeight)
    {
        this.InitializeComponent();

        AppWindow.Resize(new Windows.Graphics.SizeInt32(800, 500));
        AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = MinWidth;
        presenter.PreferredMinimumHeight = MinHeight;
        presenter.PreferredMaximumWidth = MaxWidth;
        presenter.PreferredMaximumHeight = MaxHeight;
        presenter.IsMaximizable = false;

        AppWindow.SetPresenter(presenter);
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
