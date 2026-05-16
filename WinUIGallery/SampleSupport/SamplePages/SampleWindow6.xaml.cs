// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow6 : Window
{
    public SampleWindow6()
    {
        this.InitializeComponent();
        AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        // Set the window to Full-Screen mode
        AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
