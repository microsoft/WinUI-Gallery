// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using WinUIGallery.Helpers;
using WinUIGallery.Models;
using WinUIGallery.Pages;

namespace WinUIGallery;

public sealed partial class MainWindow : Window
{
    public Windows.System.VirtualKey ArrowKey;
    public Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
    private UISettings _settings;

    public NavigationView NavigationView
    {
        get { return NavigationViewControl; }
    }

    public Action NavigationViewLoaded { get; set; }

    public MainWindow()
    {
        this.InitializeComponent();
        SetWindowProperties();
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
    }

    private void RootGrid_Loaded(object sender, RoutedEventArgs e)
    {
        NavigationOrientationHelper.UpdateNavigationViewForElement(NavigationOrientationHelper.IsLeftMode());
        _settings = new UISettings();
        _settings.ColorValuesChanged += _settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event because the triggerTitleBarRepaint workaround no longer works
    }

    private void SetWindowProperties()
    {
#if DEBUG
        this.Title = "WinUI 3 Gallery Dev";
        titleBar.Subtitle = "Dev";
#else
        this.Title = "WinUI 3 Gallery";
#endif
        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(titleBar);
        this.AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        Win32WindowHelper win32WindowHelper = new Win32WindowHelper(this);
        win32WindowHelper.SetWindowMinMaxSize(new Win32WindowHelper.POINT() { x = 640, y = 500 });
    }

