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
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System.Runtime.InteropServices;
using WinRT;
using System.Collections.ObjectModel;
using Windows.System;
using System.Text.RegularExpressions;

namespace WinUIGallery.ControlPages;

public sealed partial class RichEditBoxPage : Page
{
    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
    public static extern IntPtr GetActiveWindow();
    private Windows.UI.Color currentColor = Microsoft.UI.Colors.Green;

    public RichEditBoxPage()
    {
        this.InitializeComponent();
        MathEditor.TextDocument.SetMathMode(RichEditMathMode.MathOnly);
        MathModeDescription.Text = "Math mode enables users to have input automatically recognized and converted to MathML while being received. For example, \"4^2\" is converted to \"4\u00b2\", and \"\\pi\" is converted to \"\u03c0\".\r\nMath mode might change formatting (fonts), context menus, and other aspects of the input.";
        ObservableCollection<MathSymbol> symbols = GetSymbolsCollection();
        MathSymbolsItems.ItemsSource = symbols;
    }

    private void Menu_Opening(object sender, object e)
    {
        CommandBarFlyout myFlyout = sender as CommandBarFlyout;
        if (myFlyout != null && myFlyout.Target == REBCustom)
        {
            AppBarButton myButton = new AppBarButton
            {
                Command = new StandardUICommand(StandardUICommandKind.Share)
            };
            myFlyout.PrimaryCommands.Add(myButton);
        }
        else
        {
            CommandBarFlyout muxFlyout = sender as CommandBarFlyout;
            if (muxFlyout != null && muxFlyout.Target == REBCustom)
            {
                AppBarButton myButton = new AppBarButton
                {
                    Command = new StandardUICommand(StandardUICommandKind.Share)
                };
                muxFlyout.PrimaryCommands.Add(myButton);
            }
        }

    }

    private async void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        // Open a text file.
        FileOpenPicker open = new FileOpenPicker();
        open.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        open.FileTypeFilter.Add(".rtf");

        // When running on win32, FileOpenPicker needs to know the top-level hwnd via IInitializeWithWindow::Initialize.
        if (Window.Current == null)
        {
            IntPtr hwnd = GetActiveWindow();
            WinRT.Interop.InitializeWithWindow.Initialize(open, hwnd);
        }

        StorageFile file = await open.PickSingleFileAsync();

