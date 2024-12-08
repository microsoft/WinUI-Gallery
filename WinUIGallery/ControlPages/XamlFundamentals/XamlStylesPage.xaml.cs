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
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace WinUIGallery.ControlPages
{
    public sealed partial class XamlStylesPage : Page
    {
        public XamlStylesPage()
        {
            this.InitializeComponent();

            ApplyButtonStyle(MyButton);
        }

        private static void ApplyButtonStyle(Button myButton)
        {
            var buttonStyle = new Style(typeof(Button));
            buttonStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Microsoft.UI.Colors.LightGreen)));
            buttonStyle.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(Microsoft.UI.Colors.Black)));
            buttonStyle.Setters.Add(new Setter(FontSizeProperty, 20));

            myButton.Style = buttonStyle;
        }
    }
}
