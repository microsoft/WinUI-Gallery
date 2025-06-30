// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace WinUIGallery.ControlPages;

public sealed partial class SliderPage : Page
{
    public SliderPage()
    {
        this.InitializeComponent();
    }

    private void SnapsToRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        switch (SnapsToRadioButtons.SelectedItem)
        {
            case "StepValues":
                Slider3.SnapsTo = SliderSnapsTo.StepValues;
                break;
            default:
                Slider3.SnapsTo = SliderSnapsTo.Ticks;
                break;
        }
    }
}
