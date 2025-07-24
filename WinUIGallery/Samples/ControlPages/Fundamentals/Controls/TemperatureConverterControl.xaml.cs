// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Samples.ControlPages.Fundamentals.Controls;

public sealed partial class TemperatureConverterControl : UserControl
{
    public TemperatureConverterControl()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string input = InputTextBox.Text;
        double celsius = 0;

        bool isNumber = double.TryParse(input, out celsius);

        if (isNumber)
        {
            double fahrenheit = (celsius * 9 / 5) + 32;
            ResultTextBlock.Text = "Fahrenheit: " + fahrenheit.ToString("F2") + "°F";
        }
        else
        {
            ResultTextBlock.Text = "Invalid input!";
        }
    }
}
