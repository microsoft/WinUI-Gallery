using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class TitleBarWindow : Window
{
    public TitleBarWindow()
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true; // Extend the content into the title bar and hide the default title bar
        this.SetTitleBar(titleBar); // Set the custom title bar
        navView.SelectedItem = navView.MenuItems.OfType<NavigationViewItem>().First();
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        navView.IsPaneOpen = !navView.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        navFrame.GoBack();
    }

    private void navView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = (NavigationViewItem)args.SelectedItem;
        if (selectedItem != null)
        {
            string selectedItemTag = ((string)selectedItem.Tag);
            sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
            string pageName = "WinUIGallery.SamplePages." + selectedItemTag;
            Type pageType = Type.GetType(pageName);
            navFrame.Navigate(pageType);
        }
    }
}
