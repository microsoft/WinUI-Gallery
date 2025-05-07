using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using WinUIGallery.Helpers;
using System;

namespace WinUIGallery.SamplePages;

public class TabItemData : DependencyObject
{
    public SymbolIconSource IconSource { get; set; } = new SymbolIconSource()
    {
        Symbol = Symbol.Placeholder
    };
    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public string Content
    {
        get { return (string)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public bool IsInProgress
    {
        get { return (bool)GetValue(IsInProgressProperty); }
        set { SetValue(IsInProgressProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TabItemData), new PropertyMetadata(""));
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(string), typeof(TabItemData), new PropertyMetadata(""));
    public static readonly DependencyProperty IsInProgressProperty = DependencyProperty.Register("Content", typeof(bool), typeof(TabItemData), new PropertyMetadata(false));
}

public sealed partial class TabViewWindowingSamplePage : Page
{
    private const string DataIdentifier = "MyTabItem";
    private static readonly List<Window> windowList = [];
    private static Window tabTearOutWindow = null;

    private Win32WindowHelper win32WindowHelper;

    private readonly ObservableCollection<TabItemData> tabItemDataList = [];
    public ObservableCollection<TabItemData> TabItemDataList => tabItemDataList;

    public TabViewWindowingSamplePage()
    {
        this.InitializeComponent();

        Loaded += TabViewWindowingSamplePage_Loaded;
    }

    public void SetupWindowMinSize(Window window)
    {
        win32WindowHelper = new Win32WindowHelper(window);
        win32WindowHelper.SetWindowMinMaxSize(new Win32WindowHelper.POINT() { x = 500, y = 300 });
    }

    private void TabViewWindowingSamplePage_Loaded(object sender, RoutedEventArgs e)
    {
        var currentWindow = WindowHelper.GetWindowForElement(this);
        currentWindow.ExtendsContentIntoTitleBar = true;
        currentWindow.SetTitleBar(CustomDragRegion);
        CustomDragRegion.MinWidth = 188;

        if (!windowList.Contains(currentWindow))
        {
            windowList.Add(currentWindow);

            // We can have a window we're dragging in two different ways: either we created a new window
            // for tearing out purposes, or we're dragging an existing window.
            // If we created a new window, tabTearOutWindow will be set to that window.
            // Otherwise, it won't be set to anything, so we should set it to the window we're currently dragging.
            var inputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(currentWindow.AppWindow.Id);

            double scaleAdjustment = currentWindow.Content.XamlRoot.RasterizationScale;
            double titleContentWidth = CustomDragRegion.Width;
            double titleContentHeight = CustomDragRegion.Height;
            Windows.Graphics.RectInt32 titleBarRect = new Windows.Graphics.RectInt32(0, 0, (int)Math.Round(titleContentWidth * scaleAdjustment), (int)Math.Round(titleContentHeight * scaleAdjustment));
            inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Passthrough, new Windows.Graphics.RectInt32[] { titleBarRect });
            inputNonClientPointerSource.EnteredMoveSize += (s, args) =>
            {
                if (tabTearOutWindow == null)
                {
                    tabTearOutWindow = currentWindow;
                }
            };

            inputNonClientPointerSource.ExitedMoveSize += (s, args) =>
            {
                tabTearOutWindow = null;
            };

            currentWindow.Closed += (s, args) =>
            {
                windowList.Remove(currentWindow);
            };
        }
    }

    public void LoadDemoData()
    {
        // Main Window -- add some default items
        for (int i = 0; i < 3; i++)
        {
            TabItemDataList.Add(new TabItemData() { IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder }, Header = $"Item {i}", Content = $"Page {i}" });
        }

        Tabs.SelectedIndex = 0;
    }

    private void Tabs_TabTearOutWindowRequested(TabView sender, TabViewTabTearOutWindowRequestedEventArgs args)
    {
        tabTearOutWindow = CreateNewWindow();
        args.NewWindowId = tabTearOutWindow.AppWindow.Id;
    }

    private void Tabs_TabTearOutRequested(TabView sender, TabViewTabTearOutRequestedEventArgs args)
    {
        if (tabTearOutWindow == null)
        {
            return;
        }
        if (tabItemDataList.Count > 1)
        {
            MoveDataItems(TabItemDataList, GetTabItemDataList(tabTearOutWindow), args.Items, 0);
        }
    }

    private void Tabs_ExternalTornOutTabsDropping(TabView sender, TabViewExternalTornOutTabsDroppingEventArgs args)
    {
        args.AllowDrop = true;
    }

    private void Tabs_ExternalTornOutTabsDropped(TabView sender, TabViewExternalTornOutTabsDroppedEventArgs args)
    {
        MoveDataItems(GetTabItemDataList(tabTearOutWindow), TabItemDataList, args.Items, args.DropIndex);
    }

    private static Window CreateNewWindow()
    {
        var newPage = new TabViewWindowingSamplePage();
        var window = WindowHelper.CreateWindow();
        window.SystemBackdrop = new MicaBackdrop();
        window.ExtendsContentIntoTitleBar = true;
        window.Content = newPage;
        window.AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        newPage.SetupWindowMinSize(window);
        return window;
    }

