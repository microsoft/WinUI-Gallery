// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
            // Size the client area in device-independent pixels.
            Width = 500,
            Height = 500,
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
                // Get the theme from the parent.
                RequestedTheme = this.ActualTheme,
            }
        };

        // We need to track the new window so it can be closed when the app is closing,
        // otherwise it will crash the app.
        // This is also used to change the theme for all windows when the app theme changes.
        WindowHelper.TrackWindow(childWindow);
        childWindow.Activate();
    }
}
