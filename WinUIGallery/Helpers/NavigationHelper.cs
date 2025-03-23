using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Core;
using Microsoft.UI.Input;

namespace WinUIGallery.Helpers;

/// <summary>
/// NavigationHelper aids in navigation between pages. It manages
/// the backstack and integrates SuspensionManager to handle process
/// lifetime management and state management when navigating between pages.
/// </summary>
/// <example>
/// To make use of NavigationHelper, follow these two steps or
/// start with a BasicPage or any other Page item template other than BlankPage.
///
/// 1) Create an instance of the NavigationHelper somewhere such as in the
///     constructor for the page and register a callback for the LoadState and
///     SaveState events.
/// <code>
///     public MyPage()
///     {
///         this.InitializeComponent();
///         this.navigationHelper = new NavigationHelper(this);
///         this.navigationHelper.LoadState += navigationHelper_LoadState;
///         this.navigationHelper.SaveState += navigationHelper_SaveState;
///     }
///
///     private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
///     { }
///     private void navigationHelper_SaveState(object sender, LoadStateEventArgs e)
///     { }
/// </code>
///
/// 2) Register the page to call into the NavigationManager whenever the page participates
///     in navigation by overriding the <see cref="Page.OnNavigatedTo"/>
///     and <see cref="Page.OnNavigatedFrom"/> events.
/// <code>
///     protected override void OnNavigatedTo(NavigationEventArgs e)
///     {
///         navigationHelper.OnNavigatedTo(e);
///     }
///
///     protected override void OnNavigatedFrom(NavigationEventArgs e)
///     {
///         navigationHelper.OnNavigatedFrom(e);
///     }
/// </code>
/// </example>
[WebHostHidden]
public class NavigationHelper : DependencyObject
{
    private Page Page { get; set; }
    private Frame Frame => Page.Frame;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationHelper"/> class.
    /// </summary>
    /// <param name="page">A reference to the current page used for navigation.
    /// This reference allows for frame manipulation.</param>
    public NavigationHelper(Page page)
    {
        Page = page;
    }

    #region Process lifetime management

    private string _pageKey;

    /// <summary>
    /// Handle this event to populate the page using content passed
    /// during navigation as well as any state that was saved by
    /// the SaveState event handler.
    /// </summary>
    public event LoadStateEventHandler LoadState;
    /// <summary>
    /// Handle this event to save state that can be used by
    /// the LoadState event handler. Save the state in case
    /// the application is suspended or the page is discarded
    /// from the navigation cache.
    /// </summary>
    public event SaveStateEventHandler SaveState;

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// This method calls <see cref="LoadState"/>, where all page specific
    /// navigation and process lifetime management logic should be placed.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.  The Parameter
    /// property provides the group to be displayed.</param>
    public void OnNavigatedTo(NavigationEventArgs e)
    {
        var frameState = SuspensionManager.SessionStateForFrame(Frame);
        _pageKey = "Page-" + Frame.BackStackDepth;

        if (e.NavigationMode == NavigationMode.New)
        {
            // Clear existing state for forward navigation when adding a new page to the
            // navigation stack
            var nextPageKey = _pageKey;
            int nextPageIndex = Frame.BackStackDepth;
            while (frameState.Remove(nextPageKey))
            {
                nextPageIndex++;
                nextPageKey = "Page-" + nextPageIndex;
            }

            // Pass the navigation parameter to the new page
            LoadState?.Invoke(this, new LoadStateEventArgs(e.Parameter, null));
        }
        else
        {
            // Pass the navigation parameter and preserved page state to the page, using
            // the same strategy for loading suspended state and recreating pages discarded
            // from cache
            LoadState?.Invoke(this, new LoadStateEventArgs(e.Parameter, (Dictionary<string, object>)frameState[_pageKey]));
        }
    }

    /// <summary>
    /// Invoked when this page will no longer be displayed in a Frame.
    /// This method calls <see cref="SaveState"/>, where all page specific
    /// navigation and process lifetime management logic should be placed.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.  The Parameter
    /// property provides the group to be displayed.</param>
    public void OnNavigatedFrom(NavigationEventArgs e)
    {
        var frameState = SuspensionManager.SessionStateForFrame(Frame);
        var pageState = new Dictionary<string, object>();
        SaveState?.Invoke(this, new SaveStateEventArgs(pageState));
        frameState[_pageKey] = pageState;
    }

    #endregion
}

/// <summary>
/// RootFrameNavigationHelper registers for standard mouse and keyboard
/// shortcuts used to go back and forward. There should be only one
/// RootFrameNavigationHelper per view, and it should be associated with the
/// root frame.
/// </summary>
/// <example>
/// To make use of RootFrameNavigationHelper, create an instance of the
/// RootNavigationHelper such as in the constructor of your root page.
/// <code>
///     public MyRootPage()
///     {
///         this.InitializeComponent();
///         this.rootNavigationHelper = new RootNavigationHelper(MyFrame);
///     }
/// </code>
/// </example>
[WebHostHidden]
public class RootFrameNavigationHelper
{
    private Frame Frame { get; set; }
    private NavigationView CurrentNavView { get; set; }

#nullable enable
    private static RootFrameNavigationHelper? instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootNavigationHelper"/> class.
    /// </summary>
    /// <param name="rootFrame">A reference to the top-level frame.
    /// This reference allows for frame manipulation and to register navigation handlers.</param>
    public RootFrameNavigationHelper(Frame rootFrame, NavigationView currentNavView)
    {
        if (instance != null)
        {
            return;
        }

        Frame = rootFrame;
        Frame.Navigated += (s, e) =>
        {
            // Update the Back button whenever a navigation occurs.
            UpdateBackButton();
        };
        CurrentNavView = currentNavView;

        CurrentNavView.BackRequested += NavView_BackRequested;
        CurrentNavView.PointerPressed += CurrentNavView_PointerPressed;
        instance = this;
    }

