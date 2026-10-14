// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;

namespace WinUIGallery.ControlPages;

public sealed partial class ListDetailsPage : Page
{
    private bool _showAdaptiveDetails;
    private bool _isAdaptiveWide;
    private string _adaptiveState = string.Empty;

    public List<ListDetailItem> Items { get; } =
    [
        new ListDetailItem { Id = 1, Title = "Item 1", Text = "Select an item to see its details." },
        new ListDetailItem { Id = 2, Title = "Item 2", Text = "The same data can be presented in one or two panes." },
        new ListDetailItem { Id = 3, Title = "Item 3", Text = "Changing the layout does not change the selection." },
        new ListDetailItem { Id = 4, Title = "Item 4", Text = "Use the back button to return to the list in a narrow layout." }
    ];

    public ListDetailsPage()
    {
        InitializeComponent();
    }

    private void OnPageLoaded(object sender, RoutedEventArgs args)
    {
        ChangeState("StackedListState");
        UpdateAdaptiveLayout();
    }

    private void OnStackedItemClick(object sender, ItemClickEventArgs args)
    {
        StackedList.SelectedItem = args.ClickedItem;
        ChangeState("StackedDetailState");
        StackedBack.Focus(FocusState.Programmatic);
    }

    private void OnStackedBackClick(object sender, RoutedEventArgs args)
    {
        ChangeState("StackedListState");
        FocusSelection(StackedList);
    }

    private void OnAdaptiveItemClick(object sender, ItemClickEventArgs args)
    {
        AdaptiveList.SelectedItem = args.ClickedItem;
        _showAdaptiveDetails = true;
        UpdateAdaptiveLayout(true);
    }

    private void OnAdaptiveBackClick(object sender, RoutedEventArgs args)
    {
        _showAdaptiveDetails = false;
        UpdateAdaptiveLayout();
        FocusSelection(AdaptiveList);
    }

    private void OnAdaptiveHostSizeChanged(object sender, SizeChangedEventArgs args)
    {
        AdaptiveExample.MaxWidth = args.NewSize.Width;
    }

    private void OnAdaptiveSizeChanged(object sender, SizeChangedEventArgs args)
    {
        if (IsLoaded)
        {
            UpdateAdaptiveLayout();
        }
    }

    private void UpdateAdaptiveLayout(bool focusDetails = false)
    {
        // Use the example's width, not the window width occupied by Gallery navigation.
        bool wide = AdaptiveExample.ActualWidth >= 641;
        if (_isAdaptiveWide && !wide)
        {
            _showAdaptiveDetails = AdaptiveList.SelectedItem is not null;
        }

        _isAdaptiveWide = wide;
        string state = wide ? "AdaptiveWide" :
            _showAdaptiveDetails ? "AdaptiveDetailOnly" : "AdaptiveListOnly";
        string label = wide ? "Side-by-side" :
            _showAdaptiveDetails ? "Stacked details" : "Stacked list";
        AdaptiveLayoutStatus.Text = $"{label}, available width: {AdaptiveExample.ActualWidth:0} epx";
        if (state == _adaptiveState)
        {
            return;
        }

        DependencyObject? focusedElement = FocusManager.GetFocusedElement(XamlRoot) as DependencyObject;
        bool focusWasInList = IsWithin(focusedElement, AdaptiveList);
        bool focusWasOnBack = focusedElement == AdaptiveBack;
        ChangeState(state);
        _adaptiveState = state;

        if (!wide && _showAdaptiveDetails && (focusDetails || focusWasInList))
        {
            AdaptiveBack.Focus(FocusState.Programmatic);
        }
        else if (wide && focusWasOnBack)
        {
            FocusSelection(AdaptiveList);
        }
    }

    private void ChangeState(string state)
    {
        if (!VisualStateManager.GoToState(this, state, false))
        {
            throw new InvalidOperationException($"Visual state '{state}' was not found.");
        }
    }

    private static void FocusSelection(ListView list)
    {
        // Visibility setters must finish layout before a newly shown item can receive focus.
        if (!list.DispatcherQueue.TryEnqueue(() =>
        {
            if (!list.IsLoaded || list.Visibility != Visibility.Visible)
            {
                return;
            }

            if (list.SelectedItem is object selectedItem)
            {
                list.ScrollIntoView(selectedItem);
                list.UpdateLayout();
                if (list.ContainerFromItem(selectedItem) is ListViewItem container)
                {
                    container.Focus(FocusState.Programmatic);
                    return;
                }
            }

            list.Focus(FocusState.Programmatic);
        }))
        {
            throw new InvalidOperationException("Unable to schedule list focus restoration.");
        }
    }

    private static bool IsWithin(DependencyObject? element, DependencyObject ancestor)
    {
        while (element is not null)
        {
            if (element == ancestor)
            {
                return true;
            }

            element = VisualTreeHelper.GetParent(element);
        }

        return false;
    }
}