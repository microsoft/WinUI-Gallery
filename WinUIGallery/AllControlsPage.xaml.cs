//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using WinUIGallery.Data;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;

namespace WinUIGallery
{
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
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;

            var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.ElementAt(2);
            menuItem.IsSelected = true;

            Items = ControlInfoDataSource.Instance.Groups.Where(g => !g.IsSpecialSection).SelectMany(g => g.Items).OrderBy(i => i.Title).ToList();
        }
    }
}
