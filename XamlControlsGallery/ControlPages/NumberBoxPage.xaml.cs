//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************C:\Users\saschule\Source\Repos\Xaml-Controls-Gallery\XamlControlsGallery\Assets\InkCanvas.png
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 0.25;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;

            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
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