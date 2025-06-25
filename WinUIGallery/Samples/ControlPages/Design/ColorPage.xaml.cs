// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using WinUIGallery.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class ColorPage : Page
{
    int previousSelectedIndex = 0;

    public ColorPage()
    {
        this.InitializeComponent();
    }

    private void PageSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        SelectorBarItem selectedItem = sender.SelectedItem;
        int currentSelectedIndex = sender.Items.IndexOf(selectedItem);
        Type pageType = currentSelectedIndex switch
        {
            0 => typeof(TextSection),
            1 => typeof(FillSection),
            2 => typeof(StrokeSection),
            3 => typeof(BackgroundSection),
            4 => typeof(SignalSection),
            5 => typeof(HighContrastSection),
            _ => typeof(TextSection),
        };
        var slideNavigationTransitionEffect = currentSelectedIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

        NavigationFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });

        previousSelectedIndex = currentSelectedIndex;
    }

    private void PageSelector_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        PageSelector.SelectedItem = PageSelector.Items[0];
    }
}
