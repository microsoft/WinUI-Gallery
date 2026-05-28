// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.IO;
using Windows.Storage;

namespace WinUIGallery.ControlPages;

public sealed partial class TemplatesPage : Page
{
    public TemplatesPage()
    {
        this.InitializeComponent();
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
