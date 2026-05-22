// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class ThemeTransitionPage : Page
{

    private int _itemCount = 10;
    public ThemeTransitionPage()
    {
        this.InitializeComponent();

        for (int i = 0; i < _itemCount; i++)
        {
            AddRemoveListView.Items.Add(new ListViewItem() { Content = "Item " + i });
        }

        AddItemsToContentListView();
    }

    private void ShowPopupButton_Click(object sender, RoutedEventArgs e)
    {
        ExamplePopup.IsOpen = true;
        ClosePopupButton.Focus(FocusState.Programmatic);
    }

    private void ClosePopupButton_Click(object sender, RoutedEventArgs e)
    {
        ExamplePopup.IsOpen = false;
        ShowPopupButton.Focus(FocusState.Programmatic);
    }

    private void ContentRefreshButton_Click(object sender, RoutedEventArgs e)
    {
        AddItemsToContentListView(true);
        UIHelper.AnnounceActionForAccessibility((UIElement)sender, "Data refreshed.", "ContentRefreshNotificationId");
    }

    private void AddItemsToContentListView(bool ShowDifferentContent = false)
    {
        var items = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            items.Add(ShowDifferentContent ? "Updated content " + i : "Item " + i);
        }

        ContentList.ItemsSource = items;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddRemoveListView.Items.Add(new ListViewItem() { Content = "New Item " + _itemCount.ToString() });
        _itemCount++;
        UIHelper.AnnounceActionForAccessibility((UIElement)sender, "Item added.", "AddDeleteItemAddedNotificationId");
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (AddRemoveListView.Items.Count > 0)
        {
            AddRemoveListView.Items.RemoveAt(0);
            UIHelper.AnnounceActionForAccessibility((UIElement)sender, "Item deleted.", "AddDeleteItemDeletedNotificationId");
        }
    }

    private void RepositionButton_Click(object sender, RoutedEventArgs e)
    {
        MiddleElement.Visibility = MiddleElement.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        string announcement = MiddleElement.Visibility == Visibility.Visible ? "Element restored." : "Element repositioned.";
        UIHelper.AnnounceActionForAccessibility((UIElement)sender, announcement, "RepositionNotificationId");
    }

    private void EntranceAddButton_Click(object sender, RoutedEventArgs e)
    {
        int value = Convert.ToInt32((sender as Button)?.Tag);

        for (int i = 0; i < value; i++)
        {
            Thickness thickness = new Thickness(5.0);
            EntranceStackPanel.Children.Add(new Rectangle() { Width = 50, Height = 50, Margin = thickness, Fill = new SolidColorBrush(Microsoft.UI.Colors.LightBlue) });
        }

        string announcement = value == 1 ? "Added 1 rectangle." : $"Added {value} rectangles.";
        UIHelper.AnnounceActionForAccessibility((UIElement)sender, announcement, "EntranceAddNotificationId");
    }

    private void EntranceClearButton_Click(object sender, RoutedEventArgs e)
    {
        EntranceStackPanel.Children.Clear();
        UIHelper.AnnounceActionForAccessibility((UIElement)sender, "All rectangles cleared.", "EntranceClearNotificationId");
    }

    private void AddDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        AddButton_Click(sender, e);
        DeleteButton_Click(sender, e);
        UIHelper.AnnounceActionForAccessibility((UIElement)sender, "Item added and item deleted.", "AddDeleteBothNotificationId");
    }
}