    private static void MoveDataItems(ObservableCollection<TabItemData> source, ObservableCollection<TabItemData> destination, object[] dataItems, int index)
    {
        foreach (object tabItemData in dataItems)
        {
            source.Remove((TabItemData)tabItemData);
            destination.Insert(index, (TabItemData)tabItemData);

            index++;
        }
    }

    private static TabView GetTabView(Window window)
    {
        var tabViewPage = (TabViewWindowingSamplePage)window.Content;
        return tabViewPage.Tabs;
    }

    private static ObservableCollection<TabItemData> GetTabItemDataList(Window window)
    {
        var tabViewPage = (TabViewWindowingSamplePage)window.Content;
        return tabViewPage.TabItemDataList;
    }

    private void Tabs_AddTabButtonClick(TabView sender, object args)
    {
        TabItemDataList.Add(new TabItemData() { Header = "New Item", Content = "New Item" });
    }

    private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        TabItemDataList.Remove((TabItemData)args.Item);

        if (TabItemDataList.Count == 0)
        {
            WindowHelper.GetWindowForElement(this).Close();
        }
    }

    private void TabViewContextMenu_Opening(object sender, object e)
    {
        // The contents of the context menu depends on the state of the application, so we'll build it dynamically.
        MenuFlyout contextMenu = (MenuFlyout)sender;

        // We'll first put the generic tab view context menu items in place.
        TabViewHelper.PopulateTabViewContextMenu(contextMenu);

        var tabViewItem = (TabViewItem)contextMenu.Target;
        ListView tabViewListView = UIHelper.GetParent<ListView>(tabViewItem);
        var window = WindowHelper.GetWindowForElement(tabViewItem);

        if (tabViewListView == null)
        {
            return;
        }

        var tabItemDataList = GetTabItemDataList(window);
        var tabDataItem = tabViewListView.ItemFromContainer(tabViewItem);

        // Second, we'll include menu items to move this tab to those windows.
        MenuFlyoutSubItem moveSubItem = new() { Text = "Move tab to" };

        // If there are at least two tabs in this window, we'll include the option to move the tab to a new window.
        // This option doesn't make sense if there is only one tab, because in that case the source window would have no tabs left,
        // and we would effectively be just moving the tab from one window with only one tab to another window with only one tab,
        // leaving us in the same state as we started in.
        if (tabItemDataList.Count > 1)
        {
            MenuFlyoutItem newWindowItem = new() { Text = "New window", Icon = new SymbolIcon(Symbol.NewWindow) };

            newWindowItem.Click += (s, args) =>
            {
                var newWindow = CreateNewWindow();
                MoveDataItems(tabItemDataList, GetTabItemDataList(newWindow), [tabDataItem], 0);


                // Activating the window and setting its selected item hit a failed assert if the content hasn't been loaded yet,
                // so we'll defer these for a tick to allow that to happen first.
                DispatcherQueue.TryEnqueue(() =>
                {
                    newWindow.Activate();
                    GetTabView(newWindow).SelectedItem = tabDataItem;
                });
            };

            moveSubItem.Items.Add(newWindowItem);
        }

        // If there are other windows that exist, we'll include the option to move the tab to those windows.
        List<MenuFlyoutItem> moveToWindowItems = [];

        foreach (Window otherWindow in windowList)
        {
            if (window == otherWindow)
            {
                continue;
            }

            var windowTabItemDataList = GetTabItemDataList(otherWindow);

            if (windowTabItemDataList.Count > 0)
            {
                string moveToWindowItemText = $"Window with \"{windowTabItemDataList[0].Header}\"";

                if (windowTabItemDataList.Count > 1)
                {
                    int remainingTabCount = windowTabItemDataList.Count - 1;
                    moveToWindowItemText += $" and {remainingTabCount} other tab{(remainingTabCount == 1 ? "" : "s")}";
                }

                MenuFlyoutItem moveToWindowItem = new() { Text = moveToWindowItemText, Icon = new SymbolIcon(Symbol.BackToWindow) };
                moveToWindowItem.Click += (s, args) =>
                {
                    MoveDataItems(tabItemDataList, windowTabItemDataList, [tabDataItem], windowTabItemDataList.Count);

                    // If removing the tab from its current tab view will leave no tabs remaining, then we'll close the tab view's window.
                    if (tabItemDataList.Count == 0)
                    {
                        window.Close();
                    }

                    // Activating the window and setting its selected item hit a failed assert if the content hasn't been loaded yet,
                    // so we'll defer these for a tick to allow that to happen first.
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        otherWindow.Activate();
                        GetTabView(otherWindow).SelectedItem = tabDataItem;
                    });
                };
                moveToWindowItems.Add(moveToWindowItem);
            }
        }

        // Only include a separator if we're going to be including at least one move-to-window item.
        if (moveToWindowItems.Count > 0)
        {
            contextMenu.Items.Add(new MenuFlyoutSeparator());
        }

        foreach (MenuFlyoutItem moveToWindowItem in moveToWindowItems)
        {
            moveSubItem.Items.Add(moveToWindowItem);
        }

        // Only include the move-to sub-item if it has any items.
        if (moveSubItem.Items.Count > 0)
        {
            contextMenu.Items.Add(moveSubItem);
        }

        // If the context menu ended up with no items at all, then we'll prevent it from being shown.
        if (contextMenu.Items.Count == 0)
        {
            contextMenu.Hide();
        }
    }
}
