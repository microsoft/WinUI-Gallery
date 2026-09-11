// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using Windows.Graphics;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class WindowingPage : Page
{
    public WindowingPage()
    {
        InitializeComponent();
    }

    private void CreateNewWindow_Click(object sender, RoutedEventArgs e)
    {
        Window childWindow = new Window
        {
            ExtendsContentIntoTitleBar = true,
            SystemBackdrop = new MicaBackdrop(),
            Content = new Page
            {
                RequestedTheme = ActualTheme,
                Content = new TextBlock
                {
                    Text = "New child window!",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            },
        };

        WindowHelper.TrackWindow(childWindow);
        // ResizeClient takes physical pixels, independent of display scaling.
        childWindow.AppWindow.ResizeClient(new SizeInt32(500, 500));
        childWindow.Activate();
    }

    private void OpenSizedWindow_Click(object sender, RoutedEventArgs e)
    {
        // Non-editable options contain only finite, positive integer sizes.
        if (ClientWidthOption.SelectedItem is not int width ||
            ClientHeightOption.SelectedItem is not int height)
        {
            return;
        }

        Window window = CreateSampleWindow(
            "Window.Width / Height (experimental)",
            $"Initial client area: {width} by {height} DIPs. You can resize this window. Close it with Alt+F4 or the title bar Close button.");
        window.Width = width;
        window.Height = height;

        // Activate applies the pending Window size before the first show.
        window.Activate();
    }

    private void OpenConstrainedWindow_Click(object sender, RoutedEventArgs e)
    {
        // Every offered range includes the initial 640 by 480 DIP client area.
        if (MinimumWidthOption.SelectedItem is not int minWidth ||
            MinimumHeightOption.SelectedItem is not int minHeight ||
            MaximumWidthOption.SelectedItem is not int maxWidth ||
            MaximumHeightOption.SelectedItem is not int maxHeight)
        {
            Debug.WriteLine("Select minimum and maximum client sizes before opening the sample window.");
            return;
        }

        Window window = CreateSampleWindow(
            "Window size constraints (experimental)",
            $"Client width: {minWidth} to {maxWidth} DIPs. Client height: {minHeight} to {maxHeight} DIPs. Try resizing this window. Close it with Alt+F4 or the title bar Close button.");
        window.MinWidth = minWidth;
        window.MinHeight = minHeight;
        window.MaxWidth = maxWidth;
        window.MaxHeight = maxHeight;
        window.Width = 640;
        window.Height = 480;

        if (window.AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
        }

        // Activate applies the pending Window size before the first show.
        window.Activate();
    }

    private Window CreateSampleWindow(string title, string message)
    {
        // The Gallery helper tracks each window and removes it when it closes.
        // Gallery shutdown also closes any remaining tracked child windows.
        Window window = WindowHelper.CreateWindow();
        window.Title = title;
        window.SystemBackdrop = new MicaBackdrop();
        window.Content = new Page
        {
            RequestedTheme = ThemeHelper.RootTheme,
            Content = new TextBlock
            {
                Margin = new Thickness(24),
                Text = message,
                TextWrapping = TextWrapping.Wrap
            }
        };
        return window;
    }
}
