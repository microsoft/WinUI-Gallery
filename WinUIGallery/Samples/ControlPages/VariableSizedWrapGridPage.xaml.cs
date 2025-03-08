//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class VariableSizedWrapGridPage : Page
{
    public VariableSizedWrapGridPage()
    {
        this.InitializeComponent();
    }

    private void OrientationGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as RadioButtons)?.SelectedItem is not RadioButton selectedItem ||
            Enum.TryParse<Orientation>(selectedItem.Tag?.ToString(), out var orientation) is false ||
            Control1 is null)
        {
            return;
        }

        Control1.Orientation = orientation;
    }
}
