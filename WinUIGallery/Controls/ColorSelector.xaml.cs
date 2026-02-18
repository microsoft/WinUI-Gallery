// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace WinUIGallery.Controls;

public sealed partial class ColorSelector : UserControl
{
    public ColorSelector()
    {
        InitializeComponent();
        UpdateVisual(Color);
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(ColorSelector),
            new PropertyMetadata(Colors.Transparent, OnColorPropertyChanged));

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private static void OnColorPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorSelector)d;
        var newColor = (Color)e.NewValue;

        control.UpdateVisual(newColor);
        control.ColorChanged?.Invoke(control);
    }

    public event Action<ColorSelector>? ColorChanged;

    private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (Color != args.NewColor)
        {
            Color = args.NewColor;
        }
    }

    private void UpdateVisual(Color color)
    {
        CurrentColor.Background = new SolidColorBrush(color);

        if (ColorPicker.Color != color)
        {
            ColorPicker.Color = color;
        }
    }
}
