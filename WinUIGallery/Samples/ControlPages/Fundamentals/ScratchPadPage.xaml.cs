// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using System;
using System.Linq;
using Windows.Storage;
using Windows.System;

namespace WinUIGallery.ControlPages;

public sealed partial class ScratchPadPage : Page
{
    // The name of the ApplicationData container used to store all ScratchPad-related settings.
    private const string containerKey = "ScratchPad";

    // The key under the ScratchPad container for storing the XAML composite value.
    private const string xamlCompositeValueKey = "ScratchPadXAML";

    // The key within the XAML composite value that stores the number of 4KB segments.
    private const string xamlSegmentCountKey = "count";

    public ScratchPadPage()
    {
        this.InitializeComponent();

        var xamlStr = ReadScratchPadXAMLinLocalSettings();

        // If there is no stored XAML, load the default.
        if (xamlStr == null || xamlStr.Trim().Length == 0)
        {
            xamlStr = GetDefaultScratchXAML();
        }

        m_oldText = xamlStr;
        textbox.TextDocument.SetText(TextSetOptions.None, m_oldText);
        var formatter = new XamlTextFormatter(textbox);
        formatter.ApplyColors();

        // Provide some initial instruction in the content area.
        SetEmptyScratchPadContent();
    }

    private void SetEmptyScratchPadContent()
    {
        scratchPad.Content = new TextBlock()
        {
            Text = "Click the Load button to load the content below.",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
    }

    private string GetDefaultScratchXAML()
    {
        return
@"<StackPanel  BorderThickness=""1"" BorderBrush=""Green"" CornerRadius=""4"" Padding=""3"">
    <!-- Note: {x:Bind} is not supported in Scratch Pad. -->
    <TextBlock>This is a sample TextBlock.</TextBlock>
    <Button Content=""Click me!""/>

    <!-- Note: Syntax highlighting updates on 'Load'. -->
</StackPanel>";
    }

    public string ReadScratchPadXAMLinLocalSettings()
    {
        var appData = Microsoft.Windows.Storage.ApplicationData.GetDefault();
        if (appData.LocalSettings.Containers.ContainsKey(containerKey))
        {
            var scratchPadContainer = appData.LocalSettings.CreateContainer(containerKey, Microsoft.Windows.Storage.ApplicationDataCreateDisposition.Existing);
            if (scratchPadContainer != null && scratchPadContainer.Values.ContainsKey(xamlCompositeValueKey))
            {
                // String values are limited to 4K characters. Use a composite value to support a longer string.
                var compositeStr = scratchPadContainer.Values[xamlCompositeValueKey] as ApplicationDataCompositeValue;
                var xamlStr = "";
                int count = (int)compositeStr[xamlSegmentCountKey];
                for (int i = 0; i < count; i++)
                {
                    xamlStr += compositeStr[i.ToString()];
                }
                return xamlStr;
            }
        }
        return null;
    }

    public void SaveScratchPadXAMLinLocalSettings(string xamlStr)
    {
        var appData = Microsoft.Windows.Storage.ApplicationData.GetDefault();
        var scratchPadContainer = appData.LocalSettings.CreateContainer(containerKey, Microsoft.Windows.Storage.ApplicationDataCreateDisposition.Existing);
        // String values are limited to 4K characters. Use a composite value to support a longer string.
        var compositeStr = new ApplicationDataCompositeValue();
        int count = 0;
        while (xamlStr.Length > 0)
        {
            var len = Math.Min(xamlStr.Length, 4000);
            compositeStr[count.ToString()] = xamlStr.Substring(0, len);
            count++;
            xamlStr = xamlStr.Substring(len);
        }
        compositeStr[xamlSegmentCountKey] = count;
        scratchPadContainer.Values[xamlCompositeValueKey] = compositeStr;
    }

    private async void ResetToDefaultClick(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog();
        dialog.XamlRoot = this.XamlRoot;
        dialog.Title = "Are you sure you want to reset?";
        dialog.Content = "Resetting to the default content will replace your current content. Are you sure you want to reset?";
        dialog.PrimaryButtonText = "Reset";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.RequestedTheme = this.ActualTheme;

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            m_oldText = GetDefaultScratchXAML();
            textbox.TextDocument.SetText(TextSetOptions.None, m_oldText);
            var formatter = new XamlTextFormatter(textbox);
            formatter.ApplyColors();

            SetEmptyScratchPadContent();
            loadStatus.Text = "";
        }
    }

