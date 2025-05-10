using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System;
using Windows.Foundation;
using System.Collections.Specialized;

namespace WinUIGallery.Controls;

/// <summary>
/// A UserControl that displays a horizontal selector bar with overflow support.
/// </summary>
public sealed partial class AdaptiveSelectorBar : UserControl
{
    /// <summary>
    /// Items currently visible on the bar.
    /// </summary>
    private ObservableCollection<AdaptiveSelectorBarItem> _visibleItems { get; } = new();

    /// <summary>
    /// Items that don’t fit in the visible area, accessible through an overflow menu.
    /// </summary>
    private ObservableCollection<AdaptiveSelectorBarItem> _overflowItems { get; } = new();

    /// <summary>
    /// Prevents re-entrant layout updates.
    /// </summary>
    private bool _isUpdatingLayout;

    /// <summary>
    /// Identifies the <see cref="Items"/> dependency property.
    /// XAML dependency property for binding the list of items.
    /// </summary>
    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(
            nameof(Items),
            typeof(ObservableCollection<AdaptiveSelectorBarItem>),
            typeof(AdaptiveSelectorBar),
            new PropertyMetadata(new ObservableCollection<AdaptiveSelectorBarItem>(), OnItemsChanged));

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> dependency property.
    /// Dependency property for tracking the selected item.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), 
            typeof(AdaptiveSelectorBarItem), 
            typeof(AdaptiveSelectorBar),
            new PropertyMetadata(null, OnSelectedItemChanged));

    /// <summary>
    /// Property wrapper for the bound items.
    /// </summary>
    public ObservableCollection<AdaptiveSelectorBarItem> Items
    {
        get => (ObservableCollection<AdaptiveSelectorBarItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Property wrapper for the currently selected item.
    /// </summary>
    public AdaptiveSelectorBarItem SelectedItem
    {
        get => (AdaptiveSelectorBarItem)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Event raised whenever the selected item changes.
    /// </summary>
    public event EventHandler<AdaptiveSelectorBarItem> SelectionChanged;

    /// <summary>
    /// Initializes the <c>AdaptiveSelectorBar</c> visual tree and registers post-load layout handling.
    /// </summary>
    public AdaptiveSelectorBar()
    {
        InitializeComponent();
        Items.Clear(); // Ensures a fresh collection.
        Loaded += OnLoaded; // Hook for post-load layout/setup.
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _visibleItems.Clear();
        _overflowItems.Clear();

        AdaptiveSelectorBarItem selected = null;

        // Resolve multiple selected items by picking the last one marked IsSelected.
        foreach (var item in Items)
        {
            if (item.IsSelected)
                selected = item;
        }

        if (selected != null)
        {
            // Unselect all but the one we kept.
            foreach (var item in Items)
                item.IsSelected = item == selected;

            SelectedItem = selected; // Triggers selection logic downstream.
        }

        // Attach to parent resize event for responsive layout.
        if (Parent is FrameworkElement parent)
            parent.SizeChanged += OnParentSizeChanged;

        UpdateLayoutAndOverflow();
    }

    private void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            Width = element.Width; // Sync width.
            UpdateLayoutAndOverflow(); // Re-calculate layout.
        }
    }

    private void OnItemClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is AdaptiveSelectorBarItem item)
        {
            // Unselect everything.
            foreach (var i in Items)
                i.IsSelected = false;

            // Select clicked item.
            item.IsSelected = true;
            SelectedItem = item;
        }
    }

    private void OnMoreClicked(object sender, RoutedEventArgs e)
    {
        OverflowFlyout.Items.Clear();

        // Rebuild overflow menu with hidden items.
        foreach (var item in _overflowItems)
        {
            var menuItem = new MenuFlyoutItem
            {
                Text = item.DisplayText,
                Icon = new FontIcon { Glyph = item.FontIconGlyph },
                Tag = item,
                Padding = new Thickness(11, 5, 11, 6)
            };

            // Selection logic for overflow items.
            menuItem.Click += (s, _) =>
            {
                foreach (var i in Items)
                    i.IsSelected = false;

                item.IsSelected = true;
                SelectedItem = (AdaptiveSelectorBarItem)((MenuFlyoutItem)s).Tag;
            };

            OverflowFlyout.Items.Add(menuItem);
        }
    }

    /// <summary>
    /// Handles changes to the Items property.
    /// </summary>
    private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AdaptiveSelectorBar bar)
        {
            // Remove old collection listener.
            if (e.OldValue is ObservableCollection<AdaptiveSelectorBarItem> oldItems)
                oldItems.CollectionChanged -= bar.OnItemsCollectionChanged;

            // Add new listener.
            if (e.NewValue is ObservableCollection<AdaptiveSelectorBarItem> newItems)
                newItems.CollectionChanged += bar.OnItemsCollectionChanged;

            bar.UpdateLayoutAndOverflow(); // Refresh layout.
        }
    }

    /// <summary>
    /// Called when items are added/removed from the collection.
    /// </summary>
    private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateLayoutAndOverflow();
    }

    /// <summary>
    /// Handles changes to the SelectedItem property.
    /// </summary>
    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AdaptiveSelectorBar bar && e.NewValue is AdaptiveSelectorBarItem item)
        {
            bar.SelectionChanged?.Invoke(bar, item);
            bar.EnsureSelectedItemVisible();
        }
    }

    /// <summary>
    /// Main layout logic — figures out which items fit and manages overflow.
    /// </summary>
    private void UpdateLayoutAndOverflow()
    {
        if (MeasurementPanel == null || Items.Count == 0)
            return;

        MeasurementPanel.Children.Clear();

        // Build buttons for measurement pass.
        var buttonMap = Items.ToDictionary(item => item, item =>
        {
            var icon = new FontIcon { FontSize = 16, Glyph = item.FontIconGlyph };
            var text = new TextBlock { Text = item.DisplayText };
            var grid = new Grid { ColumnSpacing = 8 };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(icon, 0);
            Grid.SetColumn(text, 1);
            grid.Children.Add(icon);
            grid.Children.Add(text);

            var button = new Button
            {
                Tag = item,
                Padding = new Thickness(11, 5, 11, 6),
                BorderThickness = new Thickness(0),
                Content = grid
            };

            MeasurementPanel.Children.Add(button);
            return button;
        });

        // Defer layout logic to the dispatcher to avoid layout timing issues.
        DispatcherQueue.TryEnqueue(() =>
        {
            if (_isUpdatingLayout)
                return;

            _isUpdatingLayout = true;
            MeasurementPanel.UpdateLayout();

            double availableWidth = ActualWidth - 44 - 12; // Reserve width for "More" button.
            double usedWidth = 0;

            // Pre-measure each button.
            var itemWidths = buttonMap.ToDictionary(pair => pair.Key, pair =>
            {
                pair.Value.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                return pair.Value.DesiredSize.Width;
            });

            var newVisible = new ObservableCollection<AdaptiveSelectorBarItem>();
            var newOverflow = new ObservableCollection<AdaptiveSelectorBarItem>();

            bool overflowStarted = false;
            AdaptiveSelectorBarItem displacedItem = null;

            foreach (var item in Items)
            {
                double width = itemWidths[item];

                if (!overflowStarted && usedWidth + width <= availableWidth)
                {
                    newVisible.Add(item);
                    usedWidth += width;
                }
                else
                {
                    overflowStarted = true;

                    if (item == SelectedItem)
                    {
                        // Force selected item to be visible by displacing the last one.
                        if (newVisible.Count > 0)
                        {
                            displacedItem = newVisible.Last();
                            newVisible.RemoveAt(newVisible.Count - 1);
                            newOverflow.Add(displacedItem);

                            newVisible.Add(item);
                        }
                        else
                        {
                            newOverflow.Add(item);
                        }
                    }
                    else
                    {
                        newOverflow.Add(item);
                    }
                }
            }

            // Swap items to make room for SelectedItem if it's still not visible.
            if (SelectedItem != null && !newVisible.Contains(SelectedItem))
            {
                double selWidth = itemWidths[SelectedItem];
                for (int i = newVisible.Count - 1; i >= 0; i--)
                {
                    var swapCandidate = newVisible[i];
                    double swapWidth = itemWidths[swapCandidate];

                    if (usedWidth - swapWidth + selWidth <= availableWidth)
                    {
                        newVisible[i] = SelectedItem;
                        newOverflow.Remove(SelectedItem);
                        newOverflow.Insert(0, swapCandidate);
                        break;
                    }
                }
            }

            ApplyItemCollections(newVisible, newOverflow);
            _isUpdatingLayout = false;
        });
    }

    /// <summary>
    /// Syncs the visible and overflow collections.
    /// </summary>
    private void ApplyItemCollections(ObservableCollection<AdaptiveSelectorBarItem> visible, ObservableCollection<AdaptiveSelectorBarItem> overflow)
    {
        _visibleItems.Clear();
        foreach (var item in visible)
            _visibleItems.Add(item);

        _overflowItems.Clear();
        foreach (var item in overflow)
            _overflowItems.Add(item);

        // Show or hide the "More" button depending on overflow content.
        MoreButton.Visibility = _overflowItems.Any() ? Visibility.Visible : Visibility.Collapsed;

        VisibleItemsControl.UpdateLayout();
    }

    /// <summary>
    /// Makes sure the selected item is in the visible area, not overflow.
    /// </summary>
    private void EnsureSelectedItemVisible()
    {
        if (SelectedItem == null || _visibleItems.Contains(SelectedItem))
            return;

        if (_visibleItems.Count > 0)
        {
            var lastVisible = _visibleItems.Last();
            _visibleItems.RemoveAt(_visibleItems.Count - 1);
            _overflowItems.Insert(0, lastVisible);
        }

        _visibleItems.Add(SelectedItem);
        _overflowItems.Remove(SelectedItem);
    }
}