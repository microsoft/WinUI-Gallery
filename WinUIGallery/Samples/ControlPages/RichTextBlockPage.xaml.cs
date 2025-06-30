// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.ControlPages;

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

        // Get text range and highlighter
        TextRange textRange = new TextRange()
        {
            StartIndex = 28,
            Length = 11
        };
        TextHighlighter highlighter = new TextHighlighter()
        {
            Background = new SolidColorBrush(color),
            Ranges = { textRange }
        };

        // Switch texthighlighter
        TextHighlightingRichTextBlock.TextHighlighters.Clear();
        TextHighlightingRichTextBlock.TextHighlighters.Add(highlighter);
    }
}
