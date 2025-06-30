// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class ProgressRingPage : Page
{
    public ProgressRingPage()
    {
        this.InitializeComponent();
    }

    private void ProgressValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
    {
        if (!double.IsNaN(sender.Value))
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
