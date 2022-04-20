using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Threading.Tasks;

namespace AppUIBasics.ControlPages
{
    public sealed partial class SplitButtonPage : Page
    {
        private Windows.UI.Color currentColor = Colors.Green;

        public SplitButtonPage()
        {
            this.InitializeComponent();

            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
            myRichEditBox.Document.Selection.SetText(Microsoft.UI.Text.TextSetOptions.None,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tempor commodo ullamcorper a lacus.");
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rect = (Rectangle)e.ClickedItem;
            var color = ((SolidColorBrush)rect.Fill).Color;
            myRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
            CurrentColor.Background = new SolidColorBrush(color);

            myRichEditBox.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
            currentColor = color;

            // Delay required to circumvent GridView bug: https://github.com/microsoft/microsoft-ui-xaml/issues/6350
            Task.Delay(10).ContinueWith(_ => myColorButton.Flyout.Hide(), TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        private void RevealColorButton_Click(object sender, RoutedEventArgs e)
        {
            myColorButtonReveal.Flyout.Hide();
        }

        private void myColorButton_Click(Microsoft.UI.Xaml.Controls.SplitButton sender, Microsoft.UI.Xaml.Controls.SplitButtonClickEventArgs args)
        {
            var border = (Border)sender.Content;
            var color = ((Microsoft.UI.Xaml.Media.SolidColorBrush)border.Background).Color;

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
