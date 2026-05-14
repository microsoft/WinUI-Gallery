// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow7 : Window
{
    public SampleWindow7(string InitialSize)
    {
        this.InitializeComponent();

        AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        CompactOverlayPresenter presenter = CompactOverlayPresenter.Create();
        presenter.InitialSize = InitialSize switch
        {
            "Small" => CompactOverlaySize.Small,
            "Medium" => CompactOverlaySize.Medium,
            "Large" => CompactOverlaySize.Large,
            _ => presenter.InitialSize
        };

        AppWindow.SetPresenter(presenter);
    }
}
