using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace WinUIGallery.Samples.ControlPages.Fundamentals.Controls;

public sealed partial class SynonymFinderControl : UserControl
    {
        public Dictionary<string, List<string>> SynonymsSource { get; set; } = new();
        public string Header { get; set; } = string.Empty;

        public SynonymFinderControl()
        {
            this.InitializeComponent();
        }

        private void InputTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                FindSynonyms();
            }
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            FindSynonyms();
        }

        private void FindSynonyms()
        {
            if (SynonymsSource == null)
            {
                return;
            }

            string key = InputTextBox.Text.Trim();
            if (SynonymsSource.TryGetValue(key, out var synonyms))
            {
                SynonymsList.ItemsSource = synonyms;
            }
            else
            {
                SynonymsList.ItemsSource = new List<string> { $"No synonyms found for \"{key}\"" };
            }
        }
    }
