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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using System.Numerics;

namespace AppUIBasics.ControlPages
{
    public sealed partial class RadialGradientBrushPage : Page
    {
        public RadialGradientBrushPage()
        {
            this.InitializeComponent();
            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MappingModeComboBox.SelectionChanged += OnMappingModeChanged;
            SpreadMethodComboBox.SelectionChanged += OnSpreadMethodChanged;
            InitializeSliders();
        }

        private void OnSpreadMethodChanged(object sender, SelectionChangedEventArgs e)
        {
            RadialGradientBrushExample.SpreadMethod = Enum.Parse<GradientSpreadMethod>(SpreadMethodComboBox.SelectedValue.ToString());
        }

        private void OnMappingModeChanged(object sender, SelectionChangedEventArgs e)
        {
            RadialGradientBrushExample.MappingMode = Enum.Parse<BrushMappingMode>(MappingModeComboBox.SelectedValue.ToString());
            InitializeSliders();
        }

        private void InitializeSliders()
        {
            var rectSize = Rect.ActualSize.ToSize();
            if (RadialGradientBrushExample.MappingMode == BrushMappingMode.Absolute)
            {
                CenterXSlider.Maximum = RadiusXSlider.Maximum = OriginXSlider.Maximum = rectSize.Width;
                CenterYSlider.Maximum = RadiusYSlider.Maximum = OriginYSlider.Maximum = rectSize.Width;
                CenterXSlider.Value = RadiusXSlider.Value = OriginXSlider.Value = rectSize.Width / 2;
                CenterYSlider.Value = RadiusYSlider.Value = OriginYSlider.Value = rectSize.Width / 2;
                CenterXSlider.StepFrequency = RadiusXSlider.StepFrequency = OriginXSlider.StepFrequency = rectSize.Width / 50;
                CenterYSlider.StepFrequency = RadiusYSlider.StepFrequency = OriginYSlider.StepFrequency = rectSize.Height / 50;
                CenterXSlider.SmallChange = RadiusXSlider.SmallChange = OriginXSlider.SmallChange = 10;
                CenterYSlider.SmallChange = RadiusYSlider.SmallChange = OriginYSlider.SmallChange = 10;
            }
            else
            {
                CenterXSlider.Maximum = RadiusXSlider.Maximum = OriginXSlider.Maximum = 1.0;
                CenterYSlider.Maximum = RadiusYSlider.Maximum = OriginYSlider.Maximum = 1.0;
                CenterXSlider.Value = RadiusXSlider.Value = OriginXSlider.Value = 0.5;
                CenterYSlider.Value = RadiusYSlider.Value = OriginYSlider.Value = 0.5;
                CenterXSlider.StepFrequency = RadiusXSlider.StepFrequency = OriginXSlider.StepFrequency = 0.02;
                CenterYSlider.StepFrequency = RadiusYSlider.StepFrequency = OriginYSlider.StepFrequency = 0.02;
            }
        }

        private void OnSliderValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            RadialGradientBrushExample.Center = new Point(CenterXSlider.Value, CenterYSlider.Value);
            RadialGradientBrushExample.RadiusX = RadiusXSlider.Value;
            RadialGradientBrushExample.RadiusY = RadiusYSlider.Value;
            RadialGradientBrushExample.GradientOrigin = new Point(OriginXSlider.Value, OriginYSlider.Value);
        }
    }
}
