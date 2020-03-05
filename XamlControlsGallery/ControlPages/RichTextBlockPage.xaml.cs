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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class RichTextBlockPage : Page
    {
        public RichTextBlockPage()
        {
            this.InitializeComponent();
        }


        private void HighlightColorCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get color to use
            var selectedItem = (sender as ComboBox).SelectedItem as ComboBoxItem;
            var color = Colors.Yellow;
            switch (selectedItem.Content as string)
            {
                case "Yellow":
                    color = Colors.Yellow;
                    break;
                case "Red":
                    color = Colors.Red;
                    break;
                case "Blue":
                    color = Colors.Blue;
                    break;
            }

            // Get text
            TextRange textRange = new TextRange() { StartIndex = 28, Length = 11 };
            TextHighlighter highlighter = new TextHighlighter()
            {
                Background = new SolidColorBrush((Color)color),
                Ranges = { textRange }
            };


            TextHighlightingRichTextBlock.TextHighlighters.Clear();
            TextHighlightingRichTextBlock.TextHighlighters.Add(highlighter);
        }
    }
}
