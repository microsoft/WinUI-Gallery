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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.ControlPages;

public sealed partial class RichTextBlockPage : Page
{
    public RichTextBlockPage()
    {
        InitializeComponent();
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

        // Get text range and highlighter
        TextRange textRange = new()
        {
            StartIndex = 28,
            Length = 11
        };
        TextHighlighter highlighter = new()
        {
            Background = new SolidColorBrush(color),
            Ranges = { textRange }
        };

        // Switch texthighlighter
        TextHighlightingRichTextBlock.TextHighlighters.Clear();
        TextHighlightingRichTextBlock.TextHighlighters.Add(highlighter);
    }
}
