//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ProgressBarPage : Page
    {
        public ProgressBarPage()
        {
            this.InitializeComponent();
        }

        private void ProgressValue_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            // Value might be NaN, which is not valid as value, thus we need to handle changes ourselves
            if (!sender.Value.IsNaN())
            {
                ProgressBar2.Value = sender.Value;
            }
            else
            {
                sender.Value = 0;
            }
        }
    }
}
