// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.DesktopWap.Controls
{
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

        public static readonly DependencyProperty SizeLinHeightProperty = DependencyProperty.Register(nameof(SizeLinHeight), typeof(string), typeof(TypographyControl), new PropertyMetadata(""));

        public string SizeLinHeight
        {
            get => (string)GetValue(SizeLinHeightProperty);
            set => SetValue(SizeLinHeightProperty, value);
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

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CopyToClipboardButton.Visibility = Visibility.Visible;
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CopyToClipboardButton.Visibility = Visibility.Collapsed;
        }

        private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new DataPackage();
            package.SetText(ResourceName);
            Clipboard.SetContent(package);
        }
    }
}
