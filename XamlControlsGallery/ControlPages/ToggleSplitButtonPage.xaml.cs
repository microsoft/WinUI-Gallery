using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ToggleSplitButtonPage : Page
    {
        private MarkerType _type = MarkerType.Bullet;
        public ToggleSplitButtonPage()
        {
            this.InitializeComponent();
        }

        private void BulletButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBullet = (Button)sender;
            SymbolIcon symbol = (SymbolIcon)clickedBullet.Content;

            if (symbol.Symbol == Symbol.List)
            {
                _type = MarkerType.Bullet;
                mySymbolIcon.Symbol = Symbol.List;
                myListButton.SetValue(AutomationProperties.NameProperty, "Bullets");
            }
            else if (symbol.Symbol == Symbol.Bullets)
            {
                _type = MarkerType.UppercaseRoman;
                mySymbolIcon.Symbol = Symbol.Bullets;
                myListButton.SetValue(AutomationProperties.NameProperty, "Roman Numerals");
            }
            myRichEditBox.Document.Selection.ParagraphFormat.ListType = _type;

            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            myRichEditBox.Focus(FocusState.Keyboard);
        }

        private void MyListButton_IsCheckedChanged(Microsoft.UI.Xaml.Controls.ToggleSplitButton sender, Microsoft.UI.Xaml.Controls.ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            if (sender.IsChecked)
            {
                //add bulleted list
                myRichEditBox.Document.Selection.ParagraphFormat.ListType = _type;
            }
            else
            {
                //remove bulleted list
                myRichEditBox.Document.Selection.ParagraphFormat.ListType = MarkerType.None;
            }
        }
    }
}