    private string AddXmlNamespace(string xml)
    {
        xml = xml.Trim();

        char[] chars = { ' ', '/', '>' };
        var insertIndex = xml.IndexOfAny(chars);
        if (insertIndex < 0)
        {
            throw new ArgumentException("No end tag.");
        }

        xml = xml.Substring(0, insertIndex) + " xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " + xml.Substring(insertIndex);
        return xml;
    }

    private void LoadContent()
    {
        string newText;
        textbox.TextDocument.GetText(TextGetOptions.None, out newText);
        //System.Diagnostics.Debug.WriteLine("new text: " + newText);

        SaveScratchPadXAMLinLocalSettings(newText);

        // TODO: Strip out x:Bind -- maybe just convert it to spaces?
        try
        {
            loadStatus.Text = ""; // Clear the log before loading

            var xml = AddXmlNamespace(newText);

            var element = (UIElement)XamlReader.Load(xml);
            scratchPad.Content = element;
            loadStatus.Text = "Load successful.";
        }
        catch (Exception ex)
        {
            loadStatus.Text = ex.Message + "\n" + loadStatus.Text;
        }
        loadStatus.Opacity = 1.0;
    }

    private void LoadContentAndApplyFormatting()
    {
        LoadContent();

        m_lastChangeFromTyping = false;
        var formatter = new XamlTextFormatter(textbox);
        formatter.ApplyColors();
    }

    private void InsertTextboxText(string str, bool setCursorAfterInsertedStr)
    {
        var selectionStart = textbox.TextDocument.Selection.StartPosition;
        var range = textbox.TextDocument.GetRange(selectionStart, selectionStart);
        m_lastChangeFromTyping = false;
        range.Text = str;
        textbox.TextDocument.Selection.StartPosition = selectionStart + (setCursorAfterInsertedStr ? str.Length : 0);
    }

    private string GetTextboxTextPreviousLine()
    {
        string newText;
        textbox.TextDocument.GetText(TextGetOptions.None, out newText);
        var selectionIndex = textbox.TextDocument.Selection.StartPosition;
        if (selectionIndex > 0)
        {
            char[] eolChars = { '\r', '\n' };
            var endOfLineIndex = newText.LastIndexOfAny(eolChars, selectionIndex - 1);
            if (endOfLineIndex > 0)
            {
                var startOfLineIndex = newText.LastIndexOfAny(eolChars, endOfLineIndex - 1) + 1;
                return newText.Substring(startOfLineIndex, endOfLineIndex - startOfLineIndex);
            }
        }
        return "";
    }

    private void HandleEnter()
    {
        textbox.TextDocument.BeginUndoGroup();
        string previousLine = GetTextboxTextPreviousLine();
        string indentStr = previousLine.Substring(0, previousLine.Length - previousLine.TrimStart().Length);
        // TODO: Indent further if this is the start of a child tag or property.
        InsertTextboxText(indentStr, true);

        // If this looks like the start of content area in a tag, further indent and put the end tag on a new line.
        var selectionStart = textbox.TextDocument.Selection.StartPosition;
        var range = textbox.TextDocument.GetRange(selectionStart, selectionStart + 2);
        if (range.Text == "</")
        {
            InsertTextboxText("    ", true);
            selectionStart = textbox.TextDocument.Selection.StartPosition;
            range = textbox.TextDocument.GetRange(selectionStart, selectionStart);
            range.Text = "\n" + indentStr;
        }
        textbox.TextDocument.EndUndoGroup();
    }

