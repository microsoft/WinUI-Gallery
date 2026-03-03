// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class ClipboardPage : Page
{

    private string textToCopy = "";

    public ClipboardPage()
    {
        this.InitializeComponent();
        richEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "This text will be copied to the clipboard.");
        UpdateHistoryRoamingStatus();
    }

    private void CopyText_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        richEditBox.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out textToCopy);
        var package = new DataPackage();
        package.SetText(textToCopy);
        Clipboard.SetContent(package);

        UIHelper.AnnounceActionForAccessibility(button, "Text copied to clipboard", "TextCopiedSuccessNotificationId");

        VisualStateManager.GoToState(this, "ConfirmationClipboardVisible", false);
        Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        // Automatically hide the confirmation text after 2 seconds
        if (dispatcherQueue != null)
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                await Task.Delay(2000);
                VisualStateManager.GoToState(this, "ConfirmationClipboardCollapsed", false);
            });
        }

    }

    private async void PasteText_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Text))
        {
            var text = await package.GetTextAsync();
            PasteClipboard2.Text = text;

            UIHelper.AnnounceActionForAccessibility(button, "Text pasted from clipboard", "TextPastedSuccessNotificationId");
        }

    }

    private void CopyImage_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = new DataPackage();
        var imageUri = new Uri("ms-appx:///Assets/SampleMedia/rainier.jpg");
        package.SetBitmap(RandomAccessStreamReference.CreateFromUri(imageUri));

        if (Clipboard.SetContentWithOptions(package, null))
        {
            ImageStatusText.Text = "Image copied to clipboard.";
            ImageStatusText.Visibility = Visibility.Visible;
            UIHelper.AnnounceActionForAccessibility(button, "Image copied to clipboard", "ImageCopiedSuccessNotificationId");
        }
        else
        {
            ImageStatusText.Text = "Error copying image to clipboard.";
            ImageStatusText.Visibility = Visibility.Visible;
        }
    }

    private async void PasteImage_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Bitmap))
        {
            try
            {
                IRandomAccessStreamReference imageReference = await package.GetBitmapAsync();
                using (IRandomAccessStreamWithContentType imageStream = await imageReference.OpenReadAsync())
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(imageStream);
                    PastedImage.Source = bitmapImage;
                    PastedImage.Visibility = Visibility.Visible;
                    ImageStatusText.Text = "Image pasted from clipboard.";
                    ImageStatusText.Visibility = Visibility.Visible;
                    UIHelper.AnnounceActionForAccessibility(button, "Image pasted from clipboard", "ImagePastedSuccessNotificationId");
                }
            }
            catch (Exception ex)
            {
                ImageStatusText.Text = "Error pasting image: " + ex.Message;
                ImageStatusText.Visibility = Visibility.Visible;
            }
        }
        else
        {
            ImageStatusText.Text = "Bitmap format is not available in the clipboard.";
            ImageStatusText.Visibility = Visibility.Visible;
            PastedImage.Visibility = Visibility.Collapsed;
        }
    }

    private async void CopyFiles_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var filePicker = new FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);
        filePicker.FileTypeFilter.Add("*");

        var pickedFiles = await filePicker.PickMultipleFilesAsync();
        if (pickedFiles.Count > 0)
        {
            List<Windows.Storage.IStorageItem> storageItems = new();
            foreach (var file in pickedFiles)
            {
                storageItems.Add(await Windows.Storage.StorageFile.GetFileFromPathAsync(file.Path));
            }

            var package = new DataPackage();
            package.SetStorageItems(storageItems);
            package.RequestedOperation = DataPackageOperation.Copy;

            if (Clipboard.SetContentWithOptions(package, null))
            {
                FilesStatusText.Text = $"{pickedFiles.Count} file(s) copied to clipboard.";
                UIHelper.AnnounceActionForAccessibility(button, $"{pickedFiles.Count} files copied to clipboard", "FilesCopiedSuccessNotificationId");
            }
            else
            {
                FilesStatusText.Text = "Error copying files to clipboard.";
            }
        }
    }

    private async void PasteFiles_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.StorageItems))
        {
            try
            {
                var storageItems = await package.GetStorageItemsAsync();
                DataPackageOperation operation = package.RequestedOperation;

                string operationName = operation switch
                {
                    DataPackageOperation.Copy => "Copy",
                    DataPackageOperation.Move => "Move",
                    DataPackageOperation.Link => "Link",
                    _ => "None"
                };

                var output = new StringBuilder();
                output.AppendLine($"Requested operation: {operationName}");
                output.AppendLine($"File(s) on clipboard ({storageItems.Count}):");
                foreach (var item in storageItems)
                {
                    output.AppendLine($"  • {item.Name}");
                }

                FilesStatusText.Text = output.ToString();
                UIHelper.AnnounceActionForAccessibility(button, "Files pasted from clipboard", "FilesPastedSuccessNotificationId");
            }
            catch (Exception ex)
            {
                FilesStatusText.Text = "Error pasting files: " + ex.Message;
            }
        }
        else
        {
            FilesStatusText.Text = "StorageItems format is not available in the clipboard.";
        }
    }

    private void CopyWithOptions_Click(object sender, RoutedEventArgs args)
    {
        if (sender is not Button button)
        {
            return;
        }

        string text = HistoryRoamingTextBox.Text;
        if (string.IsNullOrEmpty(text))
        {
            HistoryRoamingStatusText.Text = "Please enter text to copy.";
            return;
        }

        var package = new DataPackage();
        package.SetText(text);

        var options = new ClipboardContentOptions
        {
            IsAllowedInHistory = AllowHistoryToggle.IsOn,
            IsRoamable = AllowRoamingToggle.IsOn
        };

        if (Clipboard.SetContentWithOptions(package, options))
        {
            var status = new StringBuilder("Text copied to clipboard.");
            status.Append($" History: {(options.IsAllowedInHistory ? "allowed" : "excluded")}.");
            status.Append($" Roaming: {(options.IsRoamable ? "allowed" : "excluded")}.");
            HistoryRoamingStatusText.Text = status.ToString();
            UIHelper.AnnounceActionForAccessibility(button, "Text copied with options", "OptionsCopiedSuccessNotificationId");
        }
        else
        {
            HistoryRoamingStatusText.Text = "Error copying content to clipboard.";
        }
    }

    private void UpdateHistoryRoamingStatus()
    {
        try
        {
            bool historyEnabled = Clipboard.IsHistoryEnabled();
            bool roamingEnabled = Clipboard.IsRoamingEnabled();
            HistoryEnabledText.Text = $"Clipboard history: {(historyEnabled ? "enabled" : "disabled")}";
            RoamingEnabledText.Text = $"Clipboard roaming: {(roamingEnabled ? "enabled" : "disabled")}";
        }
        catch
        {
            // IsHistoryEnabled/IsRoamingEnabled may not be available on all systems
        }
    }

    private void ShowFormats_Click(object sender, RoutedEventArgs args)
    {
        DataPackageView package = Clipboard.GetContent();
        var output = new StringBuilder();

        if (package != null && package.AvailableFormats.Count > 0)
        {
            output.AppendLine("Available formats on the clipboard:");
            foreach (string format in package.AvailableFormats)
            {
                output.AppendLine($"  • {format}");
            }
        }
        else
        {
            output.AppendLine("The clipboard is empty.");
        }

        OtherOperationsStatusText.Text = output.ToString();
    }

    private void ClearClipboard_Click(object sender, RoutedEventArgs args)
    {
        try
        {
            Clipboard.Clear();
            OtherOperationsStatusText.Text = "Clipboard has been cleared.";
        }
        catch (Exception ex)
        {
            OtherOperationsStatusText.Text = "Error clearing clipboard: " + ex.Message;
        }
    }

    private void ContentChangedToggle_Toggled(object sender, RoutedEventArgs args)
    {
        if (ContentChangedToggle.IsOn)
        {
            Clipboard.ContentChanged += OnClipboardContentChanged;
            OtherOperationsStatusText.Text = "Monitoring clipboard changes...";
        }
        else
        {
            Clipboard.ContentChanged -= OnClipboardContentChanged;
            OtherOperationsStatusText.Text = "Stopped monitoring clipboard changes.";
        }
    }

    private void OnClipboardContentChanged(object? sender, object e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            DataPackageView package = Clipboard.GetContent();
            var output = new StringBuilder("Clipboard content changed!\n");

            if (package != null && package.AvailableFormats.Count > 0)
            {
                output.AppendLine("New formats:");
                foreach (string format in package.AvailableFormats)
                {
                    output.AppendLine($"  • {format}");
                }
            }
            else
            {
                output.AppendLine("Clipboard is now empty.");
            }

            OtherOperationsStatusText.Text = output.ToString();
        });
    }
}
