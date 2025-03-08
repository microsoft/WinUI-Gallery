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
    internal static MainWindow MainWindow { get; private set; } = null!;

    private static Win32WindowHelper win32WindowHelper;
    private static int registeredKeyPressedHook = 0;
    private HookProc keyEventHook;

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

        MainWindow = new MainWindow();
        WindowHelper.TrackWindow(MainWindow);

        win32WindowHelper = new Win32WindowHelper(MainWindow);
        win32WindowHelper.SetWindowMinMaxSize(new Win32WindowHelper.POINT() { x = 500, y = 500 });

#if DEBUG
        if (Debugger.IsAttached)
        {
            DebugSettings.BindingFailed += DebugSettings_BindingFailed;
        }
#endif

        keyEventHook = KeyEventHook;
        registeredKeyPressedHook = SetWindowKeyHook(keyEventHook);

        EnsureWindow();
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
        MainWindow.AddNavigationMenuItems();
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

        MainWindow.Navigate(targetPageType, targetPageArguments);

        if (targetPageType == typeof(HomePage))
        {
            var navItem = (NavigationViewItem)MainWindow.NavigationView.MenuItems[0];
            navItem.IsSelected = true;
        }

        // Activate the startup window.
        MainWindow.Activate();
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
