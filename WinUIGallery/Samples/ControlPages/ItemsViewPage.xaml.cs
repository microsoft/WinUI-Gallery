// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WinUIGallery.Pages;

namespace WinUIGallery.ControlPages;

public sealed partial class ItemsViewPage : ItemsPageBase
{
    private LinedFlowLayout linedFlowLayout = null;
    private StackLayout stackLayout = null;
    private UniformGridLayout uniformGridLayout = null;

    private DataTemplate linedFlowLayoutItemTemplate = null;
    private DataTemplate stackLayoutItemTemplate = null;
    private DataTemplate uniformGridLayoutItemTemplate = null;

    private bool applyLinedFlowLayoutLineHeightAsync = false;
    private bool applyLinedFlowLayoutOptionsAsync = false;

    public ItemsViewPage()
    {
        this.InitializeComponent();
        this.DataContext = this;
        this.Loaded += ItemsViewPage_Loaded;
    }

    private void ItemsViewPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= ItemsViewPage_Loaded;

        if (SwappableLayoutsItemsView != null)
        {
            SwappableLayoutsItemsView.ScrollView.ViewChanged += SwappableLayoutsItemsViewScrollView_ViewChanged;

            linedFlowLayout = SwappableLayoutsItemsView.Layout as LinedFlowLayout;
            linedFlowLayoutItemTemplate = SwappableLayoutsItemsView.ItemTemplate as DataTemplate;
        }

        // Get data objects and place them into an ObservableCollection
        List<CustomDataObject> tempList = CustomDataObject.GetDataObjects(includeAllItems: true);
        ObservableCollection<CustomDataObject> Items = new ObservableCollection<CustomDataObject>(tempList);

        if (BasicItemsView != null)
        {
            BasicItemsView.ItemsSource = Items;
        }

