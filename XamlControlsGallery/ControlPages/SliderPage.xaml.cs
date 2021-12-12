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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class SliderPage : Page
    {
        public SliderPage()
        {
            this.InitializeComponent();
        }


        private void MinimumValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            //Handle exceptional scenario
            if(args.NewValue >= Slider2.Maximum)
            {
                Slider2.Maximum = args.NewValue + 1;
            }
            Slider2.Minimum = args.NewValue;
        }

        private void MaximumValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            //Handle exceptional scenario
            if(args.NewValue <= Slider2.Minimum)
            {
                Slider2.Minimum = args.NewValue - 1;
                Slider2.Value = Slider2.Minimum;
            }
            Slider2.Maximum = args.NewValue;
        }

        private void StepFrequencyValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            try
            {
                Slider2.StepFrequency = (float)args.NewValue;
            }
            catch (Exception)
            {
                Slider2.StepFrequency = (Slider2.Maximum - Slider2.Minimum) / 100.0;
            }
        }

        private void SmallChangeValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            try
            {
                Slider2.SmallChange = (float)args.NewValue;
            }
            catch (Exception)
            {
                Slider2.SmallChange = (Slider2.Maximum - Slider2.Minimum) / 100.0;
            }
        }

        private void LargeChangeValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            try
            {
                Slider2.LargeChange = (float)args.NewValue;
            }
            catch (Exception)
            {
                Slider2.LargeChange = (Slider2.Maximum - Slider2.Minimum) / 10.0;
            }
        }
    }
}
