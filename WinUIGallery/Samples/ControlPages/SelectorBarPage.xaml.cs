//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using WinUIGallery.SamplePages;

namespace WinUIGallery.ControlPages;


public sealed partial class SelectorBarPage : Page
{
    int previousSelectedIndex = 0;

    public ObservableCollection<SolidColorBrush> PinkColorCollection = [];
    public ObservableCollection<SolidColorBrush> PlumColorCollection = [];
    public ObservableCollection<SolidColorBrush> PowderBlueColorCollection = [];

    public SelectorBarPage()
    {
        InitializeComponent();

        PopulateColorCollections();
    }

    private void SelectorBar2_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        SelectorBarItem selectedItem = sender.SelectedItem;
        int currentSelectedIndex = sender.Items.IndexOf(selectedItem);
        System.Type pageType = currentSelectedIndex switch
        {
            0 => typeof(SamplePage1),
            1 => typeof(SamplePage2),
            2 => typeof(SamplePage3),
            3 => typeof(SamplePage4),
            _ => typeof(SamplePage5),
        };
        var slideNavigationTransitionEffect = currentSelectedIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

        ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });

        previousSelectedIndex = currentSelectedIndex;
    }

    private void SelectorBar3_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem == SelectorBarItemPink)
        {
            ItemsView3.ItemsSource = PinkColorCollection;
        }
        else if (sender.SelectedItem == SelectorBarItemPlum)
        {
            ItemsView3.ItemsSource = PlumColorCollection;
        }
        else
        {
            ItemsView3.ItemsSource = PowderBlueColorCollection;
        }
    }

    private void PopulateColorCollections()
    {
        SolidColorBrush solidColorBrush = new(Microsoft.UI.Colors.Pink);

        for (int colorInstance = 0; colorInstance < 5; colorInstance++)
        {
            PinkColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Plum);

        for (int colorInstance = 0; colorInstance < 7; colorInstance++)
        {
            PlumColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.PowderBlue);

        for (int colorInstance = 0; colorInstance < 4; colorInstance++)
        {
            PowderBlueColorCollection.Add(solidColorBrush);
        }
    }
}
