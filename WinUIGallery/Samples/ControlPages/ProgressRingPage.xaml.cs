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

namespace WinUIGallery.ControlPages;

public sealed partial class ProgressRingPage : Page
{
    public ProgressRingPage()
    {
        InitializeComponent();
    }

    private void ProgressValue_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
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
