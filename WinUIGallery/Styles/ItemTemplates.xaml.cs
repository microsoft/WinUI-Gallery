//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using WinUIGallery.Models;
using WinUIGallery.Pages;

namespace WinUIGallery;

public sealed partial class ItemTemplates : ResourceDictionary
{
    public ItemTemplates()
    {
        this.InitializeComponent();
    }

    private void ControlItemTemplate_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (button.Tag is ControlInfoDataItem item)
            {
                FrameworkElement element = sender as FrameworkElement;
                Page parentPage = element?.FindAscendant<Page>();
                NavigationRootPage.GetForElement(parentPage).Navigate(typeof(ItemPage), item.UniqueId, new DrillInNavigationTransitionInfo());
            }
        }
    }
}
