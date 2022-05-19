//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Common;
using AppUIBasics.Data;
using AppUIBasics.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Gaming.Input;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Microsoft.UI.Dispatching;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics
{
    public sealed partial class NavigationRootPage : Page
    {
        public Windows.System.VirtualKey ArrowKey;

        private RootFrameNavigationHelper _navHelper;
        private bool _isGamePadConnected;
        private bool _isKeyboardConnected;
        private Microsoft.UI.Xaml.Controls.NavigationViewItem _allControlsMenuItem;
        private Microsoft.UI.Xaml.Controls.NavigationViewItem _newControlsMenuItem;

        public static NavigationRootPage GetForElement(object obj)
        {
            UIElement element = (UIElement)obj;
            Window window = WindowHelper.GetWindowForElement(element);
            if (window != null)
            {
                return (NavigationRootPage)window.Content;
            }
            return null;
        }

        public Microsoft.UI.Xaml.Controls.NavigationView NavigationView
        {
            get { return NavigationViewControl; }
        }

        public Action NavigationViewLoaded { get; set; }

        public DeviceType DeviceFamily { get; set; }

        public bool IsFocusSupported
        {
            get
            {
                return DeviceFamily == DeviceType.Xbox || _isGamePadConnected || _isKeyboardConnected;
            }
        }

        public PageHeader PageHeader
        {
            get
            {
                return UIHelper.GetDescendantsOfType<PageHeader>(NavigationViewControl).FirstOrDefault();
            }
        }

        public string AppTitleText
        {
            get
            {
#if !UNIVERSAL
                return "WinUI 3 Gallery";
#else
                return "WinUI 3 Gallery (UWP)";
#endif
            }
        }

        public NavigationRootPage()
        {
            this.InitializeComponent();

            // Workaround for VisualState issue that should be fixed
            // by https://github.com/microsoft/microsoft-ui-xaml/pull/2271
            NavigationViewControl.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;

            _navHelper = new RootFrameNavigationHelper(rootFrame, NavigationViewControl);

            SetDeviceFamily();
            AddNavigationMenuItems();

            this.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                // helpful for debugging focus problems w/ keyboard & gamepad
                if (FocusManager.GetFocusedElement() is FrameworkElement focus)
                {
                    Debug.WriteLine("got focus: " + focus.Name + " (" + focus.GetType().ToString() + ")");
                }
            };

            Gamepad.GamepadAdded += OnGamepadAdded;
            Gamepad.GamepadRemoved += OnGamepadRemoved;

#if UNIVERSAL
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateAppTitle(s);
#endif

            _isKeyboardConnected = Convert.ToBoolean(new KeyboardCapabilities().KeyboardPresent);

            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            Loaded += delegate (object sender, RoutedEventArgs e)
            {
                NavigationOrientationHelper.UpdateTitleBarForElement(NavigationOrientationHelper.IsLeftMode(), this);
#if !UNIVERSAL
                WindowHelper.GetWindowForElement(this).Title = AppTitleText;
#endif
            };

            NavigationViewControl.RegisterPropertyChangedCallback(NavigationView.PaneDisplayModeProperty, new DependencyPropertyChangedCallback(OnPaneDisplayModeChanged));

            // Set the titlebar to be custom. This is also referenced by the TitlebarPage
            App.appTitlebar = AppTitleBar;
        }

        private void OnPaneDisplayModeChanged(DependencyObject sender, DependencyProperty dp)
        {
            var navigationView = sender as NavigationView;
            NavigationRootPage.GetForElement(this).AppTitleBar.Visibility = navigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top ? Visibility.Collapsed : Visibility.Visible;
        }

        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness() { Left = currMargin.Left, Top = currMargin.Top, Right = coreTitleBar.SystemOverlayRightInset, Bottom = currMargin.Bottom };
        }

        public bool CheckNewControlSelected()
        {
            return _newControlsMenuItem.IsSelected;
        }

        // Wraps a call to rootFrame.Navigate to give the Page a way to know which NavigationRootPage is navigating.
        // Please call this function rather than rootFrame.Navigate to navigate the rootFrame.
        public void Navigate(
            Type pageType,
            object targetPageArguments = null,
            Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo navigationTransitionInfo = null)
        {
            NavigationRootPageArgs args = new NavigationRootPageArgs();
            args.NavigationRootPage = this;
            args.Parameter = targetPageArguments;
            rootFrame.Navigate(pageType, args, navigationTransitionInfo);
        }

#if WINUI_PRERELEASE
        public void App_Resuming()
        {
#if UNIVERSAL
            switch (rootFrame?.Content)
            {
                case ItemPage itemPage:
                    itemPage.SetInitialVisuals();
                    break;
                case NewControlsPage newControlsPage:
                case AllControlsPage allControlsPage:
                    NavigationView.AlwaysShowHeader = false;
                    break;
            }
#endif
        }
