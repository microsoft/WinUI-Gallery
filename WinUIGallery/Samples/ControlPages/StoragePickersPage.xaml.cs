// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;
public sealed partial class StoragePickersPage : Page
{
    private IReadOnlyList<PickerLocationId> pickerLocationIds { get; set; } = new List<PickerLocationId>(Enum.GetValues<PickerLocationId>());
    private IReadOnlyList<PickerViewMode> pickerViewModes { get; set; } = new List<PickerViewMode>(Enum.GetValues<PickerViewMode>());

    public StoragePickersPage()
    {
        InitializeComponent();
    }

    private async void PickSingleFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            //disable the button to avoid double-clicking
            button.IsEnabled = false;

            var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            // Define allowed file types
            if (FileTypeComboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                string? tag = selectedItem.Tag?.ToString();

                switch (tag)
                {
                    case ".txt":
                        picker.FileTypeFilter.Add(".txt");
                        break;

                    case "images":
                        picker.FileTypeFilter.Add(".jpg");
                        picker.FileTypeFilter.Add(".png");
                        break;
                }
            }

            picker.CommitButtonText = CommitButtonTextTextBox.Text;

            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox1.SelectedItem;

            picker.ViewMode = (PickerViewMode)PickerViewModeComboBox1.SelectedItem;

            // Show the picker dialog window
            var file = await picker.PickSingleFileAsync();
            PickedSingleFileTextBlock.Text = file != null
                ? "Picked: " + file.Path
                : "No file selected.";

            //re-enable the button
            button.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(sender as Button, PickedSingleFileTextBlock.Text, "FilePickedNotificationId");
        }
    }

    private async void PickMultipleFilesButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            // Disable the button to avoid double-clicking
            button.IsEnabled = false;

            var picker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            // Define allowed file types
            if (FileTypeComboBox2.SelectedItem is ComboBoxItem selectedItem)
            {
                string? tag = selectedItem.Tag?.ToString();
                switch (tag)
                {
                    case ".txt":
                        picker.FileTypeFilter.Add(".txt");
                        break;

                    case "images":
                        picker.FileTypeFilter.Add(".jpg");
                        picker.FileTypeFilter.Add(".png");
                        break;

                    case "*":
                        picker.FileTypeFilter.Add("*");
                        break;
                }
            }

            picker.CommitButtonText = CommitButtonTextTextBox2.Text;
            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox2.SelectedItem;
            picker.ViewMode = (PickerViewMode)PickerViewModeComboBox2.SelectedItem;

            // Show the picker dialog
            var files = await picker.PickMultipleFilesAsync();

            if (files.Count > 0)
            {
                PickedMultipleFilesTextBlock.Text = "";
                foreach (var file in files)
                {
                    PickedMultipleFilesTextBlock.Text += "- Picked: " + file.Path + Environment.NewLine;
                }
            }
            else
            {
                PickedMultipleFilesTextBlock.Text = "No files selected.";
            }

            // Re-enable the button
            button.IsEnabled = true;

            // Announce result for accessibility (if you’re using the helper)
            UIHelper.AnnounceActionForAccessibility(button, PickedMultipleFilesTextBlock.Text, "FilesPickedNotificationId");
        }
    }

    private string ComboBoxItemToFileFilter(Object ob)
    {
        if (ob is ComboBoxItem item)
        {
            if ((string)item.Tag == ".txt") return "\r\n\r\n        picker.FileTypeFilter.Add(\".txt\");";
            if ((string)item.Tag == "images") return "\r\n\r\n        picker.FileTypeFilter.Add(\".jpg\");\r\n        picker.FileTypeFilter.Add(\".png\");";
        }
        return "";
    }

    private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;

            var picker = new FileSavePicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            if (TxtCheckBox.IsChecked == true)
                picker.FileTypeChoices.Add("Text Files", new List<string>() { ".txt" });

            if (JsonCheckBox.IsChecked == true)
                picker.FileTypeChoices.Add("JSON Files", new List<string>() { ".json" });

            if (XmlCheckBox.IsChecked == true)
                picker.FileTypeChoices.Add("XML Files", new List<string>() { ".xml" });

            picker.DefaultFileExtension = DefaultExtensionComboBox.SelectedItem.ToString();

            picker.SuggestedFileName = SuggestedFileNameTextBox.Text;

            picker.CommitButtonText = CommitButtonTextTextBox3.Text;

            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox3.SelectedItem;

            picker.SuggestedFolder = SuggestedFolderTextBox.Text;

            var result = await picker.PickSaveFileAsync();

            if (result != null)
            {
                string savePath = result.Path;
                await File.WriteAllTextAsync(savePath, FileContentTextBox.Text);
                SavedFileTextBlock.Text = "File saved to: " + savePath;
            }
            else
            {
                SavedFileTextBlock.Text = "File save canceled.";
            }

            button.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(button, SavedFileTextBlock.Text, "FileSavedNotificationId");
        }
    }

    string TxtCheckBoxIsCheckedToCode(bool? isChecked)
    {
        if (isChecked == true) return "\r\n        picker.FileTypeChoices.Add(\"Text Files\", new List<string>() { \".txt\" });\r\n";
        return "";
    }

    string JsonCheckBoxIsCheckedToCode(bool? isChecked)
    {
        if (isChecked == true) return "\r\n        picker.FileTypeChoices.Add(\"JSON Files\", new List<string>() { \".json\" });\r\n";
        return "";
    }

    string XmlCheckBoxIsCheckedToCode(bool? isChecked)
    {
        if (isChecked == true) return "\r\n        picker.FileTypeChoices.Add(\"XML Files\", new List<string>() { \".xml\" });\r\n";
        return "";
    }

    private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            // disable the button to avoid double-clicking
            button.IsEnabled = false;

            var picker = new FolderPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            picker.CommitButtonText = CommitButtonTextTextBox4.Text;
            picker.SuggestedStartLocation = (PickerLocationId)PickerLocationComboBox4.SelectedItem;
            picker.ViewMode = (PickerViewMode)PickerViewModeComboBox3.SelectedItem;

            // Show the picker dialog
            var folder = await picker.PickSingleFolderAsync();
            PickedFolderTextBlock.Text = folder != null
                ? "Picked: " + folder.Path
                : "No folder selected.";

            // re-enable the button
            button.IsEnabled = true;

            UIHelper.AnnounceActionForAccessibility(sender as Button, PickedFolderTextBlock.Text, "FolderPickedNotificationId");
        }
    }

    private async void SelectSuggestedFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;

            var picker = new FolderPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);

            picker.CommitButtonText = "Select folder";

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                SuggestedFolderTextBox.Text = folder.Path;
            }

            button.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(
                sender as Button,
                folder != null && !string.IsNullOrEmpty(folder.Path)
                    ? "Folder selected: " + SuggestedFolderTextBox.Text
                    : "No folder selected",
                "SuggestedFolderNotificationId");
        }
    }
}
