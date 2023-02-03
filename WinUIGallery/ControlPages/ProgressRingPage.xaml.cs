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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ProgressRingPage : Page
    {
        public ProgressRingPage()
        {
            this.InitializeComponent();
        }

        private void ProgressValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            if (!sender.Value.IsNaN())
            {
                ProgressRing2.Value = sender.Value;
            }
            else
            {
                sender.Value = 0;
            }
        }

        private void Background_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var progressRing = (ComboBox)sender == BackgroundComboBox1 ? ProgressRing1 : ProgressRing2;
            var revealBackgroundProperty = (ComboBox)sender == BackgroundComboBox1 ? RevealBackgroundProperty1 : RevealBackgroundProperty2;
            string colorName = e.AddedItems[0].ToString();
            bool showBackgroundProperty = false;
            switch (colorName)
            {
                case "Transparent":
                    progressRing.Background = new SolidColorBrush(Colors.Transparent);
                    break;
                case "LightGray":
                    progressRing.Background = new SolidColorBrush(Colors.LightGray);
                    showBackgroundProperty = true;
                    break;

                default:
                    throw new Exception($"Invalid argument: {colorName}");
            }
            revealBackgroundProperty.IsEnabled = showBackgroundProperty;
        }

    }
}
