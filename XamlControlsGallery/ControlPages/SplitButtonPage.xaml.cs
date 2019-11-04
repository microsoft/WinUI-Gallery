using Microsoft.UI;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class SplitButtonPage : Page
    {
        private Windows.UI.Color currentColor = Colors.Black;

        // String used to restore the colors when the focus gets reenabled
        // See #144 for more info https://github.com/microsoft/Xaml-Controls-Gallery/issues/144 
        // (which also applies to this RichEditBox)
        private string LastFormattedText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tempor commodo ullamcorper a lacus.";
        public SplitButtonPage()
        {
            this.InitializeComponent();
            myRichEditBox.Document.SetText(Windows.UI.Text.TextSetOptions.None,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tempor commodo ullamcorper a lacus.");
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Extract the color of the button that was clicked.
            Button clickedColor = (Button)sender;
            var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = ((Microsoft.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
            CurrentColor.Fill = new SolidColorBrush(color);

            myColorButton.Flyout.Hide();
            myRichEditBox.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
            currentColor = color;
        }
        
        private void RevealColorButton_Click(object sender, RoutedEventArgs e)
        {
            myColorButtonReveal.Flyout.Hide();
        }

        private void myColorButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)sender.Content;
            var color = ((Microsoft.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
            currentColor = color;
        }

        private void MyRichEditBox_TextChanging(object sender, RichEditBoxTextChangingEventArgs e)
        {
            // Hitting control+b and similiar commands my overwrite the color,
            // which result to black text on black background when losing focus on dark theme.
            // Solution: check if text actually changed
            if (e.IsContentChanging)
            {
                myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
            }
        }


        private void MyRichEditBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // reset colors to correct defaults for Focused state
            ITextRange documentRange = myRichEditBox.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];
            SolidColorBrush foreground = (SolidColorBrush)App.Current.Resources["TextControlForegroundFocused"];

            myRichEditBox.Document.ApplyDisplayUpdates();

            if (background != null && foreground != null)
            {
                documentRange.CharacterFormat.BackgroundColor = background.Color;
            }
            // saving selection span
            var caretPosition = myRichEditBox.Document.Selection.GetIndex(TextRangeUnit.Character) - 1;
            if (caretPosition <= 0)
            {
                // User has not entered text, prevent invalid values and just set index to 1
                caretPosition = 1;
            }
            var selectionLength = myRichEditBox.Document.Selection.Length;
            // restoring text styling, unintentionally sets caret position at beginning of text
            myRichEditBox.Document.SetText(TextSetOptions.FormatRtf, LastFormattedText);
            // restoring selection position
            myRichEditBox.Document.Selection.SetIndex(TextRangeUnit.Character, caretPosition, false);
            myRichEditBox.Document.Selection.SetRange(caretPosition, caretPosition + selectionLength);
            // Editor might have gained focus because user changed color.
            // Change selection color
            // Note that only way to regain with selection containing text is using the change color button
            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
        }

        private void MyRichEditBox_LosingFocus(object sender, RoutedEventArgs e)
        {
            myRichEditBox.Document.GetText(TextGetOptions.FormatRtf, out LastFormattedText);
        }
    }
}
