// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class MenuFlyoutPage : Page
{
    public MenuFlyoutPage()
    {
        this.InitializeComponent();
    }

    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem selectedItem)
        {
            string sortOption = selectedItem.Tag.ToString();
            switch (sortOption)
            {
                case "rating":
                    //SortByRating();
                    break;
                case "match":
                    //SortByMatch();
                    break;
                case "distance":
                    //SortByDistance();
                    break;
            }
            Control1Output.Text = "Sort by: " + sortOption;
        }
    }

    private void Example5_Loaded(object sender, RoutedEventArgs e)
    {

    }
}
