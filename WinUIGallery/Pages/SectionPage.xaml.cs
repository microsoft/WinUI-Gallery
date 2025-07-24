// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
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

        if (e.Parameter is string groupID)
        {
            var group = await ControlInfoDataSource.GetGroupAsync(groupID);

            ((NavigationViewItemBase)App.MainWindow.NavigationView.MenuItems.Single(i => (string)((NavigationViewItemBase)i).Tag == group.UniqueId)).IsSelected = true;
            TitleTxt.Text = group.Title;
            Items = group.Items.OrderBy(i => i.Title).ToList();
        }
    }

    protected override bool GetIsNarrowLayoutState()
    {
        return LayoutVisualStates.CurrentState == NarrowLayout;
    }
}
