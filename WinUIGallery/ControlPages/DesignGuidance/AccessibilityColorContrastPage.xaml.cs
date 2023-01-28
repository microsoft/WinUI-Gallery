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
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AccessibilityColorContrastPage : Page
    {
        public AccessibilityColorContrastPage()
        {
            this.InitializeComponent();
        }

        private void RecalculateContrastRatio()
        {
            var textColor = TextColorPicker.Color;
            var backgroundColor = BackgroundColorPicker.Color;

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

        public static SolidColorBrush GetSolidColorBrush(string hex)
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

        private void BackgroundColorPicker_ColorChanged(object sender, Color e)
        {
            RecalculateContrastRatio();
        }

        private void TextColorPicker_ColorChanged(object sender, Color e)
        {
            RecalculateContrastRatio();
        }
    }
}
