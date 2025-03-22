//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUIGallery.Helpers;

namespace WinUIGallery.Pages;

/// <summary>
/// A page that displays a grouped collection of items.
/// </summary>
public sealed partial class AllControlsPage : ItemsPageBase
{
    public AllControlsPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is NavigationRootPageArgs args && args.NavigationRootPage.NavigationView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item => item.Name == "AllControlsItem") is NavigationViewItem item)
        {
            item.IsSelected = true;
        }

        Items = ControlInfoDataSource.Instance.Groups.Where(g => !g.IsSpecialSection).SelectMany(g => g.Items).OrderBy(i => i.Title).ToList();
    }
}
