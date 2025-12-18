// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class CreateMultipleWindowsPage : Page
{
    public CreateMultipleWindowsPage()
    {
        this.InitializeComponent();
    }

    private void CreateNewWindow_Click(object sender, RoutedEventArgs e)
    {
        var childWindow = new Window()
        {
            ExtendsContentIntoTitleBar = true,
            SystemBackdrop = new MicaBackdrop(),
            Content = new Page()
            {
                Content = new TextBlock()
                {
                    Text = "New child window!",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                // We need to set the RequestedTheme to match the app's theme.
                RequestedTheme = ThemeHelper.RootTheme,
            }
        };

        // We need to track the new window so it can be closed when the app is closing,
        // otherwise it will crash the app.
        WindowHelper.TrackWindow(childWindow);
        childWindow.AppWindow.ResizeClient(new SizeInt32(500, 500));
        childWindow.Activate();
    }
}
