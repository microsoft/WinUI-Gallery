// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
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
        // Attach a Tapped handler to each item container so we can navigate on click.
        args.Element.Tapped -= Item_Tapped;
        args.Element.Tapped += Item_Tapped;
    }

    private void Item_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
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

        ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackConnectedAnimation");
        if (animation != null)
        {
            animation.Configuration = new DirectConnectedAnimationConfiguration();

            // Find the element for the stored item and start the back animation.
            int index = repeater.ItemsSourceView.IndexOf(_storedItem);
            if (repeater.TryGetElement(index) is FrameworkElement container
                && FindChildByName(container, "connectedElement") is UIElement animationTarget)
            {
                animation.TryStart(animationTarget);
            }
        }

        repeater.Focus(FocusState.Programmatic);
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
