using System;
using AppUIBasics;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.DesktopWap.DesignGuidancePages
{
    public sealed partial class AccessibilityColorContrastPage : ItemsPageBase
    {
        public AccessibilityColorContrastPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
            args.NavigationRootPage.NavigationView.Header = string.Empty;
        }

        // Text color
        private void TextColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            TextColorPreview.Fill = new SolidColorBrush(args.NewColor);
            TextColorHex.Text = args.NewColor.ToString().Replace("#FF", "#");
        }

        private void TextColorPickerFlyout_Opened(object sender, object e)
        {
            TextColorPicker.Color = ((SolidColorBrush)TextColorPreview.Fill).Color;
        }

        private void TextColorHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextColorPreview.Fill = GetSolidColorBrush(TextColorHex.Text);
                RecalculateContrastRatio();
            }
            catch (Exception) { }
        }

        // Background color
        private void BackgroundColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            BackgroundColorPreview.Fill = new SolidColorBrush(args.NewColor);
            BackgroundColorHex.Text = args.NewColor.ToString().Replace("#FF", "#");
        }

        private void BackgroundColorPickerFlyout_Opened(object sender, object e)
        {
            BackgroundColorPicker.Color = ((SolidColorBrush)BackgroundColorPreview.Fill).Color;
        }

        private void BackgroundColorHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                BackgroundColorPreview.Fill = GetSolidColorBrush(BackgroundColorHex.Text);
                RecalculateContrastRatio();
            }
            catch (Exception) { }
        }

        private void RecalculateContrastRatio()
        {
            var textColor = ((SolidColorBrush)TextColorPreview.Fill).Color;
            var backgroundColor = ((SolidColorBrush)BackgroundColorPreview.Fill).Color;

            var ratio = CalculateContrastRatio(textColor, backgroundColor);
            ContrastRatioPresenter.Text = Math.Round(ratio, 2).ToString() + ":1";

            SetCheckState(NormalTextCheckEllipse, NormalTextCheckIcon, NormalTextCheckResult, ratio >= 4.5);
            SetCheckState(LargeTextCheckEllipse, LargeTextCheckIcon, LargeTextCheckResult, ratio >= 3.0);
            SetCheckState(ComponentsCheckEllipse, ComponentsCheckIcon, ComponentsCheckResult, ratio >= 3.0);
        }

        private void SetCheckState(Ellipse background, FontIcon icon, TextBlock resultName, bool passed)
        {
            if (passed)
            {
                background.Fill = new SolidColorBrush(Colors.DarkGreen);
                icon.Glyph = "\uE73E";
                resultName.Text = "Pass";
            }
            else
            {
                background.Fill = new SolidColorBrush(Colors.DarkRed);
                icon.Glyph = "\uE711";
                resultName.Text = "Fail";
            }
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

        // Find the contrast ratio: https://www.w3.org/WAI/GL/wiki/Contrast_ratio
        public static double CalculateContrastRatio(Color first, Color second)
        {
            var relLuminanceOne = GetRelativeLuminance(first);
            var relLuminanceTwo = GetRelativeLuminance(second);
            return (Math.Max(relLuminanceOne, relLuminanceTwo) + 0.05)
                / (Math.Min(relLuminanceOne, relLuminanceTwo) + 0.05);
        }

        // Get relative luminance: https://www.w3.org/WAI/GL/wiki/Relative_luminance
        public static double GetRelativeLuminance(Color c)
        {
            var rSRGB = c.R / 255.0;
            var gSRGB = c.G / 255.0;
            var bSRGB = c.B / 255.0;

            var r = rSRGB <= 0.04045 ? rSRGB / 12.92 : Math.Pow(((rSRGB + 0.055) / 1.055), 2.4);
            var g = gSRGB <= 0.04045 ? gSRGB / 12.92 : Math.Pow(((gSRGB + 0.055) / 1.055), 2.4);
            var b = bSRGB <= 0.04045 ? bSRGB / 12.92 : Math.Pow(((bSRGB + 0.055) / 1.055), 2.4);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }
    }
}
