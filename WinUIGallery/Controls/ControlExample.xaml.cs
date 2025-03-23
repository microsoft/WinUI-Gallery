//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace WinUIGallery.Controls;

/// <summary>
/// Describes a textual substitution in sample content.
/// If enabled (default), then $(Key) is replaced with the stringified value.
/// If disabled, then $(Key) is replaced with the empty string.
/// </summary>
public sealed class ControlExampleSubstitution : DependencyObject
{
    public event TypedEventHandler<ControlExampleSubstitution, object> ValueChanged;

    public string Key { get; set; }

    private object _value = null;
    public object Value
    {
        get { return _value; }
        set
        {
            _value = value;
            ValueChanged?.Invoke(this, null);
        }
    }

    private bool _enabled = true;
    public bool IsEnabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            ValueChanged?.Invoke(this, null);
        }
    }

    public string ValueAsString()
    {
        if (!IsEnabled)
        {
            return string.Empty;
        }

        object value = Value;

        // For solid color brushes, use the underlying color.
        if (value is SolidColorBrush brush)
        {
            value = brush.Color;
        }

        return value == null ? string.Empty : value.ToString();
    }
}

[ContentProperty(Name = "Example")]
public sealed partial class ControlExample : UserControl
{
    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
    public string HeaderText
    {
        get { return (string)GetValue(HeaderTextProperty); }
        set {
            SetValue(HeaderTextProperty, value);
            HeaderTextPresenter.Visibility = string.IsNullOrEmpty(HeaderText) ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public static readonly DependencyProperty ExampleProperty = DependencyProperty.Register("Example", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
    public object Example
    {
        get { return GetValue(ExampleProperty); }
        set { SetValue(ExampleProperty, value); }
    }

    public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
    public object Output
    {
        get { return GetValue(OutputProperty); }
        set { SetValue(OutputProperty, value); }
    }

    public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register("Options", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
    public object Options
    {
        get { return GetValue(OptionsProperty); }
        set { SetValue(OptionsProperty, value); }
    }

    public static readonly DependencyProperty XamlProperty = DependencyProperty.Register("Xaml", typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnXamlChanged));
    public string Xaml
    {
        get { return (string)GetValue(XamlProperty); }
        set { SetValue(XamlProperty, value); }
    }

    public static readonly DependencyProperty XamlSourceProperty = DependencyProperty.Register("XamlSource", typeof(object), typeof(ControlExample), new PropertyMetadata(null, OnXamlChanged));
    public string XamlSource
    {
        get { return (string)GetValue(XamlSourceProperty); }
        set { SetValue(XamlSourceProperty, value); }
    }

    public static readonly DependencyProperty CSharpProperty = DependencyProperty.Register("CSharp", typeof(string), typeof(ControlExample), new PropertyMetadata(null, OnCSharpChanged));
    public string CSharp
    {
        get { return (string)GetValue(CSharpProperty); }
        set { SetValue(CSharpProperty, value); }
    }

    public static readonly DependencyProperty CSharpSourceProperty = DependencyProperty.Register("CSharpSource", typeof(object), typeof(ControlExample), new PropertyMetadata(null, OnCSharpChanged));
    public string CSharpSource
    {
        get { return (string)GetValue(CSharpSourceProperty); }
        set { SetValue(CSharpSourceProperty, value); }
    }

    public static readonly DependencyProperty SubstitutionsProperty = DependencyProperty.Register("Substitutions", typeof(IList<ControlExampleSubstitution>), typeof(ControlExample), new PropertyMetadata(null));
    public IList<ControlExampleSubstitution> Substitutions
    {
        get { return (IList<ControlExampleSubstitution>)GetValue(SubstitutionsProperty); }
        set { SetValue(SubstitutionsProperty, value); }
    }

    private static readonly GridLength defaultExampleHeight =
        new(1, GridUnitType.Star);

    public static readonly DependencyProperty ExampleHeightProperty = DependencyProperty.Register("ExampleHeight", typeof(GridLength), typeof(ControlExample), new PropertyMetadata(defaultExampleHeight));
    public GridLength ExampleHeight
    {
        get { return (GridLength)GetValue(ExampleHeightProperty); }
        set { SetValue(ExampleHeightProperty, value); }
    }

    public static readonly DependencyProperty WebViewHeightProperty = DependencyProperty.Register("WebViewHeight", typeof(int), typeof(ControlExample), new PropertyMetadata(400));
    public int WebViewHeight
    {
        get { return (int)GetValue(WebViewHeightProperty); }
        set { SetValue(WebViewHeightProperty, value); }
    }

    public static readonly DependencyProperty WebViewWidthProperty = DependencyProperty.Register("WebViewWidth", typeof(int), typeof(ControlExample), new PropertyMetadata(800));
    public int WebViewWidth
    {
        get { return (int)GetValue(WebViewWidthProperty); }
        set { SetValue(WebViewWidthProperty, value); }
    }

    public new static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ControlExample), new PropertyMetadata(HorizontalAlignment.Left));
    public new HorizontalAlignment HorizontalContentAlignment
    {
        get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
        set { SetValue(HorizontalContentAlignmentProperty, value); }
    }

    public static readonly DependencyProperty SourceCodeVisibilityProperty = DependencyProperty.Register("SourceCodeVisibility", typeof(Visibility), typeof(ControlExample), new PropertyMetadata(Visibility.Visible));
    public Visibility SourceCodeVisibility
    {
        get { return (Visibility)GetValue(SourceCodeVisibilityProperty); }
        set { SetValue(SourceCodeVisibilityProperty, value); }
    }

    public ControlExample()
    {
        InitializeComponent();
        Substitutions = [];
        Loaded += ControlExample_Loaded;
    }

    private void ControlExample_Loaded(object sender, RoutedEventArgs e) => HeaderTextPresenter.Visibility = string.IsNullOrEmpty(HeaderText) ? Visibility.Collapsed : Visibility.Visible;

    private enum SyntaxHighlightLanguage { Xml, CSharp };

    private void SelectorBarItem_Loaded(object sender, RoutedEventArgs e)
    {
        var item = sender as SelectorBarItem;
        if (item == null)
            return;

        if (item.Tag.ToString().Equals("Xaml", StringComparison.OrdinalIgnoreCase))
        {
            item.Visibility = string.IsNullOrEmpty(Xaml) && string.IsNullOrEmpty(XamlSource) ? Visibility.Collapsed : Visibility.Visible;
        }
        else if (item.Tag.ToString().Equals("CSharp", StringComparison.OrdinalIgnoreCase))
        {
            item.Visibility = string.IsNullOrEmpty(CSharp) && string.IsNullOrEmpty(CSharpSource) ? Visibility.Collapsed : Visibility.Visible;
        }

        var firstVisibileItem = SelectorBarControl.Items.Where(x => x.Visibility == Visibility.Visible).FirstOrDefault();
        if (firstVisibileItem != null)
        {
            firstVisibileItem.IsSelected = true;
        }

        HandlePresenterVisibility();
    }

    private static void OnXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ControlExample)d;
        ctrl?.SelectorBarItem_Loaded(ctrl.SelectorBarXamlItem, null);
    }
    private static void OnCSharpChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ControlExample)d;
        ctrl?.SelectorBarItem_Loaded(ctrl.SelectorBarCSharpItem, null);
    }

    private void SelectorBarControl_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args) => HandlePresenterVisibility();

    private void HandlePresenterVisibility()
    {
        var selectedItem = SelectorBarControl.SelectedItem;
        if (selectedItem != null)
        {
            if (selectedItem.Tag.ToString().Equals("Xaml", StringComparison.OrdinalIgnoreCase))
            {
                XamlContentPresenter.Visibility = Visibility.Visible;
                CSharpContentPresenter.Visibility = Visibility.Collapsed;
            }
            else if (selectedItem.Tag.ToString().Equals("CSharp", StringComparison.OrdinalIgnoreCase))
            {
                CSharpContentPresenter.Visibility = Visibility.Visible;
                XamlContentPresenter.Visibility = Visibility.Collapsed;
            }
        }
    }
}