        if (file != null)
        {
            using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Load the file into the Document property of the RichEditBox.
                editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream);
            }
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        FileSavePicker savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };

        // Dropdown of file types the user can save the file as
        savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

        // Default file name if the user does not type one in or select a file to replace
        savePicker.SuggestedFileName = "New Document";

        // When running on win32, FileSavePicker needs to know the top-level hwnd via IInitializeWithWindow::Initialize.
        if (Window.Current == null)
        {
            IntPtr hwnd = GetActiveWindow();
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);
        }

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
                editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
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
        editor.Document.GetText(TextGetOptions.UseCrlf, out _);
        
        // reset colors to correct defaults for Focused state
        ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

        if (background != null)
        {
            documentRange.CharacterFormat.BackgroundColor = background.Color;
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

    private void Editor_TextChanged(object sender, RoutedEventArgs e)
    {
        if (editor.Document.Selection.CharacterFormat.ForegroundColor != currentColor)
        {
            editor.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
        }
    }

    private ObservableCollection<MathSymbol> GetSymbolsCollection()
    {
        var symbols = new ObservableCollection<MathSymbol>
        {
            new("\u221E", "Infinity", "\\infty", "U+221E"),
            new("\u2248", "Approximately Equal", "\\approx", "U+2248"),
            new("\u00D7", "Multiplication", "\\times", "U+00D7"),
            new("\u00F7", "Division", "\\div", "U+00F7"),
            new("\u00B1", "Plus-Minus", "\\pm", "U+00B1"),
            new("\u2213", "Minus-Plus", "\\mp", "U+2213"),
            new("\u22C5", "Dot Product", "\\cdot", "U+22C5"),
            new("\u2218", "Function Composition", "\\circ", "U+2218"),
            new("\u22C6", "Star Operator", "\\star", "U+22C6"),
            new("\u2022", "Bullet Operator", "\\bullet", "U+2022"),
            new("\u2295", "Direct Sum", "\\oplus", "U+2295"),
            new("\u2297", "Tensor Product", "\\otimes", "U+2297"),
            new("\u2229", "Intersection", "\\cap", "U+2229"),
            new("\u222A", "Union", "\\cup", "U+222A"),
            new("\u2216", "Set Difference", "\\setminus", "U+2216"),
            new("\u2205", "Empty Set", "\\emptyset", "U+2205"),
            new("\u2283", "Superset", "\\supset", "U+2283"),
            new("\u2282", "Subset", "\\subset", "U+2282"),
            new("\u2287", "Superset or Equal", "\\supseteq", "U+2287"),
            new("\u2286", "Subset or Equal", "\\subseteq", "U+2286"),
            new("\u22A5", "Perpendicular", "\\perp", "U+22A5"),
            new("\u2225", "Parallel", "\\parallel", "U+2225"),
            new("\u2226", "Not Parallel", "\\nparallel", "U+2226"),
            new("\u21D2", "Implies", "\\Rightarrow", "U+21D2"),
            new("\u21D0", "Left Implies", "\\Leftarrow", "U+21D0"),
            new("\u21D4", "If and Only If", "\\Leftrightarrow", "U+21D4"),
            new("\u2200", "For All", "\\forall", "U+2200"),
            new("\u2203", "There Exists", "\\exists", "U+2203"),
            new("\u2204", "Does Not Exist", "\\nexists", "U+2204"),
            new("\u00AC", "Logical NOT", "\\neg", "U+00AC"),
            new("\u2228", "Logical OR", "\\lor", "U+2228"),
            new("\u2227", "Logical AND", "\\land", "U+2227"),
            new("\u2234", "Therefore", "\\therefore", "U+2234"),
            new("\u2235", "Because", "\\because", "U+2235"),
            new("\u221D", "Proportional To", "\\propto", "U+221D"),
            new("\u221A", "Square Root", "\\surd", "U+221A"),
            new("X\u0305", "Overline X", "\\overline", "U+0305"),
            new("X\u0332", "Underline X", "\\underline", "U+0332"),
            new("\u03C0", "Pi", "\\pi", "U+03C0"),
            new("\u03B1", "Alpha", "\\alpha", "U+03B1"),
            new("\u03B2", "Beta", "\\beta", "U+03B2"),
            new("\u03B3", "Gamma", "\\gamma", "U+03B3"),
            new("\u03B4", "Delta", "\\delta", "U+03B4"),
            new("\u03B5", "Epsilon", "\\epsilon", "U+03B5"),
            new("\u03B6", "Zeta", "\\zeta", "U+03B6"),
            new("\u03B7", "Eta", "\\eta", "U+03B7"),
            new("\u03B8", "Theta", "\\theta", "U+03B8"),
            new("\u03B9", "Iota", "\\iota", "U+03B9"),
            new("\u03BA", "Kappa", "\\kappa", "U+03BA"),
            new("\u03BB", "Lambda", "\\lambda", "U+03BB"),
            new("\u03BC", "Mu", "\\mu", "U+03BC"),
            new("\u03BD", "Nu", "\\nu", "U+03BD"),
            new("\u03BE", "Xi", "\\xi", "U+03BE"),
            new("\u03BF", "Omicron", "\\omicron", "U+03BF"),
            new("\u03C1", "Rho", "\\rho", "U+03C1"),
            new("\u03C3", "Sigma", "\\sigma", "U+03C3"),
            new("\u03C4", "Tau", "\\tau", "U+03C4"),
            new("\u03C5", "Upsilon", "\\upsilon", "U+03C5"),
            new("\u03C6", "Phi", "\\phi", "U+03C6"),
            new("\u03C7", "Chi", "\\chi", "U+03C7"),
            new("\u03C8", "Psi", "\\psi", "U+03C8"),
            new("\u03C9", "Omega", "\\omega", "U+03C9"),
            new("\u2211", "Summation", "\\Sum", "U+2211"),
            new("\u222B", "Integral", "\\int", "U+222B"),
            new("\u222E", "Contour Integral", "\\oint", "U+222E"),
            new("\u220F", "Product", "\\prod", "U+220F"),
            new("\u2210", "Coproduct", "\\coprod", "U+2210")
        };

        return symbols;
    }

    private void SymbolDataTemplate_Loading(FrameworkElement sender, object args)
    {
        if (sender is Grid grid)
        {
            int index = MathSymbolsItems.GetElementIndex(grid);

            if (index % 2 == 0)
            {
                grid.Style = (Style)MathModeExample.Resources["ItemStyle1"];
            }
            else
            {
                grid.Style = (Style)MathModeExample.Resources["ItemStyle2"];
            }
        }
    }

    private void InsertSymbolBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button insertionBtn)
        {
            if (MathEditor == null) return;

            MathEditor.Focus(FocusState.Programmatic);

            var selection = MathEditor.Document.Selection;
            if (selection != null)
            {
                selection.Text = insertionBtn.Tag.ToString();
                SetCursorToEnd(MathEditor);
                SimulateKeyPress(VirtualKey.Space);
            }
        }

    }

    // Use Win32 API to simulate a key press inside RichEditBox
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private void SimulateKeyPress(VirtualKey key)
    {
        byte keyCode = (byte)key;

        // Simulate key press (keydown)
        keybd_event(keyCode, 0, 0, 0);

        // Simulate key release (keyup)
        keybd_event(keyCode, 0, 2, 0);
    }

    void SetCursorToEnd(RichEditBox richEditBox)
    {
        var doc = richEditBox.Document;
        doc.Selection.StartPosition = int.MaxValue;  // Move to the end
        doc.Selection.EndPosition = int.MaxValue;
    }
}

public class MathSymbol
{
    public string Value { get; set; }
    public string Name { get; set; }   
    public string Command { get; set; }
    public string Unicode { get; set; }

    public MathSymbol(string value, string name, string command, string unicode)
    {
        Value = value;
        Name = name;
        Command = command;
        Unicode = unicode;
    }
}
