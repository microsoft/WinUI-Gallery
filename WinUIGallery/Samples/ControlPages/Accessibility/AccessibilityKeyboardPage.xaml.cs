// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.ControlPages;

public sealed partial class AccessibilityKeyboardPage : Page
{
    public AccessibilityKeyboardPage()
    {
        this.InitializeComponent();
    }

    private void MakeRedButton_Click(object sender, RoutedEventArgs e)
    {
        ColorRectangle.Fill = new SolidColorBrush(Colors.Red);
    }
    private void MakeBlueButton_Click(object sender, RoutedEventArgs e)
    {
        ColorRectangle.Fill = new SolidColorBrush(Colors.Blue);
    }
    private void MakeChartreuseButton_Click(object sender, RoutedEventArgs e)
    {
        ColorRectangle.Fill = new SolidColorBrush(Colors.Chartreuse);
    }
}
