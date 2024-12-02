using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace WinUIGallery.ControlPages
{
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
                // Check the content of the selected RadioButton
                if (selectedRadioButton.Content.ToString() == "WrapGrid")
                {
                    MyListView.ItemsPanel = (ItemsPanelTemplate)this.Resources["WrapGridTemplate"];
                    Example2.Xaml = ReadSampleCodeFileContent("TemplatesSample2_WrapGrid_xaml");
                }
                else if (selectedRadioButton.Content.ToString() == "StackPanel")
                {
                    MyListView.ItemsPanel = (ItemsPanelTemplate)this.Resources["StackPanelTemplate"];
                    Example2.Xaml = ReadSampleCodeFileContent("TemplatesSample2_StackPanel_xaml");
                }
            }
        }

        private string ReadSampleCodeFileContent(string sampleCodeFileName)
        {
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            return File.ReadAllText($"{folder.Path}\\ControlPagesSampleCode\\Templates\\{sampleCodeFileName}.txt");
        }
    }
}
