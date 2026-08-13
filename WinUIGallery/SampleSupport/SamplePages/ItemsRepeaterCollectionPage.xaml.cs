// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.System;
using WinUIGallery.ControlPages;

namespace WinUIGallery.SamplePages;

public sealed partial class ItemsRepeaterCollectionPage : Page
{
    private CustomDataObject? _storedItem;
    private double _persistedScrollPosition;

    public ItemsRepeaterCollectionPage()
    {
        this.InitializeComponent();
        this.NavigationCacheMode = NavigationCacheMode.Enabled;

        repeater.ItemsSource = CustomDataObject.GetDataObjects(includeAllItems: true);
        repeater.ElementPrepared += Repeater_ElementPrepared;
    }

    private void Repeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        args.Element.Tapped -= Item_Tapped;
        args.Element.Tapped += Item_Tapped;
        args.Element.KeyDown -= Item_KeyDown;
        args.Element.KeyDown += Item_KeyDown;
    }

    private void Item_Tapped(object sender, TappedRoutedEventArgs e)
    {
        NavigateToItem(sender as FrameworkElement);
    }

    private void Item_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        // Activate the focused item with Enter or Space, matching list-style keyboard behavior.
        if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Space)
        {
            NavigateToItem(sender as FrameworkElement);
            e.Handled = true;
        }
    }

    private void NavigateToItem(FrameworkElement element)
    {
        if (element == null)
        {
            return;
        }

        // Get the data item for this container.
        _storedItem = repeater.ItemsSourceView.GetAt(repeater.GetElementIndex(element)) as CustomDataObject;

        // Unlike ListView, ItemsRepeater doesn't have PrepareConnectedAnimation().
        // Instead, find the named element in the template and use ConnectedAnimationService directly.
        if (FindChildByName(element, "connectedElement") is UIElement animationSource)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", animationSource);
        }

        // Remember scroll position for restoration on back navigation.
        _persistedScrollPosition = scrollViewer.VerticalOffset;

        Frame.Navigate(typeof(DetailedInfoPage), _storedItem, new SuppressNavigationTransitionInfo());
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (_storedItem == null)
        {
            return;
        }

        // Restore scroll position so the connected element is visible.
        scrollViewer.ChangeView(null, _persistedScrollPosition, null, disableAnimation: true);
        UpdateLayout();

        // Find the element for the stored item so we can run the back animation and restore focus to it.
        int index = repeater.ItemsSourceView.IndexOf(_storedItem);
        var container = repeater.TryGetElement(index) as FrameworkElement;

        ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackConnectedAnimation");
        if (animation != null)
        {
            animation.Configuration = new DirectConnectedAnimationConfiguration();

            // Start the back animation from the target element inside the item's template.
            if (container != null
                && FindChildByName(container, "connectedElement") is UIElement animationTarget)
            {
                animation.TryStart(animationTarget);
            }
        }

        // Return keyboard focus to the item the user activated so keyboard and Narrator users
        // resume from where they left off instead of losing their place. Focus() is available on
        // the Grid because it is a UIElement with IsTabStop set in the template.
        if (container != null)
        {
            container.Focus(FocusState.Programmatic);
        }
        else
        {
            repeater.Focus(FocusState.Programmatic);
        }
    }

    /// <summary>
    /// Walks the visual tree to find a named child element within a template.
    /// </summary>
    private static UIElement? FindChildByName(DependencyObject parent, string name)
    {
        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child is FrameworkElement fe && fe.Name == name)
            {
                return fe;
            }

            UIElement? result = FindChildByName(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
