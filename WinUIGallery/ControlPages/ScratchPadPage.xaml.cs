using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Media.Capture.Frames;
using Windows.Media.Capture;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using System.ComponentModel;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Markup;
using System.Xml;
using Windows.Storage;

namespace WinUIGallery.ControlPages
{
    public sealed partial class ScratchPadPage : Page
    {
        public ScratchPadPage()
        {
            this.InitializeComponent();

            var xamlStr = ReadScratchPadXAMLinLocalSettings();
            if (xamlStr != null)
            {
                m_oldText = xamlStr;
                textbox.TextDocument.SetText(Microsoft.UI.Text.TextSetOptions.None, m_oldText);
                var x = new XamlTextModel(textbox);
                x.ApplyColors();
            }
        }

        public string ReadScratchPadXAMLinLocalSettings()
        {
            var appData = Windows.Storage.ApplicationData.Current;
            if (appData.LocalSettings.Containers.ContainsKey("ScratchPad"))
            {
                var scratchPadContainer = appData.LocalSettings.CreateContainer("ScratchPad", Windows.Storage.ApplicationDataCreateDisposition.Existing);
                if (scratchPadContainer != null && scratchPadContainer.Values.ContainsKey("ScratchPadXAML"))
                {
                    // String values are limited to to 4K characters. Use a composite value to support longer.
                    var compositeStr = scratchPadContainer.Values["ScratchPadXAML"] as ApplicationDataCompositeValue;
                    var xamlStr = "";
                    int count = (int)compositeStr["count"];
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
            var appData = Windows.Storage.ApplicationData.Current;
            var scratchPadContainer = appData.LocalSettings.CreateContainer("ScratchPad", Windows.Storage.ApplicationDataCreateDisposition.Always);
            // String values are limited to to 4K characters. Use a composite value to support longer.
            var compositeStr = new ApplicationDataCompositeValue();
            int count = 0;
            while (xamlStr.Length > 0)
            {
                var len = Math.Min(xamlStr.Length, 4000);
                compositeStr[count.ToString()] = xamlStr.Substring(0, len);
                count++;
                xamlStr = xamlStr.Substring(len);
            }
            compositeStr["count"] = count;
            scratchPadContainer.Values["ScratchPadXAML"] = compositeStr;
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
            textbox.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out newText);
            System.Diagnostics.Debug.WriteLine("new text: " + newText);

            SaveScratchPadXAMLinLocalSettings(newText);

            // TODO: Strip out x:Bind -- maybe just convert it to spaces?
            try
            {
                log.Text = ""; // Clear the log before loading

                var xml = AddXmlNamespace(newText);

                var element = (UIElement)XamlReader.Load(xml);
                scratchPad.Content = element;
                log.Text = "Load successful.";
            }
            catch (Exception ex)
            {
                log.Text = ex.Message + "\n" + log.Text;
            }
            log.Opacity = 1.0;
        }

        private void InsertTextboxText(string str, bool setCursorAfterInsertedStr)
        {
#if true
            var selectionStart = textbox.TextDocument.Selection.StartPosition;
            var range = textbox.TextDocument.GetRange(selectionStart, selectionStart);
            m_lastChangeFromTyping = false;
            range.Text = str;
            textbox.TextDocument.Selection.StartPosition = selectionStart + (setCursorAfterInsertedStr ? str.Length : 0);
#else
            textbox.TextDocument.Selection.TypeText(str);
#endif
        }

        private string GetTextboxTextPreviousLine()
        {
            string newText;
            textbox.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out newText);
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

        private async void HandleEnter()
        {
            textbox.TextDocument.BeginUndoGroup();
            string previousLine = GetTextboxTextPreviousLine();
            string indentStr = previousLine.Substring(0, previousLine.Length - previousLine.TrimStart().Length);
            // TODO: Ident further if this is the start of a child tag or property.
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

            try
            {
                string text;
                textbox.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out text);
                var reader = new System.Xml.XmlTextReader(GenerateStreamFromString(AddXmlNamespace(text)));
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            System.Diagnostics.Debug.WriteLine("Start Element " + reader.Name);
                            break;
                        case XmlNodeType.Text:
                            System.Diagnostics.Debug.WriteLine("Text Node: " +
                                     await reader.GetValueAsync());
                            break;
                        case XmlNodeType.EndElement:
                            System.Diagnostics.Debug.WriteLine("End Element " + reader.Name);
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine("Other node " + reader.NodeType + " with value " +
                                     reader.Value);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                log.Text += e.ToString();
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
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
                        textbox.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out text);

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

                                range.Move(Microsoft.UI.Text.TextRangeUnit.Paragraph, 1);
                                while (range.StartPosition < selectionEnd)
                                {
                                    range.Text = "    ";
                                    selectionEnd += 4;
                                    range.Move(Microsoft.UI.Text.TextRangeUnit.Paragraph, 1);
                                }
                            }
                            else // Unindent
                            {
                                bool firstLine = true;
                                while (range.StartPosition < selectionEnd)
                                {
                                    range.MoveEnd(Microsoft.UI.Text.TextRangeUnit.Character, 4);
                                    var numWhitespace = range.Text.Count(char.IsWhiteSpace);
                                    range = textbox.TextDocument.GetRange(range.StartPosition, range.StartPosition + numWhitespace);
                                    range.Text = "";
                                    if (firstLine)
                                    {
                                        firstLine = false;
                                        selectionStart -= numWhitespace;
                                    }
                                    selectionEnd -= numWhitespace;
                                    range.Move(Microsoft.UI.Text.TextRangeUnit.Paragraph, 1);
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
                case Windows.System.VirtualKey.F5:
                    LoadContent();
                    break;

                case Windows.System.VirtualKey.Enter:
                    HandleEnter();
                    break;
            }

        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            LoadContent();

            m_lastChangeFromTyping = false;
            var x = new XamlTextModel(textbox);
            x.ApplyColors();
        }

