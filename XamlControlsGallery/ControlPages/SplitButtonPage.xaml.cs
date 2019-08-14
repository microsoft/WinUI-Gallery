using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class SplitButtonPage : Page
    {
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
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = ((Windows.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
            CurrentColor.Fill = new SolidColorBrush(color);

            myColorButton.Flyout.Hide();
            myRichEditBox.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        private void myColorButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)sender.Content;
            var color = ((Windows.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
        }
    }
}
