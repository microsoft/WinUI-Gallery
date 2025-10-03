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

    private void createNewWindow_Click(object sender, RoutedEventArgs e)
    {
        var newWindow = new Window
        {
            ExtendsContentIntoTitleBar = true,
            SystemBackdrop = new MicaBackdrop(),
            Content = new Page
            {
                // The TreeHelper is a helper class in the WinUIGallery project
                // that allows us to find the current theme of the app.
                RequestedTheme = ThemeHelper.RootTheme,
                Content = new TextBlock
                {
                    Text = "New Window!",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            }
        };

        newWindow.AppWindow.ResizeClient(new SizeInt32(500, 500));

        // The WindowHelper is a helper class in the WinUIGallery project
        // that helps us close child windows when the main window closes.
        WindowHelper.TrackWindow(newWindow);
        newWindow.Activate();
    }
}