    private void OnPaneDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
        {
            titleBar.IsPaneToggleButtonVisible = false;
        }
        else
        {
            titleBar.IsPaneToggleButtonVisible = true;
        }
    }

    /// <summary>
    /// Gets the frame of the StartupWindow.
    /// </summary>
    /// <returns>The frame of the StartupWindow.</returns>
    /// <exception cref="Exception">Thrown if the window doesn't have a frame with the name "rootFrame".</exception>
    public Frame GetRootFrame()
    {
        SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
        rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
        rootFrame.NavigationFailed += OnNavigationFailed;
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
    // this handles updating the caption button colors correctly when windows system theme is changed
    // while the app is open
    private void _settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            _ = TitleBarHelper.ApplySystemThemeToCaptionButtons(this, rootFrame.ActualTheme);
        });
    }

    // Wraps a call to rootFrame.Navigate to give the Page a way to know which NavigationRootPage is navigating.
    // Please call this function rather than rootFrame.Navigate to navigate the rootFrame.
    public void Navigate(Type pageType, object targetPageArguments = null, NavigationTransitionInfo navigationTransitionInfo = null)
    {
        rootFrame.Navigate(pageType, targetPageArguments, navigationTransitionInfo);

        // Ensure the NavigationView selection is set to the correct item to mark the sample's page as visited
        if (pageType.Equals(typeof(ItemPage)) && targetPageArguments != null)
        {
            // Mark the item sample's page visited
            SettingsHelper.TryAddItem(SettingsKeys.RecentlyVisited, targetPageArguments.ToString(), InsertPosition.First, SettingsHelper.MaxRecentlyVisitedSamples);
        }
    }

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
                        else if (item.MenuItems.Count > 0)
                        {
                            foreach (var rawInnerItem in item.MenuItems)
                            {
                                if (rawInnerItem is NavigationViewItem innerItem)
                                {
                                    if ((string)innerItem.Tag == id)
                                    {
                                        group.IsExpanded = true;
                                        item.IsExpanded = true;
                                        NavigationView.SelectedItem = innerItem;
                                        innerItem.IsSelected = true;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void AddNavigationMenuItems()
    {
        foreach (var group in ControlInfoDataSource.Instance.Groups.OrderBy(i => i.Title).Where(i => !i.IsSpecialSection))
        {
            var itemGroup = new NavigationViewItem() { Content = group.Title, Tag = group.UniqueId, DataContext = group, Icon = GetIcon(group.IconGlyph) };

            var groupMenuFlyoutItem = new MenuFlyoutItem() { Text = $"Copy Link to {group.Title} samples", Icon = new FontIcon() { Glyph = "\uE8C8" }, Tag = group };
            groupMenuFlyoutItem.Click += this.OnMenuFlyoutItemClick;
            itemGroup.ContextFlyout = new MenuFlyout() { Items = { groupMenuFlyoutItem } };

            AutomationProperties.SetName(itemGroup, group.Title);
            AutomationProperties.SetAutomationId(itemGroup, group.UniqueId);

            foreach (var item in group.Items)
            {
                var itemInGroup = new NavigationViewItem() { IsEnabled = item.IncludedInBuild, Content = item.Title, Tag = item.UniqueId, DataContext = item };

                var itemInGroupMenuFlyoutItem = new MenuFlyoutItem() { Text = $"Copy Link to {item.Title} sample", Icon = new FontIcon() { Glyph = "\uE8C8" }, Tag = item };
                itemInGroupMenuFlyoutItem.Click += this.OnMenuFlyoutItemClick;
                itemInGroup.ContextFlyout = new MenuFlyout() { Items = { itemInGroupMenuFlyoutItem } };

                itemGroup.MenuItems.Add(itemInGroup);
                AutomationProperties.SetName(itemInGroup, item.Title);
                AutomationProperties.SetAutomationId(itemInGroup, item.UniqueId);
            }

            NavigationViewControl.MenuItems.Add(itemGroup);
        }

        Home.Loaded += OnHomeMenuItemLoaded;
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
                    (IconElement)new BitmapIcon() { UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute), ShowAsMonochrome = false } :
                    (IconElement)new FontIcon()
                    {
                        Glyph = imagePath
                    };
    }

    private void OnHomeMenuItemLoaded(object sender, RoutedEventArgs e)
    {
        if (NavigationViewControl.DisplayMode == NavigationViewDisplayMode.Expanded)
        {
            controlsSearchBox.Focus(FocusState.Keyboard);
        }
    }

    private void OnNavigationViewControlLoaded(object sender, RoutedEventArgs e)
    {
        // Delay necessary to ensure NavigationView visual state can match navigation
        Task.Delay(500).ContinueWith(_ => this.NavigationViewLoaded?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());

        var navigationView = sender as NavigationView;
        navigationView.RegisterPropertyChangedCallback(NavigationView.IsPaneOpenProperty, OnIsPaneOpenChanged);
    }

    private void OnIsPaneOpenChanged(DependencyObject sender, DependencyProperty dp)
    {
        var navigationView = sender as NavigationView;
        var announcementText = navigationView.IsPaneOpen ? "Navigation Pane Opened" : "Navigation Pane Closed";

        UIHelper.AnnounceActionForAccessibility(navigationView, announcementText, "NavigationViewPaneIsOpenChangeNotificationId");
    }

    private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
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
            if (selectedItem == AllControlsItem)
            {
                if (rootFrame.CurrentSourcePageType != typeof(AllControlsPage))
                {
                    Navigate(typeof(AllControlsPage));
                }
            }
            else if (selectedItem == Home)
            {
                if (rootFrame.CurrentSourcePageType != typeof(HomePage))
                {
                    Navigate(typeof(HomePage));
                }
            }
            else if (selectedItem == ColorItem)
            {
                Navigate(typeof(ItemPage), "Color");
            }
            else if (selectedItem == GeometryItem)
            {
                Navigate(typeof(ItemPage), "Geometry");
            }
            else if (selectedItem == IconographyItem)
            {
                Navigate(typeof(ItemPage), "Iconography");
            }
            else if (selectedItem == SpacingItem)
            {
                Navigate(typeof(ItemPage), "Spacing");
            }
            else if (selectedItem == TypographyItem)
            {
                Navigate(typeof(ItemPage), "Typography");
            }
            else if (selectedItem == AccessibilityScreenReaderPage)
            {
                Navigate(typeof(ItemPage), "AccessibilityScreenReader");
            }
            else if (selectedItem == AccessibilityKeyboardPage)
            {
                Navigate(typeof(ItemPage), "AccessibilityKeyboard");
            }
            else if (selectedItem == AccessibilityContrastPage)
            {
                Navigate(typeof(ItemPage), "AccessibilityColorContrast");
            }
            else if (selectedItem == XamlResourcesPage)
            {
                Navigate(typeof(ItemPage), "XamlResources");
            }
            else if (selectedItem == XamlStylesPage)
            {
                Navigate(typeof(ItemPage), "XamlStyles");
            }
            else if (selectedItem == BindingPage)
            {
                Navigate(typeof(ItemPage), "Binding");
            }
            else if (selectedItem == TemplatesPage)
            {
                Navigate(typeof(ItemPage), "Templates");
            }
            else if (selectedItem == CustomUserControlsPage)
            {
                Navigate(typeof(ItemPage), "CustomUserControls");
            }
            else if (selectedItem == ScratchPadPage)
            {
                Navigate(typeof(ItemPage), "ScratchPad");
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
        TestContentLoadedCheckBox.IsChecked = true;
    }

    private void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
    {
        TestContentLoadedCheckBox.IsChecked = false;
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
                controlsSearchBox.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title).ToList();
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
            var hasChangedSelection = EnsureItemIsVisibleInNavigation(infoDataItem.Title);

            // In case the menu selection has changed, it means that it has triggered
            // the selection changed event, that will navigate to the page already
            if (!hasChangedSelection)
            {
                Navigate(typeof(ItemPage), infoDataItem.UniqueId);
            }
        }
        else if (!string.IsNullOrEmpty(args.QueryText))
        {
            Navigate(typeof(SearchResultsPage), args.QueryText);
        }
    }

    public bool EnsureItemIsVisibleInNavigation(string name)
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
        return changedSelection;
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
        if (this.rootFrame.CanGoBack)
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
            while (!Windows.Win32.PInvoke.IsDebuggerPresent())
            {
                Thread.Sleep(1000);
            }

            Windows.Win32.PInvoke.DebugBreak();

            dispatcherQueue.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
                new Microsoft.UI.Dispatching.DispatcherQueueHandler(() =>
                {
                    DebuggerAttachedCheckBox.IsChecked = true;
                }));
        });

        var asyncAction = Windows.System.Threading.ThreadPool.RunAsync(workItem);
    }

    #endregion

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        if (this.rootFrame.CanGoBack)
        {
            this.rootFrame.GoBack();
        }
    }
}