        Task.Run(delegate ()
        {
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                if (SwappableLayoutsItemsView != null)
                {
                    // Temporarily setting the items source asynchronously to avoid LinedFlowLayout bug.
                    SwappableLayoutsItemsView.ItemsSource = Items;
                }
            });
        });

        if (SwappableSelectionModesItemsView != null)
        {
            SwappableSelectionModesItemsView.ItemsSource = Items;
        }
    }

    // Example1
    private void BasicItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs e)
    {
        tblBasicInvokeOutput.Text = "You invoked " + (e.InvokedItem as CustomDataObject).Title + ".";
    }

    // Example2
    private void ApplyLinedFlowLayoutLineHeight()
    {
        if (linedFlowLayout != null && rbSmallLineHeight != null)
        {
            linedFlowLayout.LineHeight = (bool)rbSmallLineHeight.IsChecked ? 80 : 160;
        }
    }

    private void ApplyLinedFlowLayoutOptions()
    {
        if (linedFlowLayout != null && nbLineSpacing != null && nbMinItemSpacing != null)
        {
            linedFlowLayout.LineSpacing = nbLineSpacing.Value;
            linedFlowLayout.MinItemSpacing = nbMinItemSpacing.Value;
        }
    }

    private void RbLayout_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && SwappableLayoutsItemsView != null)
        {
            switch (rb.Content.ToString())
            {
                case "LinedFlowLayout":
                    if (linedFlowLayout != null && linedFlowLayoutItemTemplate != null)
                    {
                        SwappableLayoutsItemsView.Layout = linedFlowLayout;
                        SwappableLayoutsItemsView.ItemTemplate = linedFlowLayoutItemTemplate;

                        spLinedFlowLayoutOptions.Visibility = Visibility.Visible;
                        spStackLayoutOptions.Visibility = Visibility.Collapsed;
                        spUniformGridLayoutOptions.Visibility = Visibility.Collapsed;
                    }
                    break;

                case "StackLayout":
                    if (stackLayout == null)
                    {
                        stackLayout = new StackLayout()
                        {
                            Spacing = 5
                        };
                    }

                    if (stackLayoutItemTemplate == null)
                    {
                        stackLayoutItemTemplate = Resources["StackLayoutItemTemplate"] as DataTemplate;
                    }

                    SwappableLayoutsItemsView.Layout = stackLayout;
                    SwappableLayoutsItemsView.ItemTemplate = stackLayoutItemTemplate;

                    spLinedFlowLayoutOptions.Visibility = Visibility.Collapsed;
                    spStackLayoutOptions.Visibility = Visibility.Visible;
                    spUniformGridLayoutOptions.Visibility = Visibility.Collapsed;
                    break;

                case "UniformGridLayout":
                    if (uniformGridLayout == null)
                    {
                        uniformGridLayout = new UniformGridLayout()
                        {
                            MinRowSpacing = 5,
                            MinColumnSpacing = 5,
                            MaximumRowsOrColumns = 3
                        };
                    }

                    if (uniformGridLayoutItemTemplate == null)
                    {
                        uniformGridLayoutItemTemplate = Resources["UniformGridLayoutItemTemplate"] as DataTemplate;
                    }

                    SwappableLayoutsItemsView.Layout = uniformGridLayout;
                    SwappableLayoutsItemsView.ItemTemplate = uniformGridLayoutItemTemplate;

                    spLinedFlowLayoutOptions.Visibility = Visibility.Collapsed;
                    spStackLayoutOptions.Visibility = Visibility.Collapsed;
                    spUniformGridLayoutOptions.Visibility = Visibility.Visible;
                    break;
            }
        }
    }

    private void RbLineHeight_Checked(object sender, RoutedEventArgs e)
    {
        if (SwappableLayoutsItemsView != null && SwappableLayoutsItemsView.ScrollView != null && SwappableLayoutsItemsView.ScrollView.VerticalOffset != 0)
        {
            SwappableLayoutsItemsView.ScrollView.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled, ScrollingSnapPointsMode.Ignore));
            applyLinedFlowLayoutLineHeightAsync = true;
        }
        else
        {
            ApplyLinedFlowLayoutLineHeight();
        }
    }

    private void NbLinedFlowLayoutOptions_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (SwappableLayoutsItemsView != null && SwappableLayoutsItemsView.ScrollView != null && SwappableLayoutsItemsView.ScrollView.VerticalOffset != 0)
        {
            SwappableLayoutsItemsView.ScrollView.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled, ScrollingSnapPointsMode.Ignore));
            applyLinedFlowLayoutOptionsAsync = true;
        }
        else
        {
            ApplyLinedFlowLayoutOptions();
        }
    }

    private void NbStackLayoutOptions_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (stackLayout != null)
        {
            stackLayout.Spacing = nbSpacing.Value;
        }
    }

    private void NbUniformGridLayoutOptions_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (uniformGridLayout != null)
        {
            uniformGridLayout.MinColumnSpacing = nbMinColumnSpacing.Value;
            uniformGridLayout.MinRowSpacing = nbMinRowSpacing.Value;
            uniformGridLayout.MaximumRowsOrColumns = (int)nbMaximumRowsOrColumns.Value;
        }
    }

    private void SwappableLayoutsItemsViewScrollView_ViewChanged(ScrollView sender, object args)
    {
        if (sender.VerticalOffset == 0 && (applyLinedFlowLayoutOptionsAsync || applyLinedFlowLayoutLineHeightAsync))
        {
            if (applyLinedFlowLayoutOptionsAsync)
            {
                applyLinedFlowLayoutOptionsAsync = false;
                ApplyLinedFlowLayoutOptions();
            }

            if (applyLinedFlowLayoutLineHeightAsync)
            {
                applyLinedFlowLayoutLineHeightAsync = false;
                ApplyLinedFlowLayoutLineHeight();
            }
        }
    }

    // Example3
    private void SwappableSelectionModesItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs e)
    {
        tblInvocationOutput.Text = "You invoked " + (e.InvokedItem as CustomDataObject).Title + ".";
    }

    private void SwappableSelectionModesItemsView_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs e)
    {
        if (SwappableSelectionModesItemsView != null)
        {
            tblSelectionOutput.Text = string.Format("You have selected {0} item(s).", SwappableSelectionModesItemsView.SelectedItems.Count);
        }
    }

    private void CmbSelectionMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SwappableSelectionModesItemsView != null && sender is ComboBox cmb)
        {
            SwappableSelectionModesItemsView.SelectionMode = (ItemsViewSelectionMode)cmb.SelectedIndex;
        }
    }

    private void ChkIsItemInvokedEnabled_IsCheckedChanged(object sender, RoutedEventArgs e)
    {
        tblInvocationOutput.Text = string.Empty;

        if (SwappableSelectionModesItemsView != null && chkIsItemInvokedEnabled != null)
        {
            SwappableSelectionModesItemsView.IsItemInvokedEnabled = (bool)chkIsItemInvokedEnabled.IsChecked;
        }
    }
}
