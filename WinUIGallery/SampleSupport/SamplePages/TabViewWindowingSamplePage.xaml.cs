// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Linq;
using WinUIGallery.Helpers;

namespace WinUIGallery.SamplePages;

public sealed partial class TabViewWindowingSamplePage : Page
{
    private Window? tabTearOutWindow = null;

    public TabViewWindowingSamplePage()
    {
        this.InitializeComponent();

        Loaded += TabViewWindowingSamplePage_Loaded;
    }

    private void TabViewWindowingSamplePage_Loaded(object sender, RoutedEventArgs e)
    {
        if (WindowHelper.GetWindowForElement(this) is not Window currentWindow)
        {
            return;
        }

        currentWindow.ExtendsContentIntoTitleBar = true;
        currentWindow.SetTitleBar(CustomDragRegion);
        CustomDragRegion.MinWidth = 188;

        // Set minimum window size using OverlappedPresenter (requires XamlRoot, so must be done after Loaded).
        WindowHelper.SetWindowMinSize(currentWindow, 500, 300);
    }

    public void LoadDemoData()
    {
        // Main Window -- add some default items
        for (int i = 0; i < 3; i++)
        {
            Tabs.TabItems.Add(new TabViewItem() { IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder }, Header = $"Item {i}", Content = new TabContentSampleControl() { DataContext = $"Page {i}" } });
        }

        Tabs.SelectedIndex = 0;
    }

    public void AddTabToTabs(TabViewItem tab)
    {
        Tabs.TabItems.Add(tab);
    }

    private void Tabs_TabTearOutWindowRequested(TabView sender, TabViewTabTearOutWindowRequestedEventArgs args)
    {
        var newPage = new TabViewWindowingSamplePage();

        tabTearOutWindow = WindowHelper.CreateWindow();
        tabTearOutWindow.ExtendsContentIntoTitleBar = true;
        tabTearOutWindow.Content = newPage;
        tabTearOutWindow.AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");

        args.NewWindowId = tabTearOutWindow.AppWindow.Id;
    }

    private void Tabs_TabTearOutRequested(TabView sender, TabViewTabTearOutRequestedEventArgs args)
    {
        if (tabTearOutWindow == null)
        {
            return;
        }

        var newPage = (TabViewWindowingSamplePage)tabTearOutWindow.Content;

        foreach (TabViewItem tab in args.Tabs.Cast<TabViewItem>())
        {
            GetParentTabView(tab)?.TabItems.Remove(tab);
            newPage.AddTabToTabs(tab);
        }

        // Clear the reference now that the tear-out is complete to avoid stale references
        // if multiple tear-outs happen in sequence.
        tabTearOutWindow = null;

        // Close the source window if all tabs have been torn out.
        CloseWindowIfEmpty(sender);
    }

    private void Tabs_ExternalTornOutTabsDropping(TabView sender, TabViewExternalTornOutTabsDroppingEventArgs args)
    {
        args.AllowDrop = true;
    }

    private void Tabs_ExternalTornOutTabsDropped(TabView sender, TabViewExternalTornOutTabsDroppedEventArgs args)
    {
        int position = 0;

        foreach (TabViewItem tab in args.Tabs.Cast<TabViewItem>())
        {
            // Find the source TabView before removing the tab, so we can check if it's empty afterwards.
            TabView? sourceTabView = GetParentTabView(tab);
            sourceTabView?.TabItems.Remove(tab);
            sender.TabItems.Insert(args.DropIndex + position, tab);
            position++;

            // Close the source window if all its tabs have been moved to this window.
            if (sourceTabView != null && sourceTabView.TabItems.Count == 0)
            {
                CloseWindowIfEmpty(sourceTabView);
            }
        }
    }

    private TabView? GetParentTabView(TabViewItem tab)
    {
        DependencyObject current = tab;

        while (current != null)
        {
            if (current is TabView tabView)
            {
                return tabView;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private void CloseWindowIfEmpty(TabView tabView)
    {
        if (tabView.TabItems.Count == 0)
        {
            // Find the window containing this TabView and close it.
            // Walk up from the TabView to find a Page, then use it to locate the window.
            DependencyObject current = tabView;
            while (current != null)
            {
                if (current is Page page)
                {
                    WindowHelper.GetWindowForElement(page)?.Close();
                    return;
                }

                current = VisualTreeHelper.GetParent(current);
            }
        }
    }

    private TabViewItem CreateNewTVI(string header, string dataContext)
    {
        var newTab = new TabViewItem()
        {
            IconSource = new SymbolIconSource()
            {
                Symbol = Symbol.Placeholder
            },
            Header = header,
            Content = new TabContentSampleControl()
            {
                DataContext = dataContext
            }
        };

        return newTab;
    }

    private void Tabs_AddTabButtonClick(TabView sender, object args)
    {
        var tab = CreateNewTVI("New Item", "New Item");
        sender.TabItems.Add(tab);
    }

    private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
        CloseWindowIfEmpty(sender);
    }
}
