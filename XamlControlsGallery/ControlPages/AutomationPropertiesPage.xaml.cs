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

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// Page containing sample related to XAML AutomationProperties.
    /// </summary>
    public sealed partial class AutomationPropertiesPage : Page
    {
        public AutomationPropertiesPage()
        {
            this.InitializeComponent();
        }

        private void FontSizeNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            // Ensure that if user clears the NumberBox, we don't pass 0 or null as fontsize
            if(sender.Value >= sender.Minimum)
            {
                FontSizeChangingTextBlock.FontSize = sender.Value;
            }
            else
            {
                // We fell below minimum, so lets restore a correct value
                sender.Value = sender.Minimum;
            }
        }
    }
}
