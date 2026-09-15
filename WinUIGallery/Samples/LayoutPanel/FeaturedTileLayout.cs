// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace WinUIGallery.Layouts;

public partial class FeaturedTileLayout : NonVirtualizingLayout
{
    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing),
        typeof(double),
        typeof(FeaturedTileLayout),
        new PropertyMetadata(0d, OnSpacingChanged));

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
    {
        double width = double.IsInfinity(availableSize.Width) ? 520 : availableSize.Width;
        double columnWidth = Math.Max(0, (width - Spacing) / 2);

        for (int index = 0; index < context.Children.Count; index++)
        {
            UIElement child = context.Children[index];
            child.Measure(new Size(index == 0 ? width : columnWidth, double.PositiveInfinity));
        }

        double totalHeight = context.Children.Count == 0 ? 0 : context.Children[0].DesiredSize.Height;

        for (int index = 1; index < context.Children.Count; index += 2)
        {
            totalHeight += Spacing + GetRowHeight(context, index);
        }

        return new Size(width, totalHeight);
    }

    protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
    {
        if (context.Children.Count == 0)
        {
            return finalSize;
        }

        double columnWidth = Math.Max(0, (finalSize.Width - Spacing) / 2);
        UIElement featuredItem = context.Children[0];
        featuredItem.Arrange(new Rect(0, 0, finalSize.Width, featuredItem.DesiredSize.Height));
        double y = featuredItem.DesiredSize.Height;

        for (int index = 1; index < context.Children.Count; index += 2)
        {
            y += Spacing;
            double rowHeight = GetRowHeight(context, index);
            context.Children[index].Arrange(new Rect(0, y, columnWidth, rowHeight));

            if (index + 1 < context.Children.Count)
            {
                context.Children[index + 1].Arrange(new Rect(columnWidth + Spacing, y, columnWidth, rowHeight));
            }

            y += rowHeight;
        }

        return new Size(finalSize.Width, y);
    }

    private static double GetRowHeight(NonVirtualizingLayoutContext context, int firstIndex)
    {
        double height = context.Children[firstIndex].DesiredSize.Height;
        return firstIndex + 1 < context.Children.Count
            ? Math.Max(height, context.Children[firstIndex + 1].DesiredSize.Height)
            : height;
    }

    private static void OnSpacingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is FeaturedTileLayout layout)
        {
            layout.InvalidateMeasure();
        }
    }
}
