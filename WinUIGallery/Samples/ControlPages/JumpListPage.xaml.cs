// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.StartScreen;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class JumpListPage : Page
{
    public JumpListPage()
    {
        this.InitializeComponent();
    }

    private async void AddTasksButton_Click(object sender, RoutedEventArgs e)
    {
        if (!NativeMethods.IsAppPackaged)
        {
            return;
        }

        JumpList jumpList = await JumpList.LoadCurrentAsync();

        JumpListItem composeTask = JumpListItem.CreateWithArguments("/compose", "New Message");
        composeTask.Description = "Compose a new message";
        composeTask.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        JumpListItem searchTask = JumpListItem.CreateWithArguments("/search", "Search");
        searchTask.Description = "Search for items";
        searchTask.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        jumpList.Items.Add(composeTask);
        jumpList.Items.Add(searchTask);

        await jumpList.SaveAsync();
    }

    private async void ClearTasksButton_Click(object sender, RoutedEventArgs e)
    {
        if (!NativeMethods.IsAppPackaged)
        {
            return;
        }

        JumpList jumpList = await JumpList.LoadCurrentAsync();
        jumpList.Items.Clear();
        await jumpList.SaveAsync();
    }

    private async void AddCustomGroupButton_Click(object sender, RoutedEventArgs e)
    {
        if (!NativeMethods.IsAppPackaged)
        {
            return;
        }

        JumpList jumpList = await JumpList.LoadCurrentAsync();

        JumpListItem item1 = JumpListItem.CreateWithArguments("/project-alpha", "Project Alpha");
        item1.GroupName = "Projects";
        item1.Description = "Open Project Alpha";
        item1.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        JumpListItem item2 = JumpListItem.CreateWithArguments("/project-beta", "Project Beta");
        item2.GroupName = "Projects";
        item2.Description = "Open Project Beta";
        item2.Logo = new Uri("ms-appx:///Assets/Tiles/AppList.targetsize-48.png");

        jumpList.Items.Add(item1);
        jumpList.Items.Add(item2);

        await jumpList.SaveAsync();
    }
}