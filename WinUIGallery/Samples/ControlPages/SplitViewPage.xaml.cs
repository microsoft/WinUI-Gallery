using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace WinUIGallery.ControlPages;

public sealed partial class SplitViewPage : Page
{
    private ObservableCollection<NavLink> _navLinks =
    [
        new NavLink() { Label = "People", Symbol = Symbol.People  },
        new NavLink() { Label = "Globe", Symbol = Symbol.Globe },
        new NavLink() { Label = "Message", Symbol = Symbol.Message },
        new NavLink() { Label = "Mail", Symbol = Symbol.Mail },
    ];

    public ObservableCollection<NavLink> NavLinks
    {
        get { return _navLinks; }
    }

    public SplitViewPage()
    {
        InitializeComponent();
    }

    private void NavLinksList_ItemClick(object sender, ItemClickEventArgs e) => content.Text = (e.ClickedItem as NavLink).Label + " Page";

    private void PanePlacement_Toggled(object sender, RoutedEventArgs e)
    {
        var ts = sender as ToggleSwitch;
        splitView.PanePlacement = ts.IsOn ? SplitViewPanePlacement.Right : SplitViewPanePlacement.Left;
    }

    private void displayModeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e) => splitView.DisplayMode = (SplitViewDisplayMode)Enum.Parse(typeof(SplitViewDisplayMode), (e.AddedItems[0] as ComboBoxItem).Content.ToString());

    private void paneBackgroundCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var colorString = (e.AddedItems[0] as ComboBoxItem).Content.ToString();

        VisualStateManager.GoToState(this, colorString, false);
    }
}

public class NavLink
{
    public string Label { get; set; }
    public Symbol Symbol { get; set; }
}
