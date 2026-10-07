// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System.IO;
using Windows.Storage;

namespace WinUIGallery.ControlPages;

public sealed partial class TemplatesPage : Page
{
    public TemplatesPage()
    {
        this.InitializeComponent();

        CodeTemplateComboBox.ItemTemplate = new DataTemplate(() =>
        {
            FontIcon icon = new FontIcon { Glyph = "\uE734", FontSize = 14 };

            TextBlock text = new TextBlock();
            text.SetBinding(TextBlock.TextProperty, new Binding());

            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            panel.Children.Add(icon);
            panel.Children.Add(text);
            return panel;
        });
    }

    private void LayoutSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems[0] is RadioButton selectedRadioButton)
        {
            // Check the tag of the selected RadioButton
            if (selectedRadioButton.Tag.ToString() == "WrapGrid")
            {
                MyListView.ItemsPanel = (ItemsPanelTemplate)Resources["WrapGridTemplate"];
                Example3.Xaml = ReadSampleCodeFileContent("TemplatesSample3_WrapGrid_xaml");
            }
            else if (selectedRadioButton.Tag.ToString() == "StackPanel")
            {
                MyListView.ItemsPanel = (ItemsPanelTemplate)Resources["StackPanelTemplate"];
                Example3.Xaml = ReadSampleCodeFileContent("TemplatesSample3_StackPanel_xaml");
            }
        }
    }

    private static string ReadSampleCodeFileContent(string sampleCodeFileName)
    {
        StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        return File.ReadAllText($"{folder.Path}\\Samples\\SampleCode\\Templates\\{sampleCodeFileName}.txt");
    }
}
