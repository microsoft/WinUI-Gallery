// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class DropDownButtonPage : Page
{
    public DropDownButtonPage()
    {
        this.InitializeComponent();
    }

    private void EmailMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuFlyoutItem;
        if (menuItem != null)
        {
            EmailDropDownButton.Content = menuItem.Text;
        }
    }

    private void EmailIconMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuFlyoutItem;
        if (menuItem != null)
        {
            if (menuItem.Tag is string glyph)
            {
                EmailIcon.Glyph = glyph;
            }
            EmailIconText.Text = menuItem.Text;
        }
    }
}
