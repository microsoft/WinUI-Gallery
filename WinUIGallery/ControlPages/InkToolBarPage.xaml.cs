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

namespace WinUIGallery.ControlPages
{
    public sealed partial class InkToolBarPage : Page
    {
        public InkToolBarPage()
        {
            this.InitializeComponent();
        }

        private void InputType_Changed(object sender, RoutedEventArgs e)
        {
            if (inkCanvas2 == null) return;

            var inputTypes = InkInputType.None;
            if (PenCheck.IsChecked == true) inputTypes |= InkInputType.Pen;
            if (TouchCheck.IsChecked == true) inputTypes |= InkInputType.Touch;
            if (MouseCheck.IsChecked == true) inputTypes |= InkInputType.Mouse;

            inkCanvas2.AllowedInputTypes = inputTypes;
        }
    }
}
