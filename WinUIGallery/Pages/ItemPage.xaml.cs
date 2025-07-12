// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Reflection;
using WinUIGallery.Controls;
using WinUIGallery.Helpers;
using WinUIGallery.Models;

namespace WinUIGallery.Pages;

/// <summary>
/// A page that displays details for a single item within a group.
/// </summary>
public sealed partial class ItemPage : Page
{
    private static string WinUIBaseUrl = "https://github.com/microsoft/microsoft-ui-xaml/tree/main/src/controls/dev";
    private static string GalleryBaseUrl = "https://github.com/microsoft/WinUI-Gallery/tree/main/WinUIGallery/Samples/ControlPages/";

    public ControlInfoDataItem Item
    {
        get { return _item; }
        set { _item = value; }
    }

    private ControlInfoDataItem _item;
    private ElementTheme? _currentElementTheme;

    public ItemPage()
    {
        this.InitializeComponent();
        Loaded += (s, e) => SetInitialVisuals();
    }

    public void SetInitialVisuals()
    {
        pageHeader.ToggleThemeAction = OnToggleTheme;
        App.MainWindow.NavigationViewLoaded = OnNavigationViewLoaded;
        this.Focus(FocusState.Programmatic);
    }

    private void OnNavigationViewLoaded()
    {
        App.MainWindow.EnsureNavigationSelection(this.Item.UniqueId);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is string uniqueId)
        {
            var group = await ControlInfoDataSource.GetGroupFromItemAsync(uniqueId);
            var item = group?.Items.FirstOrDefault(x => x.UniqueId.Equals(uniqueId));

            if (item != null)
            {
                Item = item;

                // Load control page into frame.
                Type pageType = Type.GetType("WinUIGallery.ControlPages." + item.UniqueId + "Page");

                if (pageType != null)
                {
                    var pageName = string.IsNullOrEmpty(group.Folder) ? pageType.Name : $"{group.Folder}/{pageType.Name}";
                    pageHeader.SetControlSourceLink(WinUIBaseUrl, item.SourcePath);
                    pageHeader.SetSamplePageSourceLinks(GalleryBaseUrl, pageName);
                    System.Diagnostics.Debug.WriteLine(string.Format("[ItemPage] Navigate to {0}", pageType.ToString()));
                    this.contentFrame.Navigate(pageType);
                }
                App.MainWindow.EnsureNavigationSelection(item?.UniqueId);

                if (contentFrame.Content is Page loadedPage && PageScrollBehaviorHelper.GetSuppressHostScrolling(loadedPage))
                {
                    // Disabling page scrolling, as the page itself will have ScrollViewers to handle specific scrolling use cases
                    svPanel.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
        }
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        SetControlExamplesTheme(ThemeHelper.ActualTheme);
        base.OnNavigatingFrom(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        App.MainWindow.NavigationViewLoaded = null;
        pageHeader.ToggleThemeAction = null;
        pageHeader.CopyLinkAction = null;

        // We use reflection to call the OnNavigatedFrom function the user leaves this page
        // See this PR for more information: https://github.com/microsoft/WinUI-Gallery/pull/145
        Frame contentFrameAsFrame = contentFrame as Frame;
        Page innerPage = contentFrameAsFrame.Content as Page;
        if (innerPage != null)
        {
            MethodInfo dynMethod = innerPage.GetType().GetMethod("OnNavigatedFrom",
                BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod.Invoke(innerPage, new object[] { e });
        }

        base.OnNavigatedFrom(e);
    }

    public static ItemPage GetForElement(object obj)
    {
        UIElement element = (UIElement)obj;
        Window window = WindowHelper.GetWindowForElement(element);
        if (window != null)
        {
            return (ItemPage)window.Content;
        }
        return null;
    }

    private void OnToggleTheme()
    {
        var currentElementTheme = ((_currentElementTheme ?? ElementTheme.Default) == ElementTheme.Default) ? ThemeHelper.ActualTheme : _currentElementTheme.Value;
        var newTheme = currentElementTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
        SetControlExamplesTheme(newTheme);
    }

    private void SetControlExamplesTheme(ElementTheme theme)
    {
        var controlExamples = (this.contentFrame.Content as UIElement)?.GetDescendantsOfType<SampleThemeListener>();

        if (controlExamples != null)
        {
            _currentElementTheme = theme;
            foreach (var controlExample in controlExamples)
            {
                controlExample.RequestedTheme = theme;
            }
            if (controlExamples.Count() == 0)
            {
                this.RequestedTheme = theme;
            }
        }
    }
}