#endif

        public void EnsureNavigationSelection(string id)
        {
            foreach (object rawGroup in this.NavigationView.MenuItems)
            {
                if (rawGroup is NavigationViewItem group)
                {
                    foreach (object rawItem in group.MenuItems)
                    {
                        if (rawItem is NavigationViewItem item)
                        {
                            if ((string)item.Tag == id)
                            {
                                group.IsExpanded = true;
                                NavigationView.SelectedItem = item;
                                item.IsSelected = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void AddNavigationMenuItems()
        {
            foreach (var group in ControlInfoDataSource.Instance.Groups.OrderBy(i => i.Title))
            {
                var itemGroup = new Microsoft.UI.Xaml.Controls.NavigationViewItem() { Content = group.Title, Tag = group.UniqueId, DataContext = group, Icon = GetIcon(group.ImageIconPath) };

                var groupMenuFlyoutItem = new MenuFlyoutItem() { Text = $"Copy Link to {group.Title} Samples", Icon = new FontIcon() { Glyph = "\uE8C8" }, Tag = group };
                groupMenuFlyoutItem.Click += this.OnMenuFlyoutItemClick;
                itemGroup.ContextFlyout = new MenuFlyout() { Items = { groupMenuFlyoutItem } };

                AutomationProperties.SetName(itemGroup, group.Title);

                foreach (var item in group.Items)
                {
                    var itemInGroup = new Microsoft.UI.Xaml.Controls.NavigationViewItem() { IsEnabled = item.IncludedInBuild, Content = item.Title, Tag = item.UniqueId, DataContext = item, Icon = GetIcon(item.ImageIconPath) };

                    var itemInGroupMenuFlyoutItem = new MenuFlyoutItem() { Text = $"Copy Link to {item.Title} Sample", Icon = new FontIcon() { Glyph = "\uE8C8" }, Tag = item };
                    itemInGroupMenuFlyoutItem.Click += this.OnMenuFlyoutItemClick;
                    itemInGroup.ContextFlyout = new MenuFlyout() { Items = { itemInGroupMenuFlyoutItem } };

                    itemGroup.MenuItems.Add(itemInGroup);
                    AutomationProperties.SetName(itemInGroup, item.Title);
                }

                NavigationViewControl.MenuItems.Add(itemGroup);

                if (group.UniqueId == "AllControls")
                {
                    this._allControlsMenuItem = itemGroup;
                }
                else if (group.UniqueId == "NewControls")
                {
                    this._newControlsMenuItem = itemGroup;
                }
            }

            // Move "What's New" and "All Controls" to the top of the NavigationView
            NavigationViewControl.MenuItems.Remove(_allControlsMenuItem);
            NavigationViewControl.MenuItems.Remove(_newControlsMenuItem);
            NavigationViewControl.MenuItems.Insert(0, _allControlsMenuItem);
            NavigationViewControl.MenuItems.Insert(0, _newControlsMenuItem);

            // Separate the All/New items from the rest of the categories.
            NavigationViewControl.MenuItems.Insert(2, new Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator());

            _newControlsMenuItem.Loaded += OnNewControlsMenuItemLoaded;
        }

        private void OnMenuFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuFlyoutItem).Tag)
            {
                case ControlInfoDataItem item:
                    ProtocolActivationClipboardHelper.Copy(item);
                    return;
                case ControlInfoDataGroup group:
                    ProtocolActivationClipboardHelper.Copy(group);
                    return;
            }
        }

        private static IconElement GetIcon(string imagePath)
        {
            return imagePath.ToLowerInvariant().EndsWith(".png") ?
                        (IconElement)new BitmapIcon() { UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute) , ShowAsMonochrome = false} :
                        (IconElement)new FontIcon()
                        {
                           // FontFamily = new FontFamily("Segoe MDL2 Assets"),
                            Glyph = imagePath
                        };
        }

        private void SetDeviceFamily()
        {
            var familyName = AnalyticsInfo.VersionInfo.DeviceFamily;

            if (!Enum.TryParse(familyName.Replace("Windows.", string.Empty), out DeviceType parsedDeviceType))
            {
                parsedDeviceType = DeviceType.Other;
            }

            DeviceFamily = parsedDeviceType;
        }

        private void OnNewControlsMenuItemLoaded(object sender, RoutedEventArgs e)
        {
            if (IsFocusSupported && NavigationViewControl.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded)
            {
                controlsSearchBox.Focus(FocusState.Keyboard);
            }
        }

        private void OnGamepadRemoved(object sender, Gamepad e)
        {
            _isGamePadConnected = Gamepad.Gamepads.Any();
        }

        private void OnGamepadAdded(object sender, Gamepad e)
        {
            _isGamePadConnected = Gamepad.Gamepads.Any();
        }

        private void OnNavigationViewControlLoaded(object sender, RoutedEventArgs e)
        {
            // Delay necessary to ensure NavigationView visual state can match navigation
            Task.Delay(500).ContinueWith(_ => this.NavigationViewLoaded?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnNavigationViewSelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            // Close any open teaching tips before navigation
            CloseTeachingTips();

            if (args.IsSettingsSelected)
            {
                if (rootFrame.CurrentSourcePageType != typeof(SettingsPage))
                {
                    Navigate(typeof(SettingsPage));
                }
            }
            else
            {
                var selectedItem = args.SelectedItemContainer;

                if (selectedItem == _allControlsMenuItem)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(AllControlsPage))
                    {
                        Navigate(typeof(AllControlsPage));
                    }
                }
                else if (selectedItem == _newControlsMenuItem)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(NewControlsPage))
                    {
                        Navigate(typeof(NewControlsPage));
                    }
                }
                else
                {
                    if (selectedItem.DataContext is ControlInfoDataGroup)
                    {
                        var itemId = ((ControlInfoDataGroup)selectedItem.DataContext).UniqueId;
                        Navigate(typeof(SectionPage), itemId);
                    }
                    else if (selectedItem.DataContext is ControlInfoDataItem)
                    {
                        var item = (ControlInfoDataItem)selectedItem.DataContext;
                        Navigate(typeof(ItemPage), item.UniqueId);
                    }

                }
            }
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Close any open teaching tips before navigation
            CloseTeachingTips();

            if (e.SourcePageType == typeof(AllControlsPage) ||
                e.SourcePageType == typeof(NewControlsPage))
            {
                NavigationViewControl.AlwaysShowHeader = false;
            }
            else
            {
                NavigationViewControl.AlwaysShowHeader = true;
            }

            TestContentLoadedCheckBox.IsChecked = true;
        }

        private void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            TestContentLoadedCheckBox.IsChecked = false;
        }

        private void CloseTeachingTips()
        {
            if (PageHeader != null)
            {
                PageHeader.TeachingTip1.IsOpen = false;
                PageHeader.TeachingTip3.IsOpen = false;
            }
        }

        private void OnControlsSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suggestions = new List<ControlInfoDataItem>();

                var querySplit = sender.Text.Split(" ");
                foreach (var group in ControlInfoDataSource.Instance.Groups)
                {
                    var matchingItems = group.Items.Where(
                        item =>
                        {
                            // Idea: check for every word entered (separated by space) if it is in the name, 
                            // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button"
                            // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words
                            bool flag = item.IncludedInBuild;
                            foreach (string queryToken in querySplit)
                            {
                                // Check if token is not in string
                                if (item.Title.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                                {
                                    // Token is not in string, so we ignore this item.
                                    flag = false;
                                }
                            }
                            return flag;
                        });
                    foreach (var item in matchingItems)
                    {
                        suggestions.Add(item);
                    }
                }
                if (suggestions.Count > 0)
                {
                    controlsSearchBox.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title);
                }
                else
                {
                    controlsSearchBox.ItemsSource = new string[] { "No results found" };
                }
            }
        }

        private void OnControlsSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is ControlInfoDataItem)
            {
                var infoDataItem = args.ChosenSuggestion as ControlInfoDataItem;
                var itemId = infoDataItem.UniqueId;
                EnsureItemIsVisibleInNavigation(infoDataItem.Title);
                Navigate(typeof(ItemPage), itemId);
            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                Navigate(typeof(SearchResultsPage), args.QueryText);
            }
        }

        public void EnsureItemIsVisibleInNavigation(string name)
        {
            bool changedSelection = false;
            foreach (object rawItem in NavigationView.MenuItems)
            {
                // Check if we encountered the separator
                if (!(rawItem is NavigationViewItem))
                {
                    // Skipping this item
                    continue;
                }

                var item = rawItem as NavigationViewItem;

                // Check if we are this category
                if ((string)item.Content == name)
                {
                    NavigationView.SelectedItem = item;
                    changedSelection = true;
                }
                // We are not :/
                else
                {
                    // Maybe one of our items is?
                    if (item.MenuItems.Count != 0)
                    {
                        foreach (NavigationViewItem child in item.MenuItems)
                        {
                            if ((string)child.Content == name)
                            {
                                // We are the item corresponding to the selected one, update selection!

                                // Deal with differences in displaymodes
                                if (NavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
                                {
                                    // In Topmode, the child is not visible, so set parent as selected
                                    // Everything else does not work unfortunately
                                    NavigationView.SelectedItem = item;
                                    item.StartBringIntoView();
                                }
                                else
                                {
                                    // Expand so we animate
                                    item.IsExpanded = true;
                                    // Ensure parent is expanded so we actually show the selection indicator
                                    NavigationView.UpdateLayout();
                                    // Set selected item
                                    NavigationView.SelectedItem = child;
                                    child.StartBringIntoView();
                                }
                                // Set to true to also skip out of outer for loop
                                changedSelection = true;
                                // Break out of child iteration for loop
                                break;
                            }
                        }
                    }
                }
                // We updated selection, break here!
                if (changedSelection)
                {
                    break;
                }
            }
        }

        private void NavigationViewControl_PaneClosing(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_PaneOpening(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness() { Left = (sender.CompactPaneLength * 2), Top = currMargin.Top, Right = currMargin.Right, Bottom = currMargin.Bottom };

            }
            else
            {
                AppTitleBar.Margin = new Thickness() { Left = sender.CompactPaneLength, Top = currMargin.Top, Right = currMargin.Right, Bottom = currMargin.Bottom };
            }

            UpdateAppTitleMargin(sender);
            UpdateHeaderMargin(sender);
        }

        private void UpdateAppTitleMargin(Microsoft.UI.Xaml.Controls.NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                AppTitle.TranslationTransition = new Vector3Transition();

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Translation = new System.Numerics.Vector3(smallLeftIndent, 0, 0);
                }
                else
                {
                    AppTitle.Translation = new System.Numerics.Vector3(largeLeftIndent, 0, 0);
                }
            }
            else
            {
                Thickness currMargin = AppTitle.Margin;

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Margin = new Thickness() { Left = smallLeftIndent, Top = currMargin.Top, Right = currMargin.Right, Bottom = currMargin.Bottom };
                }
                else
                {
                    AppTitle.Margin = new Thickness() { Left = largeLeftIndent, Top = currMargin.Top, Right = currMargin.Right, Bottom = currMargin.Bottom };
                }
            }
        }

        private void UpdateHeaderMargin(Microsoft.UI.Xaml.Controls.NavigationView sender)
        {
            if (PageHeader != null)
            {
                if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    PageHeader.HeaderPadding = (Thickness)App.Current.Resources["PageHeaderMinimalPadding"];
                }
                else
                {
                    PageHeader.HeaderPadding = (Thickness)App.Current.Resources["PageHeaderDefaultPadding"];
                }
            }
        }

        private void CtrlF_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            controlsSearchBox.Focus(FocusState.Programmatic);
        }

        #region Helpers for test automation

        private static string _error = string.Empty;
        private static string _log = string.Empty;

        private async void WaitForIdleInvokerButton_Click(object sender, RoutedEventArgs e)
        {
            _idleStateEnteredCheckBox.IsChecked = false;
            await Windows.System.Threading.ThreadPool.RunAsync(WaitForIdleWorker);

            _logReportingTextBox.Text = _log;

            if (_error.Length == 0)
            {
                _idleStateEnteredCheckBox.IsChecked = true;
            }
            else
            {
                // Setting Text will raise a property-changed event, so even if we
                // immediately set it back to the empty string, we'll still get the
                // error-reported event that we can detect and handle.
                _errorReportingTextBox.Text = _error;
                _errorReportingTextBox.Text = string.Empty;

                _error = string.Empty;
            }
        }

        private static void WaitForIdleWorker(IAsyncAction action)
        {
            _error = IdleSynchronizer.TryWait(out _log);
        }

        private void CloseAppInvokerButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void GoBackInvokerButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(this.rootFrame.CanGoBack)
            {
                this.rootFrame.GoBack();
            }
        }

        private void WaitForDebuggerInvokerButton_Click(object sender, RoutedEventArgs e)
        {
            DebuggerAttachedCheckBox.IsChecked = false;

            var dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            var workItem = new Windows.System.Threading.WorkItemHandler((IAsyncAction _) =>
            {
                while (!IsDebuggerPresent())
                {
                    Thread.Sleep(1000);
                }

                DebugBreak();

                dispatcherQueue.TryEnqueue(
                    DispatcherQueuePriority.Low,
                    new DispatcherQueueHandler(() =>
                {
                    DebuggerAttachedCheckBox.IsChecked = true;
                }));
            });

            var asyncAction = Windows.System.Threading.ThreadPool.RunAsync(workItem);
        }

        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        private static extern void DebugBreak();

        #endregion
    }

    public class NavigationRootPageArgs
    {
        public NavigationRootPage NavigationRootPage;
        public object Parameter;
    }

    public enum DeviceType
    {
        Desktop,
        Mobile,
        Other,
        Xbox
    }
}
