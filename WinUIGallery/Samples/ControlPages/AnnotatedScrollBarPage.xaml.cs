// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class AnnotatedScrollBarPage : Page
{
    // Define the number of items present in each section of the source collection.
    private const int AzureCount = 32;
    private const int CrimsonCount = 50;
    private const int CyanCount = 8;
    private const int FuchsiaCount = 70;
    private const int GoldCount = 90;

    // Each item is sized 120x90 in the ItemsRepeater.
    private const int ItemWidth = 120;
    private const int ItemHeight = 90;

    // ItemsRepeater's ItemsSource.
    public ObservableCollection<SolidColorBrush> ColorCollection = new ObservableCollection<SolidColorBrush>();

    public AnnotatedScrollBarPage()
    {
        this.InitializeComponent();
        this.DataContext = this;
        this.Loaded += AnnotatedScrollBarPage_Loaded;

        PopulateColorCollection();
    }

    private void AnnotatedScrollBarPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Connect the ScrollView and AnnotatedScrollBar controls. The AnnotatedScrollBar provides
        // an IScrollController implementation, while the ScrollView consumes it.
        scrollView.ScrollPresenter.VerticalScrollController = annotatedScrollBar.ScrollController;
    }

    private void AnnotatedScrollBar_DetailLabelRequested(object sender, AnnotatedScrollBarDetailLabelRequestedEventArgs e)
    {
        // Provide a string as the tooltip content when hovering the mouse over the AnnotatedScrollBar's vertical rail. The string simply
        // represents the color of the last item in the row computed from AnnotatedScrollBarDetailLabelRequestedEventArgs.ScrollOffset.
        e.Content = GetOffsetLabel(e.ScrollOffset);
    }

    private void ItemsRepeater_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
    {
        // When the ItemsRepeater is resized, its items layout may change and thus require an update of
        // the AnnotatedScrollBar label positions.
        PopulateLabelCollection();
    }

    private void AnnotatedScrollBarMaxHeightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (annotatedScrollBar != null)
        {
            // Changing the height of the AnnotatedScrollBar to illustrate how labels
            // are hidden to avoid collisions when the available room shrinks too much.
            annotatedScrollBar.MaxHeight = (sender as Slider).Value;
        }
    }

    private void PopulateColorCollection()
    {
        SolidColorBrush solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Azure);

        for (int colorInstance = 0; colorInstance < AzureCount; colorInstance++)
        {
            this.ColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Crimson);

        for (int colorInstance = 0; colorInstance < CrimsonCount; colorInstance++)
        {
            this.ColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Cyan);

        for (int colorInstance = 0; colorInstance < CyanCount; colorInstance++)
        {
            this.ColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Fuchsia);

        for (int colorInstance = 0; colorInstance < FuchsiaCount; colorInstance++)
        {
            this.ColorCollection.Add(solidColorBrush);
        }

        solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Gold);

        for (int colorInstance = 0; colorInstance < GoldCount; colorInstance++)
        {
            this.ColorCollection.Add(solidColorBrush);
        }
    }

    private void PopulateLabelCollection()
    {
        if (annotatedScrollBar != null)
        {
            // Get rid of the labels that may have been defined earlier.
            annotatedScrollBar.Labels.Clear();

            // A new label is associated with the first item of each color section.
            // The offset value of a label is function of the row that item belongs to.
            annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Azure", GetOffsetOfItem(0)));
            annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Crimson", GetOffsetOfItem(AzureCount)));
            annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Cyan", GetOffsetOfItem(AzureCount + CrimsonCount)));
            annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Fuchsia", GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount)));
            annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Gold", GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount + FuchsiaCount)));
        }
    }

    private string GetOffsetLabel(double offset)
    {
        if (offset <= GetOffsetOfItem(AzureCount - 1))
        {
            return GetItemColor(AzureCount - 1);
        }
        else if (offset <= GetOffsetOfItem(AzureCount + CrimsonCount - 1))
        {
            return GetItemColor(AzureCount + CrimsonCount - 1);
        }
        else if (offset <= GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount - 1))
        {
            return GetItemColor(AzureCount + CrimsonCount + CyanCount - 1);
        }
        else if (offset <= GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount + FuchsiaCount - 1))
        {
            return GetItemColor(AzureCount + CrimsonCount + CyanCount + FuchsiaCount - 1);
        }
        else
        {
            return GetItemColor(AzureCount + CrimsonCount + CyanCount + FuchsiaCount);
        }
    }

    private int GetOffsetOfItem(int itemIndex)
    {
        return ItemHeight * (itemIndex / GetItemsPerRow());
    }

    private string GetItemColor(int itemIndex)
    {
        if (itemIndex < AzureCount)
        {
            return "Azure";
        }
        else if (itemIndex < AzureCount + CrimsonCount)
        {
            return "Crimson";
        }
        else if (itemIndex < AzureCount + CrimsonCount + CyanCount)
        {
            return "Cyan";
        }
        else if (itemIndex < AzureCount + CrimsonCount + CyanCount + FuchsiaCount)
        {
            return "Fuchsia";
        }
        else
        {
            return "Gold";
        }
    }

    private int GetItemsPerRow()
    {
        return (itemsRepeater == null || itemsRepeater.ActualWidth == 0) ? 1 : (int)Math.Max(itemsRepeater.ActualWidth / ItemWidth, 1);
    }
}
