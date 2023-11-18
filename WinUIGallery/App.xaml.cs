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
using System.Reflection;
using AppUIBasics.Common;
using AppUIBasics.Data;
using AppUIBasics.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using WinUIGallery.DesktopWap.DataModel;
using WASDK = Microsoft.WindowsAppSDK;

namespace AppUIBasics
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static Window startupWindow;

        public static string WinAppSdkDetails
        {
            // TODO: restore patch number and version tag when WinAppSDK supports them both
            get => string.Format("Windows App SDK {0}.{1}",
                WASDK.Release.Major, WASDK.Release.Minor);
        }

        public static string WinAppSdkRuntimeDetails
        {
            get
            {
                // Retrieve Windows App Runtime version info dynamically
                var windowsAppRuntimeVersion =
                    from module in Process.GetCurrentProcess().Modules.OfType<ProcessModule>()
                    where module.FileName.EndsWith("Microsoft.WindowsAppRuntime.Insights.Resource.dll")
                    select FileVersionInfo.GetVersionInfo(module.FileName);
                return WinAppSdkDetails + ", Windows App Runtime " + windowsAppRuntimeVersion.First().FileVersion; 
            }
        }

        // Get the initial window created for this app
        // On UWP, this is simply Window.Current
        // On Desktop, multiple Windows may be created, and the StartupWindow may have already
        // been closed.
        public static Window StartupWindow
        {
            get
            {
                return startupWindow;
            }
        }
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

#if WINUI_PRERELEASE
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
#endif
        }

        public void EnableSound(bool withSpatial = false)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            if (!withSpatial)
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            else
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
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
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
            }
#endif

            EnsureWindow();
        }

        private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
        {

        }

#if WINUI_PRERELEASE
        protected override void OnActivated(IActivatedEventArgs args)
        {
            EnsureWindow(args);
        }
#endif

        private async void EnsureWindow(IActivatedEventArgs args = null)
        {
            // No matter what our destination is, we're going to need control data loaded - let's knock that out now.
            // We'll never need to do this again.
            await ControlInfoDataSource.Instance.GetGroupsAsync();
            await IconsDataSource.Instance.LoadIcons();

            Frame rootFrame = GetRootFrame();

            ThemeHelper.Initialize();

            Type targetPageType = typeof(HomePage);
            string targetPageArguments = string.Empty;

            if (args != null)
            {
                if (args.Kind == ActivationKind.Launch)
                {
                    if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        try
                        {
                            await SuspensionManager.RestoreAsync();
                        }
                        catch (SuspensionManagerException)
                        {
                            //Something went wrong restoring state.
                            //Assume there is no state and continue
                        }
                    }

                    targetPageArguments = ((Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)args).Arguments;
                }
            }
            var eventargs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            if (eventargs != null && eventargs.Kind is ExtendedActivationKind.Protocol && eventargs.Data is ProtocolActivatedEventArgs)
            {
                ProtocolActivatedEventArgs ProtocolArgs = eventargs.Data as ProtocolActivatedEventArgs;
                var uri = ProtocolArgs.Uri.LocalPath.Replace("/", "");

                targetPageArguments = uri;
                string targetId = string.Empty;

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

            NavigationRootPage rootPage = StartupWindow.Content as NavigationRootPage;
            rootPage.Navigate(targetPageType, targetPageArguments);

            if (targetPageType == typeof(HomePage))
            {
                ((Microsoft.UI.Xaml.Controls.NavigationViewItem)((NavigationRootPage)App.StartupWindow.Content).NavigationView.MenuItems[0]).IsSelected = true;
            }

            // Ensure the current window is active
            StartupWindow.Activate();
        }

        public Frame GetRootFrame()
        {
            Frame rootFrame;
            NavigationRootPage rootPage = StartupWindow.Content as NavigationRootPage;
            if (rootPage == null)
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
            else
            {
                rootFrame = (Frame)rootPage.FindName("rootFrame");
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

#if WINUI_PRERELEASE
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
#endif // WINUI_PRERELEASE
    }
}
