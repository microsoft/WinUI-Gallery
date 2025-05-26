//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
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
