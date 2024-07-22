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
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinUIGallery.Common;

namespace WinUIGallery.ControlPages
{
    public sealed partial class ListBoxPage : Page
    {
        private List<FontFamilyTuple> _fonts = new List<FontFamilyTuple>()
        {
            new FontFamilyTuple("Arial", new FontFamily("Arial")),
            new FontFamilyTuple("Comic Sans MS", new FontFamily("Comic Sans MS")),
            new FontFamilyTuple("Courier New", new FontFamily("Courier New")),
            new FontFamilyTuple("Segoe UI", new FontFamily("Segoe UI")),
            new FontFamilyTuple("Times New Roman", new FontFamily("Times New Roman"))
        };

        public List<FontFamilyTuple> Fonts
        {
            get { return _fonts; }
        }
        public ListBoxPage()
        {
            this.InitializeComponent();
        }

        private void ColorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string colorName = e.AddedItems[0].ToString();
            switch (colorName)
            {
                case "Yellow":
                    Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
                    break;
                case "Green":
                    Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Green);
                    break;
                case "Blue":
                    Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Blue);
                    break;
                case "Red":
                    Control1Output.Fill = new SolidColorBrush(Microsoft.UI.Colors.Red);
                    break;
            }
        }

        private void ListBox2_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox2.SelectedIndex = 2;
        }
    }
}
