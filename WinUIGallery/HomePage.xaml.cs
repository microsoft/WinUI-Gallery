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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WinUIGallery
{
    public sealed partial class HomePage : ItemsPageBase
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        public string WinAppSdkDetails => App.WinAppSdkDetails;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
            var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.First();
            menuItem.IsSelected = true;

            Items = ControlInfoDataSource.Instance.Groups.SelectMany(g => g.Items.Where(i => i.BadgeString != null)).OrderBy(i => i.Title).ToList();
            itemsCVS.Source = FormatData();
        }

        private ObservableCollection<GroupInfoList> FormatData()
        {
            var query = from item in this.Items
                        group item by item.BadgeString into g
                        orderby g.Key
                        select new GroupInfoList(g) { Key = g.Key };

            ObservableCollection<GroupInfoList> groupList = new ObservableCollection<GroupInfoList>(query);

            //Move Preview samples to the back of the list
            var previewGroup = groupList.ElementAt(1);
            if (previewGroup?.Key.ToString() == "Preview")
            {
                groupList.RemoveAt(1);
                groupList.Insert(groupList.Count, previewGroup);
            }

            foreach (var item in groupList)
            {
                switch (item.Key.ToString())
                {
                    case "New":
                        item.Title = "Recently added samples";
                        break;
                    case "Updated":
                        item.Title = "Recently updated samples";
                        break;
                    case "Preview":
                        item.Title = "Preview samples";
                        break;
                }
            }

            return groupList;
        }

        protected override bool GetIsNarrowLayoutState()
        {
            return LayoutVisualStates.CurrentState == NarrowLayout;
        }
    }

    public partial class GroupInfoList : List<object>
    {
        public GroupInfoList(IEnumerable<object> items) : base(items) { }

        public object Key { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
