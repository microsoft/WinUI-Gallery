// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace WinUIGallery.Controls;

public sealed partial class TypographyControl : UserControl
{
    public TypographyControl()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ExampleProperty = DependencyProperty.Register(nameof(Example), typeof(string), typeof(TypographyControl), new PropertyMetadata(""));

    public string Example
    {
        get => (string)GetValue(ExampleProperty);
        set => SetValue(ExampleProperty, value);
    }

    public static readonly DependencyProperty WeightProperty = DependencyProperty.Register(nameof(Weight), typeof(string), typeof(TypographyControl), new PropertyMetadata(""));

    public string Weight
    {
        get => (string)GetValue(WeightProperty);
        set => SetValue(WeightProperty, value);
    }

    public static readonly DependencyProperty VariableFontProperty = DependencyProperty.Register(nameof(VariableFont), typeof(string), typeof(TypographyControl), new PropertyMetadata(""));

    public string VariableFont
    {
        get => (string)GetValue(VariableFontProperty);
        set => SetValue(VariableFontProperty, value);
    }

    public static readonly DependencyProperty SizeLineHeightProperty = DependencyProperty.Register(nameof(SizeLineHeight), typeof(string), typeof(TypographyControl), new PropertyMetadata(""));

    public string SizeLineHeight
    {
        get => (string)GetValue(SizeLineHeightProperty);
        set => SetValue(SizeLineHeightProperty, value);
    }

    public static readonly DependencyProperty ExampleStyleProperty = DependencyProperty.Register(nameof(ExampleStyleProperty), typeof(Style), typeof(TypographyControl), new PropertyMetadata(null));

    public Style ExampleStyle
    {
        get => (Style)GetValue(ExampleStyleProperty);
        set => SetValue(ExampleStyleProperty, value);
    }

    public static readonly DependencyProperty ResourceNameProperty = DependencyProperty.Register(nameof(ExampleStyleProperty), typeof(string), typeof(TypographyControl), new PropertyMetadata(null));

    public string ResourceName
    {
        get => (string)GetValue(ResourceNameProperty);
        set => SetValue(ResourceNameProperty, value);
    }

    private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
    {
        DataPackage package = new DataPackage();
        package.SetText(ResourceName);
        Clipboard.SetContent(package);
    }
}
