using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.DesktopWap.Controls
{
    public sealed partial class InlineColorPicker : UserControl
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set {
                ColorBrush = new SolidColorBrush(value);
                SetValue(ColorProperty, value);
            }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(InlineColorPicker), new PropertyMetadata(Colors.White));

        public SolidColorBrush ColorBrush
        {
            get { return (SolidColorBrush)GetValue(ColorBrushProperty); }
            set { SetValue(ColorBrushProperty, value); }
        }
        public static readonly DependencyProperty ColorBrushProperty =
            DependencyProperty.Register("ColorBrush", typeof(SolidColorBrush), typeof(InlineColorPicker), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        
        public event EventHandler<Color> ColorChanged;

        public InlineColorPicker()
        {
            this.InitializeComponent();
            this.Loaded += InlineColorPicker_Loaded;
        }

        private void InlineColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            ColorHex.Text = Color.ToString().Replace("#FF","#");
        }

        public SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(255, r, g, b));
            return myBrush;
        }

        private void Picker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            ColorPreview.Fill = new SolidColorBrush(args.NewColor);
            ColorHex.Text = args.NewColor.ToString().Replace("#FF", "#");
            Color = args.NewColor;
            ColorChanged?.Invoke(this, args.NewColor);
        }

        private void PickerFlyout_Opened(object sender, object e)
        {
            Picker.Color = ((SolidColorBrush)ColorPreview.Fill).Color;
        }

        private void ColorHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ColorPreview.Fill = GetSolidColorBrush(ColorHex.Text);
                Color = ((SolidColorBrush)ColorPreview.Fill).Color;
                ColorChanged?.Invoke(this, Color);
            }
            catch (Exception) { }
        }
    }
}