    private void textbox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
    }
    private void textbox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        m_lastChangeFromTyping = true;
        switch (e.Key)
        {
            case Windows.System.VirtualKey.Tab:
                if (textbox.TextDocument.Selection.Length > 1)
                {
                    var isShiftKeyDown = ((int)InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift) &
                        (int)Windows.UI.Core.CoreVirtualKeyStates.Down) == (int)Windows.UI.Core.CoreVirtualKeyStates.Down;
                    string text;
                    textbox.TextDocument.GetText(TextGetOptions.None, out text);

                    var selectionStart = textbox.TextDocument.Selection.StartPosition;
                    var selectionEnd = selectionStart + textbox.TextDocument.Selection.Length;
                    char[] eolChars = { '\r', '\n' };
                    var startOfLineIndex = text.LastIndexOfAny(eolChars, selectionStart) + 1;
                    if (startOfLineIndex >= 0)
                    {
                        var range = textbox.TextDocument.GetRange(startOfLineIndex, startOfLineIndex);

                        if (!isShiftKeyDown) // Indent
                        {
                            range.Text = "    ";
                            selectionStart += 4;
                            selectionEnd += 4;

                            range.Move(TextRangeUnit.Paragraph, 1);
                            while (range.StartPosition < selectionEnd)
                            {
                                range.Text = "    ";
                                selectionEnd += 4;
                                range.Move(TextRangeUnit.Paragraph, 1);
                            }
                        }
                        else // Unindent
                        {
                            bool firstLine = true;
                            while (range.StartPosition < selectionEnd)
                            {
                                range.MoveEnd(TextRangeUnit.Character, 4);
                                var numWhitespace = range.Text.Count(char.IsWhiteSpace);
                                range = textbox.TextDocument.GetRange(range.StartPosition, range.StartPosition + numWhitespace);
                                range.Text = "";
                                if (firstLine)
                                {
                                    firstLine = false;
                                    selectionStart -= numWhitespace;
                                }
                                selectionEnd -= numWhitespace;
                                range.Move(TextRangeUnit.Paragraph, 1);
                            }
                        }

                        textbox.TextDocument.Selection.StartPosition = selectionStart;
                        textbox.TextDocument.Selection.EndPosition = selectionEnd;

                        e.Handled = true;
                    }
                }
                break;
        }
    }
    private void textbox_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
    {
        m_lastChangeFromTyping = true;
        switch (e.Key)
        {
            case VirtualKey.F5:
                LoadContentAndApplyFormatting();
                break;

            case VirtualKey.Enter:
                HandleEnter();
                break;
        }
    }

    private void LoadClick(object sender, RoutedEventArgs e)
    {
        LoadContentAndApplyFormatting();
    }

    bool m_lastChangeFromTyping = false;
    string m_oldText = "";
    private void textbox_TextChanged(object sender, RoutedEventArgs e)
    {
        if (textbox.TextDocument.Selection.Length == 0 && m_lastChangeFromTyping)
        {
            if (loadStatus.Text == "Load successful.")
            {
                loadStatus.Text = "";
            }
            else
            {
                loadStatus.Opacity = 0.5; // dim the message which is now old
            }

            string newText;
            textbox.TextDocument.GetText(TextGetOptions.None, out newText);
            if (newText.Length == m_oldText.Length + 1)
            {
                // Added just one character
                var selectionIndex = textbox.TextDocument.Selection.StartPosition;
                if (selectionIndex >= 2 && newText[selectionIndex - 1] == '>' && newText[selectionIndex - 2] != '/')
                {
                    var tagStartIndex = newText.LastIndexOf('<', selectionIndex - 1);
                    // If we found a start tag and there wasn't already a close of that tag
                    if (tagStartIndex >= 0 && newText.LastIndexOf('>', selectionIndex - 2) < tagStartIndex)
                    {
                        var tagName = newText.Substring(tagStartIndex + 1);

                        char[] chars = { ' ', '/', '>', '\t', '\r', '\n' };
                        var nameEndIndex = tagName.IndexOfAny(chars);
                        if (nameEndIndex > 0)
                        {
                            tagName = tagName = tagName.Substring(0, nameEndIndex);
                            if (tagName != "!--") // don't add a close tag for a comment
                            {
                                InsertTextboxText("</" + tagName + ">", false);
                            }
                        }
                    }
                }
                else if (newText[selectionIndex - 1] == '=' &&
                    (selectionIndex >= newText.Length - 1 || newText[selectionIndex] != '"'))
                {
                    // Might want to auto-insert quotes for a property. Check if this appears
                    // to be inside a tag and just after a property name.
                    char[] tagChars = { '<', '>' };
                    var lastTagIndex = newText.LastIndexOfAny(tagChars, selectionIndex);
                    if (lastTagIndex >= 0 && newText[lastTagIndex] == '<')
                    {
                        // In a tag. Make sure we aren't in a property value (improper comparison
                        // here is to check if the last quote is proceed by an '=').
                        var quoteIndex = newText.LastIndexOf('"', selectionIndex);
                        if (quoteIndex < lastTagIndex || newText[quoteIndex - 1] != '=')
                        {
                            InsertTextboxText("\"\"", false);
                            textbox.TextDocument.Selection.StartPosition++;
                        }
                    }

                }
            }
        }

        // Save the text so next time we can compare against the new text
        textbox.TextDocument.GetText(TextGetOptions.None, out m_oldText);
    }

    private void textbox_ActualThemeChanged(FrameworkElement sender, object args)
    {
        // Updating the formatting for theme change
        var formatter = new XamlTextFormatter(textbox);
        formatter.ApplyColors();
    }
}

