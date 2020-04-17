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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && NumberBoxSpinButtonPlacementExample != null)
            {
                string spinButtonPlacementModeName = rb.Tag.ToString();

                switch (spinButtonPlacementModeName)
                {
                    case "Inline":
                        NumberBoxSpinButtonPlacementExample.SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline;
                        break;
                    case "Compact":
                        NumberBoxSpinButtonPlacementExample.SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact;
                        break;
                }
            }
        }
    }
}
