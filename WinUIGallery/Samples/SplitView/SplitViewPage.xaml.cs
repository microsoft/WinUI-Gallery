// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;


namespace WinUIGallery.ControlPages;

public sealed partial class SplitViewPage : Page
{
    private ObservableCollection<NavLink> _navLinks = new ObservableCollection<NavLink>()
    {
        new NavLink() { Label = "People", Symbol = Symbol.People  },
        new NavLink() { Label = "Globe", Symbol = Symbol.Globe },
        new NavLink() { Label = "Message", Symbol = Symbol.Message },
        new NavLink() { Label = "Mail", Symbol = Symbol.Mail },
    };

    public ObservableCollection<NavLink> NavLinks
    {
        get { return _navLinks; }
    }

    public SplitViewPage()
    {
        this.InitializeComponent();
        this.Loaded += SplitViewPage_Loaded;
    }

    private void SplitViewPage_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateNavLinkItemLayout();
    }

    private void NavLinksList_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not NavLink navLink)
        {
            return;
        }

        content.Text = navLink.Label + " Page";
    }

    private void PanePlacement_Toggled(object sender, RoutedEventArgs e)
    {
        if ((sender as ToggleSwitch)?.IsOn is true)
        {
            splitView.PanePlacement = SplitViewPanePlacement.Right;
        }
        else
        {
            splitView.PanePlacement = SplitViewPanePlacement.Left;
        }

        UpdateNavLinkItemLayout();
    }

    private void UpdateNavLinkItemLayout()
    {
        if (splitView.PanePlacement == SplitViewPanePlacement.Right)
        {
            VisualStateManager.GoToState(this, "RightIconLayout", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "LeftIconLayout", false);
        }
    }

    private void togglePaneButton_CheckedChanged(object sender, RoutedEventArgs e)
    {
        UpdateNavLinkItemLayout();
    }

    private void displayModeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((e.AddedItems[0] as ComboBoxItem)?.Content.ToString() is not string displayMode)
        {
            return;
        }

        splitView.DisplayMode = (SplitViewDisplayMode)Enum.Parse(typeof(SplitViewDisplayMode), displayMode);
    }

    private void paneBackgroundCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((e.AddedItems[0] as ComboBoxItem)?.Content.ToString() is not string colorString)
        {
            return;
        }

        VisualStateManager.GoToState(this, colorString, false);
    }
}

public class NavLink
{
    public string Label { get; set; } = string.Empty;
    public Symbol Symbol { get; set; }
}
