//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.BadgeNotifications;
using Windows.ApplicationModel.Activation;
using WinUIGallery.Helpers;
using WinUIGallery.Pages;
using static WinUIGallery.Helpers.Win32;

namespace WinUIGallery;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
sealed partial class App : Application
{
    private static Window startupWindow;
    private static Win32WindowHelper win32WindowHelper;
    private static int registeredKeyPressedHook = 0;
    private HookProc keyEventHook;

    /// <summary>
    /// Get the initial window created for this app.
    /// </summary>
    public static Window StartupWindow
    {
        get => startupWindow;
    }

    /// <summary>
    /// Initializes the singleton Application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        UnhandledException += HandleExceptions;
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        IdleSynchronizer.Init();

        startupWindow = WindowHelper.CreateWindow();
        startupWindow.ExtendsContentIntoTitleBar = true;

        win32WindowHelper = new Win32WindowHelper(startupWindow);
        win32WindowHelper.SetWindowMinMaxSize(new Win32WindowHelper.POINT() { x = 532, y = 500 });

#if DEBUG
        if (Debugger.IsAttached)
        {
            DebugSettings.BindingFailed += DebugSettings_BindingFailed;
        }
#endif

        keyEventHook = KeyEventHook;
        registeredKeyPressedHook = SetWindowKeyHook(keyEventHook);

        EnsureWindow();

        startupWindow.Closed += (s, e) =>
        {
            BadgeNotificationManager.Current.ClearBadge();
        };
    }

    private int KeyEventHook(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && IsKeyDownHook(lParam))
        {
            RootFrameNavigationHelper.RaiseKeyPressed((uint)wParam);
        }

        return CallNextHookEx(registeredKeyPressedHook, nCode, wParam, lParam);
    }

    private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
    {
        // Ignore the exception from NonExistentProperty in BindingPage.xaml, 
        // as the sample code intentionally includes a binding failure.
        if (e.Message.Contains("NonExistentProperty"))
        {
            return;
        }

        throw new Exception($"A debug binding failed: " + e.Message);
    }

    private async void EnsureWindow()
    {
        await ControlInfoDataSource.Instance.GetGroupsAsync();
        await IconsDataSource.Instance.LoadIcons();

        Frame rootFrame = GetRootFrame();

        ThemeHelper.Initialize();

        var targetPageType = typeof(HomePage);
        var targetPageArguments = string.Empty;

        AppActivationArguments eventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        if (eventArgs != null && eventArgs.Kind == ExtendedActivationKind.Protocol && eventArgs.Data is ProtocolActivatedEventArgs)
        {
            var ProtocolArgs = eventArgs.Data as ProtocolActivatedEventArgs;
            string uri = ProtocolArgs.Uri.LocalPath.Replace("/", "");
            targetPageArguments = uri;

            if (uri == "AllControls")
            {
                targetPageType = typeof(AllControlsPage);
            }
            else if (uri == "NewControls")
            {
                targetPageType = typeof(HomePage);
            }
            else if (ControlInfoDataSource.Instance.Groups.Any(g => g.UniqueId == uri))
            {
                targetPageType = typeof(SectionPage);
            }
            else if (ControlInfoDataSource.Instance.Groups.Any(g => g.Items.Any(i => i.UniqueId == uri)))
            {
                targetPageType = typeof(ItemPage);
            }
        }

        var rootPage = StartupWindow.Content as NavigationRootPage;
        rootPage.Navigate(targetPageType, targetPageArguments);

        if (targetPageType == typeof(HomePage))
        {
            var navItem = (NavigationViewItem)rootPage.NavigationView.MenuItems[0];
            navItem.IsSelected = true;
        }

        // Activate the startup window.
        StartupWindow.Activate();
    }

    /// <summary>
    /// Gets the frame of the StartupWindow.
    /// </summary>
    /// <returns>The frame of the StartupWindow.</returns>
    /// <exception cref="Exception">Thrown if the window doesn't have a frame with the name "rootFrame".</exception>
    public Frame GetRootFrame()
    {
        Frame rootFrame;
        if (StartupWindow.Content is NavigationRootPage rootPage)
        {
            rootFrame = (Frame)rootPage.FindName("rootFrame");
        }
        else
        {
            rootPage = new NavigationRootPage();
            rootFrame = (Frame)rootPage.FindName("rootFrame");
            if (rootFrame == null)
            {
                throw new Exception("Root frame not found");
            }
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
            rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
            rootFrame.NavigationFailed += OnNavigationFailed;

            StartupWindow.Content = rootPage;
        }

        return rootFrame;
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    /// <summary>
    /// Prevents the app from crashing when a exception gets thrown and notifies the user.
    /// </summary>
    /// <param name="sender">The app as an object.</param>
    /// <param name="e">Details about the exception.</param>
    private void HandleExceptions(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true; //Don't crash the app.

        //Create the notification.
        var notification = new AppNotificationBuilder()
            .AddText("An exception was thrown.")
            .AddText($"Type: {e.Exception.GetType()}")
            .AddText($"Message: {e.Message}\r\n" +
                     $"HResult: {e.Exception.HResult}")
            .BuildNotification();

        //Show the notification
        AppNotificationManager.Default.Show(notification);
    }
}
