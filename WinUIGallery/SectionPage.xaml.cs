//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Data;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics
{
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
            var group = await ControlInfoDataSource.Instance.GetGroupAsync((string)args.Parameter);

            var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItemBase)navigationRootPage.NavigationView.MenuItems.Single(i => (string)((Microsoft.UI.Xaml.Controls.NavigationViewItemBase)i).Tag == group.UniqueId);
            menuItem.IsSelected = true;
            navigationRootPage.NavigationView.Header = group;

            Items = group.Items.OrderBy(i => i.Title).ToList();
        }

        protected override bool GetIsNarrowLayoutState()
        {
            return LayoutVisualStates.CurrentState == NarrowLayout;
        }
    }
}
