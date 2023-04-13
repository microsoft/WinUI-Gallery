//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AccessibilityKeyboardPage : Page
    {
        public AccessibilityKeyboardPage()
        {
            this.InitializeComponent();
        }

        private void MakeRedButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRectangle.Fill = new SolidColorBrush(Colors.Red);
        }
        private void MakeBlueButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRectangle.Fill = new SolidColorBrush(Colors.Blue);
        }
        private void MakeChartreuseButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRectangle.Fill = new SolidColorBrush(Colors.Chartreuse);
        }
    }
}
