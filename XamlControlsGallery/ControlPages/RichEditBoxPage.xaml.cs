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
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Microsoft.UI;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class RichEditBoxPage : Page
    {
        private Windows.UI.Color currentColor = Colors.Black;
        // String used to restore the colors when the focus gets reenabled
        // See #144 for more info https://github.com/microsoft/Xaml-Controls-Gallery/issues/144
        private string LastFormattedText = "";
        public RichEditBoxPage()
        {
            this.InitializeComponent();
        }

        private void Menu_Opening(object sender, object e)
        {
            CommandBarFlyout myFlyout = sender as CommandBarFlyout;
            if (myFlyout.Target == REBCustom)
            {
                AppBarButton myButton = new AppBarButton();
                myButton.Command = new StandardUICommand(StandardUICommandKind.Share);
                myFlyout.PrimaryCommands.Add(myButton);
            }
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a text file.
            Windows.Storage.Pickers.FileOpenPicker open =
                new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".rtf");

            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Load the file into the Document property of the RichEditBox.
                    editor.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream);
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we 
                // finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                }

                // Let Windows know that we're finished changing the file so the 
                // other app can update the remote version of the file.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = ((Microsoft.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

            editor.Document.Selection.CharacterFormat.ForegroundColor = color;

            fontColorButton.Flyout.Hide();
            editor.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
            currentColor = color;
        }

        private void FindBoxHighlightMatches()
        {
            FindBoxRemoveHighlights();

            Windows.UI.Color highlightBackgroundColor = (Windows.UI.Color)App.Current.Resources["SystemColorHighlightColor"];
            Windows.UI.Color highlightForegroundColor = (Windows.UI.Color)App.Current.Resources["SystemColorHighlightTextColor"];

            string textToFind = findBox.Text;
            if (textToFind != null)
            {
                ITextRange searchRange = editor.Document.GetRange(0, 0);
                while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                {
                    searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                    searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                }
            }
        }

        private void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush defaultBackground = editor.Background as SolidColorBrush;
            SolidColorBrush defaultForeground = editor.Foreground as SolidColorBrush;

            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            // reset colors to correct defaults for Focused state
            ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];
            SolidColorBrush foreground = (SolidColorBrush)App.Current.Resources["TextControlForegroundFocused"];

            editor.Document.ApplyDisplayUpdates();

            if (background != null && foreground != null)
            {
                documentRange.CharacterFormat.BackgroundColor = background.Color;
            }
            // saving selection span
            var caretPosition = editor.Document.Selection.GetIndex(TextRangeUnit.Character) - 1;
            if (caretPosition <= 0)
            {
                // User has not entered text, prevent invalid values and just set index to 1
                caretPosition = 1;
            }
            var selectionLength = editor.Document.Selection.Length;
            // restoring text styling, unintentionally sets caret position at beginning of text
            editor.Document.SetText(TextSetOptions.FormatRtf, LastFormattedText);
            // restoring selection position
            editor.Document.Selection.SetIndex(TextRangeUnit.Character, caretPosition, false);
            editor.Document.Selection.SetRange(caretPosition, caretPosition + selectionLength);
            // Editor might have gained focus because user changed color.
            // Change selection color
            // Note that only way to regain with selection containing text is using the change color button
            editor.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
        }

        private void Editor_LosingFocus(object sender, RoutedEventArgs e)
        {
            editor.Document.GetText(TextGetOptions.FormatRtf, out LastFormattedText);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 768)
            {
                editor.Width = e.NewSize.Width - 20;
            }
            else
            {
                editor.Width = e.NewSize.Width - 100;
            }
        }

        private void REBCustom_Loaded(object sender, RoutedEventArgs e)
        {
            // Prior to UniversalApiContract 7, RichEditBox did not have a default ContextFlyout set.
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                // customize the menu that opens on text selection
                REBCustom.SelectionFlyout.Opening += Menu_Opening;

                // also customize the context menu to match selection menu
                REBCustom.ContextFlyout.Opening += Menu_Opening;
            }
        }

        private void REBCustom_Unloaded(object sender, RoutedEventArgs e)
        {
            // Prior to UniversalApiContract 7, RichEditBox did not have a default ContextFlyout set.
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                REBCustom.SelectionFlyout.Opening -= Menu_Opening;
                REBCustom.ContextFlyout.Opening -= Menu_Opening;
            }
        }

        private void Editor_TextChanging(object sender, RichEditBoxTextChangingEventArgs e)
        {
            // Fix bug where selected text would get colored when editor loses focus
            if (FocusManager.GetFocusedElement() == editor)
            {
                editor.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
            }
        }
    }
}