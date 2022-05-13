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
    public sealed partial class VariableSizedWrapGridPage : Page
    {
        public VariableSizedWrapGridPage()
        {
            this.InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && Control1 != null)
            {
                string orientationName = rb.Tag.ToString();

                switch (orientationName)
                {
                    case "Horizontal":
                        Control1.Orientation = Orientation.Horizontal;
                        break;
                    case "Vertical":
                        Control1.Orientation = Orientation.Vertical;
                        break;
                }
            }
        }
    }
}
