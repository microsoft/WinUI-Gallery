// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
            Content = new TextBlock
            {
                Text = "New child window!",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RequestedTheme = ActualTheme
            },
        };

        WindowHelper.TrackWindow(childWindow);
        // ResizeClient takes physical pixels, independent of display scaling.
        childWindow.AppWindow.ResizeClient(new SizeInt32(500, 500));
        childWindow.Activate();
    }

    private void OpenConfiguredWindow_Click(object sender, RoutedEventArgs e)
    {
        double width = ClientWidthInput.Value;
        double height = ClientHeightInput.Value;
        double minWidth = MinimumWidthInput.Value;
        double minHeight = MinimumHeightInput.Value;
        double maxWidth = MaximumWidthInput.Value;
        double maxHeight = MaximumHeightInput.Value;

        if (!double.IsFinite(width) ||
            !double.IsFinite(height) ||
            !double.IsFinite(minWidth) ||
            !double.IsFinite(minHeight) ||
            !double.IsFinite(maxWidth) ||
            !double.IsFinite(maxHeight))
        {
            SizeValidationInfoBar.Message = "Enter a value for every dimension.";
            SizeValidationInfoBar.IsOpen = true;
            return;
        }

        if (minWidth > width || width > maxWidth || minHeight > height || height > maxHeight)
        {
            SizeValidationInfoBar.Message = "Width and Height must be within their minimum and maximum limits.";
            SizeValidationInfoBar.IsOpen = true;
            return;
        }

        SizeValidationInfoBar.IsOpen = false;

        Window window = CreateSampleWindow(
            "Window client size and constraints (experimental)",
            $"Initial client area: {width} by {height} DIPs. Width is constrained to {minWidth} to {maxWidth} DIPs and height to {minHeight} to {maxHeight} DIPs. Try resizing this window.");
        window.MinWidth = minWidth;
        window.MinHeight = minHeight;
        window.MaxWidth = maxWidth;
        window.MaxHeight = maxHeight;
        window.Width = width;
        window.Height = height;

        if (window.AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
        }

        // Activate applies the pending Window size and constraints before the first show.
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
