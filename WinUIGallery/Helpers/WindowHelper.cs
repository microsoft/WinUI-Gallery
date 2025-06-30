//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using WinRT.Interop;

namespace WinUIGallery.Helpers;

// Helper class to allow the app to find the Window that contains an
// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
// of all active Windows.  The app code must call WindowHelper.CreateWindow
// rather than "new Window" so we can keep track of all the relevant
// windows.  In the future, we would like to support this in platform APIs.
public class WindowHelper
{
    static public Window CreateWindow()
    {
        MainWindow newWindow = new MainWindow();
        TrackWindow(newWindow);
        return newWindow;
    }

    static public void TrackWindow(Window window)
    {
        window.Closed += (sender,args) => {
            _activeWindows.Remove(window);
        };
        _activeWindows.Add(window);
    }

    static public Window GetWindowForElement(UIElement element)
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

    static public List<Window> ActiveWindows { get { return _activeWindows; }}

    static private List<Window> _activeWindows = new List<Window>();

    static public StorageFolder GetAppLocalFolder()
    {
        StorageFolder localFolder;
        if (!NativeHelper.IsAppPackaged)
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
