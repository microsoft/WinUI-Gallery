// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace WinUIGallery.Helpers;
public static class PageScrollBehaviorHelper
{
    public static readonly DependencyProperty SuppressHostScrollingProperty =
        DependencyProperty.RegisterAttached(
            "SuppressHostScrolling",
            typeof(bool),
            typeof(PageScrollBehaviorHelper),
            new PropertyMetadata(false));

    public static void SetSuppressHostScrolling(DependencyObject element, bool value)
    {
        element.SetValue(SuppressHostScrollingProperty, value);
    }

    public static bool GetSuppressHostScrolling(DependencyObject element)
    {
        return (bool)element.GetValue(SuppressHostScrollingProperty);
    }
}
