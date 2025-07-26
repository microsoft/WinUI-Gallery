// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;

namespace WinUIGallery.ControlPages;

public sealed partial class ComboBoxPage : Page
{
    public List<double> FontSizes { get; } = new List<double>()
        {
            8,
            9,
            10,
            11,
            12,
            14,
            16,
            18,
            20,
            24,
            28,
            36,
            48,
            72
        };

    public ComboBoxPage()
    {
        this.InitializeComponent();
    }

    private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string colorName = e.AddedItems[0].ToString();
        Windows.UI.Color color;
        switch (colorName)
        {
            case "Yellow":
                color = Colors.Yellow;
                break;
            case "Green":
                color = Colors.Green;
                break;
            case "Blue":
                color = Colors.Blue;
                break;
            case "Red":
                color = Colors.Red;
                break;
            default:
                throw new Exception($"Invalid argument: {colorName}");
        }
        Control1Output.Fill = new SolidColorBrush(color);
    }

    private void Combo3_Loaded(object sender, RoutedEventArgs e)
    {
        Combo3.SelectedIndex = 2;

        if ((ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7)))
        {
            Combo3.TextSubmitted += Combo3_TextSubmitted;
        }
    }

    private void Combo3_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
    {
        bool isDouble = double.TryParse(sender.Text, out double newValue);

        // Set the selected item if:
        // - The value successfully parsed to double AND
        // - The value is in the list of sizes OR is a custom value between 8 and 100
        if (isDouble && (FontSizes.Contains(newValue) || (newValue < 100 && newValue > 8)))
        {
            // Update the SelectedItem to the new value. 
            sender.SelectedItem = newValue;
        }
        else
        {
            // If the item is invalid, reject it and revert the text. 
            sender.Text = sender.SelectedValue.ToString();

            var dialog = new ContentDialog();
            dialog.Content = "The font size must be a number between 8 and 100.";
            dialog.CloseButtonText = "Close";
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.XamlRoot = sender.XamlRoot;
            _ = dialog.ShowAsync();
        }

        // Mark the event as handled so the framework doesn’t update the selected item automatically. 
        args.Handled = true;
    }
}