    /// <summary>
    /// Invoked on every keystroke, including system keys such as Alt key combinations.
    /// Used to detect keyboard navigation between pages even when the page itself
    /// doesn't have focus.
    /// </summary>
    /// <param name="sender">Instance that triggered the event.</param>
    /// <param name="e">Event data describing the conditions that led to the event.</param>
    public static void RaiseKeyPressed(uint keyCode)
    {
        if (instance == null) return;

        // Only investigate further when Left, Right, or the dedicated
        // Previous or Next keys are pressed.
        if (keyCode == (int)VirtualKey.Left ||
            keyCode == (int)VirtualKey.Right ||
            keyCode == 166 ||
            keyCode == 167 ||
            keyCode == (int)VirtualKey.Back)
        {
            var downState = CoreVirtualKeyStates.Down;
            // VirtualKeys 'Menu' key is also the 'Alt' key on the keyboard.
            bool isMenuKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu) & downState) == downState;
            bool isControlKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control) & downState) == downState;
            bool isShiftKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift) & downState) == downState;
            bool isWindowsKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.LeftWindows) & downState) == downState;
            bool isModifierKeyPressed = !isMenuKeyPressed && !isControlKeyPressed && !isShiftKeyPressed;
            bool isOnlyAltPressed = isMenuKeyPressed && !isControlKeyPressed && !isShiftKeyPressed;

            if (((int)keyCode == 166 && isModifierKeyPressed) ||
                (keyCode == (int)VirtualKey.Left && isOnlyAltPressed) ||
                (keyCode == (int)VirtualKey.Back && isWindowsKeyPressed))
            {
                // When the previous key or Alt+Left are pressed navigate back.
                instance.TryGoBack();
            }
            else if (((int)keyCode == 167 && isModifierKeyPressed) ||
                (keyCode == (int)VirtualKey.Right && isOnlyAltPressed))
            {
                // When the next key or Alt+Right are pressed navigate forward.
                instance.TryGoForward();
            }
        }
    }

    private void CurrentNavView_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint(CurrentNavView).Properties;

        // Ignore button chords with the left, right, and middle buttons
        if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
            properties.IsMiddleButtonPressed)
            return;

        // If back or forward are pressed (but not both) navigate appropriately
        bool backPressed = properties.IsXButton1Pressed;
        bool forwardPressed = properties.IsXButton2Pressed;
        if (backPressed ^ forwardPressed)
        {
            e.Handled = true;
            if (backPressed) TryGoBack();
            if (forwardPressed) TryGoForward();
        }
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => TryGoBack();

    private bool TryGoBack()
    {
        bool navigated = false;
        // Don't go back if the nav pane is overlayed.
        if (CurrentNavView.IsPaneOpen && (CurrentNavView.DisplayMode == NavigationViewDisplayMode.Compact || CurrentNavView.DisplayMode == NavigationViewDisplayMode.Minimal))
        {
            return navigated;
        }

        if (Frame.CanGoBack)
        {
            Frame.GoBack();
            navigated = true;
        }

        return navigated;
    }

    private bool TryGoForward()
    {
        bool navigated = false;
        if (Frame.CanGoForward)
        {
            Frame.GoForward();
            navigated = true;
        }
        return navigated;
    }

    private void UpdateBackButton() => CurrentNavView.IsBackEnabled = Frame.CanGoBack;
}

/// <summary>
/// Represents the method that will handle the <see cref="NavigationHelper.LoadState"/>event
/// </summary>
public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
/// <summary>
/// Represents the method that will handle the <see cref="NavigationHelper.SaveState"/>event
/// </summary>
public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

/// <summary>
/// Class used to hold the event data required when a page attempts to load state.
/// </summary>
public class LoadStateEventArgs : EventArgs
{
    /// <summary>
    /// The parameter value passed to <see cref="Frame.Navigate(Type, object)"/>
    /// when this page was initially requested.
    /// </summary>
    public object NavigationParameter { get; private set; }
    /// <summary>
    /// A dictionary of state preserved by this page during an earlier
    /// session.  This will be null the first time a page is visited.
    /// </summary>
    public Dictionary<string, object> PageState { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadStateEventArgs"/> class.
    /// </summary>
    /// <param name="navigationParameter">
    /// The parameter value passed to <see cref="Frame.Navigate(Type, object)"/>
    /// when this page was initially requested.
    /// </param>
    /// <param name="pageState">
    /// A dictionary of state preserved by this page during an earlier
    /// session.  This will be null the first time a page is visited.
    /// </param>
    public LoadStateEventArgs(object navigationParameter, Dictionary<string, object> pageState)
        : base()
    {
        NavigationParameter = navigationParameter;
        PageState = pageState;
    }
}
/// <summary>
/// Class used to hold the event data required when a page attempts to save state.
/// </summary>
public class SaveStateEventArgs : EventArgs
{
    /// <summary>
    /// An empty dictionary to be populated with serializable state.
    /// </summary>
    public Dictionary<string, object> PageState { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
    /// </summary>
    /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
    public SaveStateEventArgs(Dictionary<string, object> pageState)
        : base()
    {
        PageState = pageState;
    }
}