public class XamlTextFormatter
{
    private RichEditBox m_richEditBox;
    public XamlTextFormatter(RichEditBox richEditBox)
    {
        m_richEditBox = richEditBox;
    }

    enum ZoneType
    {
        Unknown,
        OpenTag,
        EndTag,
        TagName,
        PropertyName,
        PropertyValue,
        Whitespace,
        Comment,
        Content
    }

    public void ApplyColors()
    {
        var doc = m_richEditBox.Document;
        doc.BeginUndoGroup();

        string rebText;
        doc.GetText(TextGetOptions.None, out rebText);

        var startIndex = 0;
        var currentZoneType = ZoneType.Unknown;
        bool inATag = false;
        for (var currIndex = 0; currIndex < rebText.Length; currIndex++)
        {
            if (char.IsWhiteSpace(rebText[currIndex]))
            {
                if (currentZoneType != ZoneType.Whitespace)
                {
                    UpdateZone(startIndex, currIndex, currentZoneType);
                    startIndex = currIndex;
                    currentZoneType = ZoneType.Whitespace;
                }
                // else already whitespace, so continue
            }
            else if (rebText[currIndex] == '<')
            {
                UpdateZone(startIndex, currIndex, currentZoneType);
                startIndex = currIndex;
                if (rebText.Substring(currIndex).StartsWith("<!--"))
                {
                    currentZoneType = ZoneType.Comment;
                    currIndex += 3;

                    // Look for end of comment
                    var endIndex = rebText.IndexOf("-->", currIndex + 1);
                    if (endIndex >= 0)
                    {
                        currIndex = endIndex + 2;
                    }
                }
                else
                {
                    currentZoneType = ZoneType.OpenTag;
                    inATag = true;
                }
            }
            else if (rebText[currIndex] == '/')
            {
                // This could either be "</" at the start a tag like "</StackPanel>", or "/>" at
                // the end of a tag like "<Button/>".
                if (currentZoneType != ZoneType.OpenTag && currentZoneType != ZoneType.EndTag)
                {
                    UpdateZone(startIndex, currIndex, currentZoneType);
                    startIndex = currIndex;
                    currentZoneType = ZoneType.EndTag;
                    inATag = false;
                }
                // else already end tag, so continue
            }
            else if (rebText[currIndex] == '>')
            {
                if (currentZoneType != ZoneType.EndTag)
                {
                    UpdateZone(startIndex, currIndex, currentZoneType);
                    startIndex = currIndex;
                    currentZoneType = ZoneType.EndTag;
                    inATag = false;
                }
                // else already end tag, so continue
            }
            else
            {
                if (currentZoneType == ZoneType.OpenTag)
                {
                    UpdateZone(startIndex, currIndex, currentZoneType);
                    startIndex = currIndex;
                    currentZoneType = ZoneType.TagName;
                }
                else if (currentZoneType == ZoneType.PropertyName && rebText[currIndex] == '=')
                {
                    UpdateZone(startIndex, currIndex, currentZoneType);
                    startIndex = currIndex;
                    currentZoneType = ZoneType.PropertyValue;

                    // Look for the end of the value
                    if (currIndex < rebText.Length - 2 && rebText[currIndex + 1] == '"')
                    {
                        var endIndex = rebText.IndexOf("\"", currIndex + 2);
                        if (endIndex >= 0)
                        {
                            currIndex = endIndex;
                        }
                    }
                }
                else if (currentZoneType != ZoneType.TagName && currentZoneType != ZoneType.PropertyName
                     && currentZoneType != ZoneType.PropertyValue && inATag)
                {
                    UpdateZone(startIndex, currIndex, currentZoneType);
                    startIndex = currIndex;
                    currentZoneType = ZoneType.PropertyName;
                }
                else if (!inATag)
                {
                    UpdateZone(startIndex, currIndex, currentZoneType);
                    startIndex = currIndex;
                    currentZoneType = ZoneType.Content;
                }
                // else already ...other..., so continue
            }
        }
        doc.EndUndoGroup();
    }

