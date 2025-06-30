// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class ViewboxPage : Page
{
    public ViewboxPage()
    {
        this.InitializeComponent();
    }

    private void StretchDirectionButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && Control1 != null)
        {
            string direction = rb.Tag.ToString();
            switch (direction)
            {
                case "UpOnly":
                    Control1.StretchDirection = StretchDirection.UpOnly;
                    break;

                case "DownOnly":
                    Control1.StretchDirection = StretchDirection.DownOnly;
                    break;

                case "Both":
                    Control1.StretchDirection = StretchDirection.Both;
                    break;
            }
        }
    }

    private void StretchButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && Control1 != null)
        {
            string stretch = rb.Tag.ToString();
            switch (stretch)
            {
                case "None":
                    Control1.Stretch = Stretch.None;
                    break;

                case "Fill":
                    Control1.Stretch = Stretch.Fill;
                    break;

                case "Uniform":
                    Control1.Stretch = Stretch.Uniform;
                    break;

                case "UniformToFill":
                    Control1.Stretch = Stretch.UniformToFill;
                    break;
            }
        }
    }

    private void Stretch_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as RadioButtons)?.SelectedItem is not RadioButton selectedItem ||
            Enum.TryParse<Stretch>(selectedItem.Tag?.ToString(), out var stretch) is false ||
            Control1 is null)
        {
            return;
        }

        Control1.Stretch = stretch;
    }

    private void StretchDirection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as RadioButtons)?.SelectedItem is not RadioButton selectedItem ||
            Enum.TryParse<StretchDirection>(selectedItem.Tag?.ToString(), out var stretchDirection) is false ||
            Control1 is null)
        {
            return;
        }

        Control1.StretchDirection = stretchDirection;
    }
}
