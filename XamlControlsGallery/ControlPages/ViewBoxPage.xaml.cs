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
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ViewBoxPage : Page
    {
        public ViewBoxPage()
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
    }
}
