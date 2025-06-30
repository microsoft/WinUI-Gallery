// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class ProgressBarPage : Page
{
    public ProgressBarPage()
    {
        this.InitializeComponent();
    }

    private void ProgressValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
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
