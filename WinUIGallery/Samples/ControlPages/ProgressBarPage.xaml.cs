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

namespace WinUIGallery.ControlPages;

public sealed partial class ProgressBarPage : Page
{
    public ProgressBarPage()
    {
        this.InitializeComponent();
    }

    private void ProgressValue_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        // Value might be NaN, which is not valid as value, thus we need to handle changes ourselves
       
        if (!double.IsNaN(sender.Value))
        {
            ProgressBar2.Value = sender.Value;
        }
        else
        {
            sender.Value = 0;
        }
    }
}
