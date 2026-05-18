// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;
using WinUIGallery.SamplePages;

namespace WinUIGallery.ControlPages;


public sealed partial class SelectorBarPage : Page
{
    int previousSelectedIndex = 0;

    public ObservableCollection<SolidColorBrush> PinkColorCollection = new ObservableCollection<SolidColorBrush>();
    public ObservableCollection<SolidColorBrush> PlumColorCollection = new ObservableCollection<SolidColorBrush>();
    public ObservableCollection<SolidColorBrush> PowderBlueColorCollection = new ObservableCollection<SolidColorBrush>();

    public SelectorBarPage()
    {
        this.InitializeComponent();

        PopulateColorCollections();
    }

    private void SelectorBar2_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        SelectorBarItem selectedItem = sender.SelectedItem;
        int currentSelectedIndex = sender.Items.IndexOf(selectedItem);
        System.Type pageType;

        switch (currentSelectedIndex)
        {
            case 0:
                pageType = typeof(SamplePage1);
                break;
            case 1:
                pageType = typeof(SamplePage2);
                break;
            case 2:
                pageType = typeof(SamplePage3);
                break;
            case 3:
                pageType = typeof(SamplePage4);
                break;
            default:
                pageType = typeof(SamplePage5);
                break;
        }

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
        SolidColorBrush solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Pink);

        for (int colorInstance = 0; colorInstance < 5; colorInstance++)
        {
            this.PinkColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Plum);

        for (int colorInstance = 0; colorInstance < 7; colorInstance++)
        {
            this.PlumColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.PowderBlue);

        for (int colorInstance = 0; colorInstance < 4; colorInstance++)
        {
            this.PowderBlueColorCollection.Add(solidColorBrush);
        }
    }
}
