// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.ControlPages;

public sealed partial class RadioButtonsPage : Page
{
    public RadioButtonsPage()
    {
        this.InitializeComponent();
    }

    private void BackgroundColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ControlOutput != null && sender is RadioButtons rb)
        {
            string colorName = rb.SelectedItem as string;
            switch (colorName)
            {
                case "Yellow":
                    ControlOutput.Background = new SolidColorBrush(Colors.Yellow);
                    break;
                case "Green":
                    ControlOutput.Background = new SolidColorBrush(Colors.Green);
                    break;
                case "White":
                    ControlOutput.Background = new SolidColorBrush(Colors.White);
                    break;
            }
        }
    }

    private void BorderBrush_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ControlOutput != null && sender is RadioButtons rb)
        {
            string colorName = rb.SelectedItem as string;
            switch (colorName)
            {
                case "Yellow":
                    ControlOutput.BorderBrush = new SolidColorBrush(Colors.Gold);
                    break;
                case "Green":
                    ControlOutput.BorderBrush = new SolidColorBrush(Colors.DarkGreen);
                    break;
                case "White":
                    ControlOutput.BorderBrush = new SolidColorBrush(Colors.White);
                    break;
            }
        }
    }
}
