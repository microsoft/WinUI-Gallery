// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using WinUIGallery.Models;

namespace WinUIGallery.Helpers;

public static class WindowStateHelper
{
    private static void SaveWindowState(WindowState state)
    {
        var s = SettingsHelper.Current;

        s.MainWindowPositionX = state.PositionX;
        s.MainWindowPositionY = state.PositionY;
        s.MainWindowWidth = state.Width;
        s.MainWindowHeight = state.Height;
        s.MainWindowScale = state.Scale;
        s.IsMainWindowMaximized = state.IsMaximized;
    }

    private static WindowState? LoadWindowState()
    {
        var s = SettingsHelper.Current;

        bool isInvalid =
            s.MainWindowWidth <= 0 ||
            s.MainWindowHeight <= 0 ||
            (
                s.MainWindowPositionX == 0 &&
                s.MainWindowPositionY == 0 &&
                s.MainWindowWidth == 0 &&
                s.MainWindowHeight == 0 &&
                s.MainWindowScale == 1.0 &&
                !s.IsMainWindowMaximized
            );

        if (isInvalid)
            return null;

        return new WindowState
        {
            PositionX = s.MainWindowPositionX,
            PositionY = s.MainWindowPositionY,
            Width = s.MainWindowWidth,
            Height = s.MainWindowHeight,
            Scale = s.MainWindowScale,
            IsMaximized = s.IsMainWindowMaximized
        };
    }

    public static void Save(Window window)
    {
        var appWindow = window.AppWindow;

        double scale = window.Content.XamlRoot.RasterizationScale;
        bool isMaximized = appWindow.Presenter is OverlappedPresenter p &&
                           p.State == OverlappedPresenterState.Maximized;

        // Restore before saving accurate dimensions
        (appWindow.Presenter as OverlappedPresenter)?.Restore();

        var state = new WindowState
        {
            PositionX = appWindow.Position.X,
            PositionY = appWindow.Position.Y,
            Width = appWindow.Size.Width,
            Height = appWindow.Size.Height,
            Scale = scale,
            IsMaximized = isMaximized
        };

        SaveWindowState(state);
    }

    public static void ApplySavedState(Window window)
    {
        if (window.Content is FrameworkElement content)
        {
            double currentScale = content.XamlRoot.RasterizationScale;
            var state = LoadWindowState();
            if (state is null) return;

            // adjust for DPI/scaling change
            double scaleFactor = currentScale / state.Scale;
            int width = (int)(state.Width * scaleFactor);
            int height = (int)(state.Height * scaleFactor);
            int x = state.PositionX;
            int y = state.PositionY;

            // ensure window fits display area
            var displayArea = DisplayArea.GetFromWindowId(window.AppWindow.Id, DisplayAreaFallback.Primary).WorkArea;
            width = Math.Min(width, displayArea.Width);
            height = Math.Min(height, displayArea.Height);
            x = Math.Clamp(x, displayArea.X, displayArea.X + displayArea.Width - width);
            y = Math.Clamp(y, displayArea.Y, displayArea.Y + displayArea.Height - height);

            window.AppWindow.Move(new PointInt32(x, y));
            window.AppWindow.Resize(new SizeInt32(width, height));

            if (state.IsMaximized)
                (window.AppWindow.Presenter as OverlappedPresenter)?.Maximize();
        }
    }
}
