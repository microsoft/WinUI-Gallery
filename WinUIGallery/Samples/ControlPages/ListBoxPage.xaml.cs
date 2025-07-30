// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.ControlPages;

public sealed partial class ListBoxPage : Page
{
    public ListBoxPage()
    {
        this.InitializeComponent();
    }

    private void ColorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string colorName = e.AddedItems[0].ToString();
        switch (colorName)
        {
            case "Yellow":
                Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
                break;
            case "Green":
                Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Green);
                break;
            case "Blue":
                Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Blue);
                break;
            case "Red":
                Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Red);
                break;
        }
    }
}
