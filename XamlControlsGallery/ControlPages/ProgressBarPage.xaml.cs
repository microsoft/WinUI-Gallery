//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
