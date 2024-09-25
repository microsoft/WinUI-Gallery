using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WinUIGallery.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace WinUIGallery.ControlPages
{
    public sealed partial class FilePickerPage : Page
    {

        public FilePickerPage()
        {
            this.InitializeComponent();
        }

        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickAFileOutputTextBlock.Text = "";

            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Creating the text for the Texblock
                Span span = new Span();
                Run run1 = new Run();
                run1.Text = "Picked file: ";

                // Adding the name of the picked file in bold
                Run run2 = new Run();
                run2.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
                run2.Text = file.Name;
                
                span.Inlines.Add(run1);
                span.Inlines.Add(run2);
                PickAFileOutputTextBlock.Inlines.Add(span);
            }
            else
            {
                PickAFileOutputTextBlock.Text = "Operation cancelled.";
            }

            //re-enable the button
            senderButton.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(sender as Button, PickAFileOutputTextBlock.Text, "FilePickedNotificationId");
        }
        private async void PickAPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickAPhotoOutputTextBlock.Text = "";

            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Creating the text for the Texblock
                Span span = new Span();
                Run run1 = new Run();
                run1.Text = "Picked photo: ";

                // Adding the name of the picked file in bold
                Run run2 = new Run();
                run2.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
                run2.Text = file.Name;

                span.Inlines.Add(run1);
                span.Inlines.Add(run2);
                PickAPhotoOutputTextBlock.Inlines.Add(span);
            }
            else
            {
                PickAPhotoOutputTextBlock.Text = "Operation cancelled.";
            }

            //re-enable the button
            senderButton.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(sender as Button, PickAPhotoOutputTextBlock.Text, "PhotoPickedNotificationId");
        }

        private async void PickFilesButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickFilesOutputTextBlock.Text = "";

            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                // Creating the text for the Texblock
                Span span = new Span();
                Run run1 = new Run();
                run1.Text = "Picked files:\n";
                span.Inlines.Add(run1);

                // Adding the names of the picked files in bold
                foreach (StorageFile file in files)
                {
                    Run runTemp = new Run();
                    runTemp.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
                    runTemp.Text = file.Name + "\n";

                    span.Inlines.Add(runTemp);
                }

                PickFilesOutputTextBlock.Inlines.Add(span);
            }
            else
            {
                PickFilesOutputTextBlock.Text = "Operation cancelled.";
            }

            //re-enable the button
            senderButton.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(sender as Button, PickFilesOutputTextBlock.Text, "FilesPickedNotificationId");
        }

        private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickFolderOutputTextBlock.Text = "";

            // Create a folder picker
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                // Creating the text for the Texblock
                Span span = new Span();
                Run run1 = new Run();
                run1.Text = "Picked folder: ";

                // Adding the name of the picked file in bold
                Run run2 = new Run();
                run2.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
                run2.Text = folder.Name;

                span.Inlines.Add(run1);
                span.Inlines.Add(run2);
                PickFolderOutputTextBlock.Inlines.Add(span);
            }
            else
            {
                PickFolderOutputTextBlock.Text = "Operation cancelled.";
            }

            //re-enable the button
            senderButton.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(sender as Button, PickFolderOutputTextBlock.Text, "FolderPickedNotificationId");
        }

        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            // Clear previous returned file name, if it exists, between iterations of this scenario
            SaveFileOutputTextBlock.Text = "";

            // Create a file picker
            FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            // Set options for your file picker
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            var enteredFileName = ((sender as Button).Parent as StackPanel)
            .FindName("FileNameTextBox") as TextBox;
            savePicker.SuggestedFileName = enteredFileName.Text;

            // Open the picker for the user to pick a file
            StorageFile file= await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);

                // write to file
                var textBox = ((sender as Button).Parent as StackPanel).FindName("FileContentTextBox") as TextBox;
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    using (var tw = new StreamWriter(stream))
                    {
                        tw.WriteLine(textBox?.Text);
                    }
                }
                // Another way to write a string to the file is to use this instead:
                // await FileIO.WriteTextAsync(file, "Example file contents.");
                
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    SaveFileOutputTextBlock.Text = "File " + file.Name + " was saved.";
                }
                else if (status == FileUpdateStatus.CompleteAndRenamed)
                {
                    SaveFileOutputTextBlock.Text = "File " + file.Name + " was renamed and saved.";
                }
                else
                {
                    SaveFileOutputTextBlock.Text = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                SaveFileOutputTextBlock.Text = "Operation cancelled.";
            }

            //re-enable the button
            senderButton.IsEnabled = true;
            UIHelper.AnnounceActionForAccessibility(sender as Button, SaveFileOutputTextBlock.Text, "FileSavedNotificationId");
        }
    }
}
