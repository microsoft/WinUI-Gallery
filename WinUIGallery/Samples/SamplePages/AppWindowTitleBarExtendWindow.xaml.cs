// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class AppWindowTitleBarExtendWindow : Window
{
    public AppWindowTitleBarExtendWindow(bool ExtendsContentIntoTitleBar, TitleBarHeightOption heightOption)
    {
        InitializeComponent();

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsResizable = false;

        AppWindow.SetPresenter(presenter);
        AppWindow.Resize(new Windows.Graphics.SizeInt32(600, 400));

        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

        AppWindow.TitleBar.ExtendsContentIntoTitleBar = ExtendsContentIntoTitleBar;

        if (AppWindow.TitleBar.ExtendsContentIntoTitleBar)
        {
            AppWindow.TitleBar.PreferredHeightOption = heightOption;
        }
    }
}
