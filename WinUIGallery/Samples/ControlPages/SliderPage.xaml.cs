//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace WinUIGallery.ControlPages;

public sealed partial class SliderPage : Page
{
    public SliderPage()
    {
        InitializeComponent();
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
