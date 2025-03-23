//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Windows.Globalization.NumberFormatting;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class NumberBoxPage : Page
{
    public NumberBoxPage()
    {
        InitializeComponent();
        SetNumberBoxNumberFormatter();
    }

    private void SetNumberBoxNumberFormatter()
    {
        IncrementNumberRounder rounder = new()
        {
            Increment = 0.25,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };

        DecimalFormatter formatter = new()
        {
            IntegerDigits = 1,
            FractionDigits = 2,
            NumberRounder = rounder
        };
        FormattedNumberBox.NumberFormatter = formatter;
    }

    private void SpinButtonPlacementGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is RadioButtons radioButtons)
        {
            switch (radioButtons.SelectedIndex)
            {
                case 0:
                    NumberBoxSpinButtonPlacementExample.SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline;
                    break;
                case 1:
                    NumberBoxSpinButtonPlacementExample.SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact;
                    break;
            }
        }
    }
}
