// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace WinUIGallery.Helpers;

/// <summary>
/// Provides an attached property to indicate whether a page manages its own scrolling.
/// This can be used by a hosting container (like ItemsPage) to disable page level scrolling.
/// </summary>
public static partial class PageScrollBehaviorHelper
{
    /// <summary>
    /// Identifies the IsSelfScrolling attached property.
    /// When set to true, the page is expected to manage its own scroll behavior.
    /// </summary>
    public static readonly DependencyProperty SuppressHostScrollingProperty =
        DependencyProperty.RegisterAttached(
            "SuppressHostScrolling",
            typeof(bool),
            typeof(PageScrollBehaviorHelper),
            new PropertyMetadata(false));

    /// <summary>
    /// Sets the IsSelfScrolling attached property on the specified element.
    /// </summary>
    /// <param name="element">The element to set the property on.</param>
    /// <param name="value">True if the element handles its own scrolling; otherwise, false.</param>

    public static void SetSuppressHostScrolling(DependencyObject element, bool value)
    {
        element.SetValue(SuppressHostScrollingProperty, value);
    }

    /// <summary>
    /// Gets the value of the IsSelfScrolling attached property from the specified element.
    /// </summary>
    /// <param name="element">The element to read the property from.</param>
    /// <returns>True if the element handles its own scrolling; otherwise, false.</returns>

    public static bool GetSuppressHostScrolling(DependencyObject element)
    {
        return (bool)element.GetValue(SuppressHostScrollingProperty);
    }
}
