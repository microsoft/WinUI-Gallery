// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Windows.Globalization.NumberFormatting;

namespace WinUIGallery.ControlPages;

public sealed partial class NumberBoxPage : Page
{
    public NumberBoxPage()
    {
        this.InitializeComponent();
        SetNumberBoxNumberFormatter();
    }

    private void SetNumberBoxNumberFormatter()
    {
        IncrementNumberRounder rounder = new IncrementNumberRounder
        {
            Increment = 0.25,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };

        DecimalFormatter formatter = new DecimalFormatter
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
