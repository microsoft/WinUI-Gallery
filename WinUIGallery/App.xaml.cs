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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AppUIBasics.Common;
using AppUIBasics.Data;
using AppUIBasics.Helper;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Microsoft.UI.Windowing;
using Microsoft.Windows.AppLifecycle;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Window MainWindow { get; private set; }
        private readonly SemaphoreSlim _activationGate = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _saveGate = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _errorDialogGate = new SemaphoreSlim(1);
        private const string NavigationCheckpointKey = "WinUI3NavigationCheckpoint";
        private bool _windowReady;
        private bool _closing;
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            CrashDiagnostics.Install(this);
            this.InitializeComponent();
            this.FocusVisualKind = FocusVisualKind.HighVisibility;
        }

        public void EnableSound(bool withSpatial = false)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            if(!withSpatial)
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

        private async Task SaveSessionAsync()
        {
            await _saveGate.WaitAsync();
            try
            {
                await SuspensionManager.SaveAsync();
                ApplicationData.Current.LocalSettings.Values.Remove(NavigationCheckpointKey);
            }
            finally
            {
                _saveGate.Release();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}

            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
            }
#endif
            await ActivateAsync(Program.InitialActivation);
            Program.RegisterActivationHandler(MainWindow.DispatcherQueue, OnRedirectedActivation);
        }

        private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
        {
            
        }

        private async void OnRedirectedActivation(ActivationRequest args)
        {
            await ActivateAsync(args);
        }

        private async Task ActivateAsync(ActivationRequest args)
        {
            await _activationGate.WaitAsync();
            try
            {
                await EnsureWindow(args);
            }
            finally
            {
                _activationGate.Release();
            }
        }

        private async Task EnsureWindow(ActivationRequest args)
        {
            // No matter what our destination is, we're going to need control data loaded - let's knock that out now.
            // We'll never need to do this again.
            await ControlInfoDataSource.Instance.GetGroupsAsync();

            bool firstActivation = MainWindow == null;
            if (firstActivation)
            {
                MainWindow = WindowHelper.TrackWindow(new Window
                {
                    Title = "WinUI 3 Gallery",
                    ExtendsContentIntoTitleBar = true,
                    SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop()
                });
            }

            Frame rootFrame = GetRootFrame();
            if (firstActivation)
            {
                ThemeHelper.Initialize(MainWindow);
                MainWindow.AppWindow.Closing += OnWindowClosing;
                rootFrame.Navigated += OnRootFrameNavigated;
            }

            MainWindow.Activate();
            if (firstActivation && args.Kind == ExtendedActivationKind.Launch)
            {
                if (ApplicationData.Current.LocalSettings.Values[NavigationCheckpointKey] is ApplicationDataCompositeValue checkpoint)
                {
                    var pageName = checkpoint["Page"] as string;
                    var pageType = pageName == null ? null : typeof(App).Assembly.GetType(pageName);
                    if (pageType == null || !typeof(Page).IsAssignableFrom(pageType))
                    {
                        throw new InvalidOperationException("The saved navigation checkpoint names an unavailable Gallery page.");
                    }
                    object parameter = checkpoint["HasParameter"] is true ? checkpoint["Parameter"] : null;
                    rootFrame.Navigate(pageType, parameter);
                    UpdateNavigationBasedOnSelectedPage(rootFrame);
                }
                else if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("_sessionState.xml") != null)
                {
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                        UpdateNavigationBasedOnSelectedPage(rootFrame);
                    }
                    catch (SuspensionManagerException error)
                    {
                        await ShowStateErrorAsync("Saved navigation could not be restored. The Gallery will open its home page.", error);
                    }
                }
            }

            if (args.Kind == ExtendedActivationKind.Launch && rootFrame.Content != null)
            {
                _windowReady = true;
                return;
            }

            Type targetPageType = typeof(NewControlsPage);
            string targetPageArguments = string.Empty;

            if (args.Kind == ExtendedActivationKind.Launch)
            {
                targetPageArguments = args.LaunchArguments;
            }
            else if (args.Kind == ExtendedActivationKind.Protocol)
            {
                Match match;

                string targetId = string.Empty;

                Uri uri = args.ProtocolUri;
                switch (uri?.AbsoluteUri)
                {
                    case string s when IsMatching(s, "(/*)category/(.*)"):
                        targetId = match.Groups[2]?.ToString();
                        if (targetId == "AllControls")
                        {
                            targetPageType = typeof(AllControlsPage);
                        }
                        else if (targetId == "NewControls")
                        {
                            targetPageType = typeof(NewControlsPage);
                        }
                        else if (ControlInfoDataSource.Instance.Groups.Any(g => g.UniqueId == targetId))
                        {
                            targetPageType = typeof(SectionPage);
                        }
                        break;

                    case string s when IsMatching(s, "(/*)item/(.*)"):
                        targetId = match.Groups[2]?.ToString();
                        if (ControlInfoDataSource.Instance.Groups.Any(g => g.Items.Any(i => i.UniqueId == targetId)))
                        {
                            targetPageType = typeof(ItemPage);
                        }
                        break;
                }

                targetPageArguments = targetId;

                bool IsMatching(string parent, string expression)
                {
                    match = Regex.Match(parent, expression);
                    return match.Success;
                }
            }

            rootFrame.Navigate(targetPageType, targetPageArguments);

            if (targetPageType == typeof(NewControlsPage))
            {
                ((Microsoft.UI.Xaml.Controls.NavigationViewItem)((NavigationRootPage)MainWindow.Content).NavigationView.MenuItems[0]).IsSelected = true;
            }
            else if (targetPageType == typeof(ItemPage))
            {
                NavigationRootPage.Current.EnsureNavigationSelection(targetPageArguments);
            }

            // Ensure the current window is active
            MainWindow.Activate();
            _windowReady = true;
            SaveNavigationCheckpoint(targetPageType, targetPageArguments);
        }

        private static void UpdateNavigationBasedOnSelectedPage(Frame rootFrame)
        {
            // Check if we brought back an ItemPage
            if (rootFrame.Content is ItemPage itemPage)
            {
                // We did, so bring the selected item back into view
                string name = itemPage.Item.Title;
                if (MainWindow.Content is NavigationRootPage nav)
                {
                    // Finally brings back into view the correct item.
                    // But first: Update page layout!
                    nav.EnsureItemIsVisibleInNavigation(name);
                }
            }
        }

        private Frame GetRootFrame()
        {
            Frame rootFrame;
            if (!(MainWindow.Content is NavigationRootPage rootPage))
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

                MainWindow.Content = rootPage;
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

        private void OnRootFrameNavigated(object sender, NavigationEventArgs args)
        {
            if (!_windowReady)
            {
                return;
            }

            SaveNavigationCheckpoint(args.SourcePageType, args.Parameter);
        }

        private static void SaveNavigationCheckpoint(Type pageType, object parameter)
        {
            if (parameter != null && parameter is not string)
            {
                ApplicationData.Current.LocalSettings.Values.Remove(NavigationCheckpointKey);
                Trace.TraceWarning("Navigation checkpoint cannot persist parameter type {0}. Full history will be saved on close.", parameter.GetType());
                return;
            }

            // Frame.GetNavigationState invokes page teardown, so only use it when closing.
            ApplicationData.Current.LocalSettings.Values[NavigationCheckpointKey] = new ApplicationDataCompositeValue
            {
                ["Page"] = pageType.FullName,
                ["HasParameter"] = parameter != null,
                ["Parameter"] = parameter as string ?? string.Empty
            };
        }

        private async void OnWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            args.Cancel = true;
            if (_closing)
            {
                return;
            }

            _closing = true;
            try
            {
                await SaveSessionAsync();
                MainWindow.AppWindow.Closing -= OnWindowClosing;
                foreach (var window in WindowHelper.Windows.Where(window => window != MainWindow).ToArray())
                {
                    window.Close();
                }
                MainWindow.Close();
            }
            catch (SuspensionManagerException error)
            {
                _closing = false;
                await ShowStateErrorAsync("The Gallery could not save its navigation state. The window has been kept open.", error);
            }
        }

        private async Task ShowStateErrorAsync(string message, Exception error)
        {
            Trace.TraceError("{0} {1}", message, error);
            await _errorDialogGate.WaitAsync();
            try
            {
                await new ContentDialog
                {
                    XamlRoot = MainWindow.Content.XamlRoot,
                    Title = "Navigation state",
                    Content = message,
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
            finally
            {
                _errorDialogGate.Release();
            }
        }
    }
}