        bool m_lastChangeFromTyping = false;
        string m_oldText = "";
        private void textbox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (textbox.TextDocument.Selection.Length == 0 && m_lastChangeFromTyping)
            {
                if (log.Text == "Load successful.")
                {
                    log.Text = "";
                }
                else
                {
                    log.Opacity = 0.5;
                }

                string newText;
                textbox.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out newText);
                var selectionIndex = textbox.TextDocument.Selection.StartPosition;
                //if (selectionIndex >= 2 && newText[selectionIndex-1] == '>' && newText[selectionIndex-2] != '/'
                //    && (m_oldText != null && m_oldText.Length > selectionIndex-1 && m_oldText[selectionIndex-1] != '>'))
                if (newText.Length == m_oldText.Length + 1)
                {
                    // Added just one character
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
                                InsertTextboxText("</" + tagName + ">", false);
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
            textbox.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out m_oldText);
        }
    }

    public class XamlTextModel
    {
        //string m_text;
        RichEditBox m_richEditBox;
        public XamlTextModel(/*string text,*/ RichEditBox richEditBox)
        {
            //m_text = text;
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
            //var range = doc.GetRange(0, 1);
            string rebText;
            doc.GetText(Microsoft.UI.Text.TextGetOptions.None, out rebText);

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
            //
            var doc = m_richEditBox.Document;
            var range = doc.GetRange(startIndex, endIndexExclusive);
            Windows.UI.Color foregroundColor = Microsoft.UI.Colors.Black;
            switch (zoneType)
            {
                case ZoneType.OpenTag:
                case ZoneType.EndTag:
                    foregroundColor = Microsoft.UI.Colors.Blue;
                    break;

                case ZoneType.TagName:
                    foregroundColor = Microsoft.UI.Colors.Brown;
                    break;

                case ZoneType.PropertyName:
                    foregroundColor = Microsoft.UI.Colors.Red;
                    break;

                case ZoneType.PropertyValue:
                    foregroundColor = Microsoft.UI.Colors.Blue;
                    //range.CharacterFormat.Underline = Microsoft.UI.Text.UnderlineType.Wave;
                    break;

                case ZoneType.Whitespace:
                case ZoneType.Content:
                    foregroundColor = Microsoft.UI.Colors.Black;
                    break;

                case ZoneType.Comment:
                    foregroundColor = Microsoft.UI.Colors.Green;
                    break;
            }

            range.CharacterFormat.ForegroundColor = foregroundColor;
        }
    }
}
