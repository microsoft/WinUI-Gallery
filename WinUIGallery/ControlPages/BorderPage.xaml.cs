//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class BorderPage : Page
    {
        public BorderPage()
        {
            this.InitializeComponent();
        }

        private void ThicknessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Control1 != null) Control1.BorderThickness = new Thickness(e.NewValue);
        }

        private void BGRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && Control1 != null)
            {
                string colorName = rb.Content.ToString();
                switch (colorName)
                {
                    case "Yellow":
                        Control1.Background = new SolidColorBrush(Colors.Yellow);
                        break;
                    case "Green":
                        Control1.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "Blue":
                        Control1.Background = new SolidColorBrush(Colors.Blue);
                        break;
                    case "White":
                        Control1.Background = new SolidColorBrush(Colors.White);
                        break;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && Control1 != null)
            {
                string colorName = rb.Content.ToString();
                switch (colorName)
                {
                    case "Yellow":
                        Control1.BorderBrush = new SolidColorBrush(Colors.Gold);
                        break;
                    case "Green":
                        Control1.BorderBrush = new SolidColorBrush(Colors.DarkGreen);
                        break;
                    case "Blue":
                        Control1.BorderBrush = new SolidColorBrush(Colors.DarkBlue);
                        break;
                    case "White":
                        Control1.BorderBrush = new SolidColorBrush(Colors.White);
                        break;
                }
            }
        }
    }
}
