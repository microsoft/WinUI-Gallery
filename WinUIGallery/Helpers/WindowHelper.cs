// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUIGallery.Helpers;

// Helper class to allow the app to find the Window that contains an
// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
// of all active Windows.  The app code must call WindowHelper.CreateWindow
// rather than "new Window" so we can keep track of all the relevant
// windows.  In the future, we would like to support this in platform APIs.
public partial class WindowHelper
{
    static public Window CreateWindow()
    {
        MainWindow newWindow = new MainWindow();
        TrackWindow(newWindow);
        return newWindow;
    }

    static public void TrackWindow(Window window)
    {
        window.Closed += (sender, args) =>
        {
            _activeWindows.Remove(window);
        };
        _activeWindows.Add(window);
    }

    static public Window? GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (Window window in _activeWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return window;
                }
            }
        }
        return null;
    }
    // get dpi for an element
    static public double GetRasterizationScaleForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (Window window in _activeWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return element.XamlRoot.RasterizationScale;
                }
            }
        }
        return 0.0;
    }

    static public void SetWindowMinSize(Window window, double width, double height)
    {
        if (window.Content is not FrameworkElement windowContent)
        {
            System.Diagnostics.Debug.WriteLine("Window content is not a FrameworkElement.");
            return;
        }

        if (windowContent.XamlRoot is null)
        {
            System.Diagnostics.Debug.WriteLine("Window content's XamlRoot is null.");
            return;
        }

        if (window.AppWindow.Presenter is not OverlappedPresenter presenter)
        {
            System.Diagnostics.Debug.WriteLine("Window's AppWindow.Presenter is not an OverlappedPresenter.");
            return;
        }

        var scale = windowContent.XamlRoot.RasterizationScale;
        var minWidth = width * scale;
        var minHeight = height * scale;
        presenter.PreferredMinimumWidth = (int)minWidth;
        presenter.PreferredMinimumHeight = (int)minHeight;
    }

    static public List<Window> ActiveWindows { get { return _activeWindows; } }

    static private List<Window> _activeWindows = new List<Window>();

    static public StorageFolder GetAppLocalFolder()
    {
        StorageFolder localFolder;
        if (!NativeMethods.IsAppPackaged)
        {
            localFolder = Task.Run(async () => await StorageFolder.GetFolderFromPathAsync(System.AppContext.BaseDirectory)).Result;
        }
        else
        {
            localFolder = ApplicationData.Current.LocalFolder;
        }
        return localFolder;
    }
}
