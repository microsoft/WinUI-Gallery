// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;

namespace WinUIGallery.UnitTests;

public partial class UnitTestApp : Application
{
    public UnitTestApp()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();

        s_window = new UnitTestAppWindow();
        s_window.Activate();

        UITestMethodAttribute.DispatcherQueue = s_window.DispatcherQueue;

        Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
    }

    private static UnitTestAppWindow s_window;

    public static UnitTestAppWindow UnitTestAppWindow
    {
        get { return s_window; }
    }
}
