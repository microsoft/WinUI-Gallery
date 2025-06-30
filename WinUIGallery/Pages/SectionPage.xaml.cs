// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using WinUIGallery.Helpers;

namespace WinUIGallery.Pages;

/// <summary>
/// A page that displays an overview of a single group, including a preview of the items
/// within the group.
/// </summary>
public sealed partial class SectionPage : ItemsPageBase
{
    public SectionPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
        NavigationRootPage navigationRootPage = args.NavigationRootPage;
        var group = await ControlInfoDataSource.GetGroupAsync((string)args.Parameter);

        var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItemBase)navigationRootPage.NavigationView.MenuItems.Single(i => (string)((Microsoft.UI.Xaml.Controls.NavigationViewItemBase)i).Tag == group.UniqueId);
        menuItem.IsSelected = true;
        TitleTxt.Text = group.Title;
        Items = group.Items.OrderBy(i => i.Title).ToList();
    }

    protected override bool GetIsNarrowLayoutState()
    {
        return LayoutVisualStates.CurrentState == NarrowLayout;
    }
}
