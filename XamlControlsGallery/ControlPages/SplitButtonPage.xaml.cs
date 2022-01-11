using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace AppUIBasics.ControlPages
{
    public sealed partial class SplitButtonPage : Page
    {
        private Color currentColor = Colors.Green;

        public SplitButtonPage()
        {
            this.InitializeComponent();

            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
            myRichEditBox.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tempor commodo ullamcorper a lacus.");
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rect = (Rectangle)e.ClickedItem;
            var color = ((SolidColorBrush)rect.Fill).Color;
            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
            CurrentColor.Background = new SolidColorBrush(color);

            //if (myColorButton.Flyout.IsOpen)
            //{
            //    myColorButton.Flyout.Hide
            //}
            myColorButton.Flyout.Hide();
            myRichEditBox.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            currentColor = color;
        }

        private void RevealColorButton_Click(object sender, RoutedEventArgs e)
        {
            myColorButtonReveal.Flyout.Hide();
        }
        private void myColorButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            var border = (Border)sender.Content;
            var color = ((Windows.UI.Xaml.Media.SolidColorBrush)border.Background).Color;

            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
            currentColor = color;
        }

        private void MyRichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if(myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor != currentColor)
            {
                myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
            }
        }
    }
}
