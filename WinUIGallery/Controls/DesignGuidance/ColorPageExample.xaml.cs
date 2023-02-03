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
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.DesktopWap.Controls.DesignGuidance
{
    [ContentProperty(Name = "ExampleContent")]
    public sealed partial class ColorPageExample : UserControl
    {

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ColorPageExample), new PropertyMetadata(""));

        public UIElement ExampleContent
        {
            get { return (UIElement)GetValue(ExampleContentProperty); }
            set { SetValue(ExampleContentProperty, value); }
        }
        public static readonly DependencyProperty ExampleContentProperty =
            DependencyProperty.Register("ExampleContent", typeof(UIElement), typeof(ColorPageExample), new PropertyMetadata(null));

        public ColorPageExample()
        {
            this.InitializeComponent();
        }
    }
}