    // endIndexExclusive is the index just after the zone.
    private void UpdateZone(int startIndex, int endIndexExclusive, ZoneType zoneType)
    {
        if (startIndex >= endIndexExclusive)
        {
            return;
        }

        bool lightTheme = m_richEditBox.ActualTheme != ElementTheme.Dark;

        var doc = m_richEditBox.Document;
        var range = doc.GetRange(startIndex, endIndexExclusive);
        Windows.UI.Color foregroundColor = lightTheme ? Microsoft.UI.Colors.Black : Microsoft.UI.Colors.White;
        switch (zoneType)
        {
            case ZoneType.OpenTag:
            case ZoneType.EndTag:
                foregroundColor = lightTheme ? Microsoft.UI.Colors.Blue : Microsoft.UI.Colors.Gray;
                break;

            case ZoneType.TagName:
                foregroundColor = lightTheme ? Microsoft.UI.Colors.Brown : Microsoft.UI.Colors.White;
                break;

            case ZoneType.PropertyName:
                foregroundColor = lightTheme ? Microsoft.UI.Colors.Red : Microsoft.UI.Colors.LightSkyBlue;
                break;

            case ZoneType.PropertyValue:
                foregroundColor = lightTheme ? Microsoft.UI.Colors.Blue : Microsoft.UI.Colors.DodgerBlue;
                break;

            case ZoneType.Whitespace:
            case ZoneType.Content:
                foregroundColor = lightTheme ? Microsoft.UI.Colors.Black : Microsoft.UI.Colors.White;
                break;

            case ZoneType.Comment:
                foregroundColor = lightTheme ? Microsoft.UI.Colors.Green : Microsoft.UI.Colors.LimeGreen;
                break;
        }

        range.CharacterFormat.ForegroundColor = foregroundColor;
    }
}
