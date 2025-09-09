// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using WinUIGallery.Helpers;

namespace WinUIGallery.Pages;

/// <summary>
/// A page that displays a grouped collection of items.
/// </summary>
public sealed partial class AllControlsPage : ItemsPageBase
{
    public AllControlsPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        //if (e.Parameter is NavigationRootPageArgs args && args.NavigationRootPage.NavigationView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item => item.Name == "AllControlsItem") is NavigationViewItem item)
        //{
        //    item.IsSelected = true;
        //}

        Items = ControlInfoDataSource.Instance.Groups.Where(g => !g.IsSpecialSection).SelectMany(g => g.Items).OrderBy(i => i.Title).ToList();
    }
}
