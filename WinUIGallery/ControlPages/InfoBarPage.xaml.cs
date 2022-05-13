using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class InfoBarPage : Page
    {
        public InfoBarPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayMessage.Value = "A long essential app message...";
            DisplayButton.Value = string.Empty;
        }

        private void SeverityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string severityName = e.AddedItems[0].ToString();

            switch (severityName)
            {
                case "Error":
                    TestInfoBar1.Severity = InfoBarSeverity.Error;
                    break;

                case "Warning":
                    TestInfoBar1.Severity = InfoBarSeverity.Warning;
                    break;

                case "Success":
                    TestInfoBar1.Severity = InfoBarSeverity.Success;
                    break;

                case "Informational":
                default:
                    TestInfoBar1.Severity = InfoBarSeverity.Informational;
                    break;
            }
        }

        private void MessageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestInfoBar2 == null) return;

            if (MessageComboBox.SelectedIndex == 0) // short
            {
                string shortMessage = "A short essential app message.";
                TestInfoBar2.Message = shortMessage;
                DisplayMessage.Value = shortMessage;
            }
            else if (MessageComboBox.SelectedIndex == 1) //long
            {
                TestInfoBar2.Message = @"A long essential app message for your users to be informed of, acknowledge, or take action on. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin dapibus dolor vitae justo rutrum, ut lobortis nibh mattis. Aenean id elit commodo, semper felis nec.";
                DisplayMessage.Value = "A long essential app message...";
            }
        }

        private void ActionButtonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestInfoBar2 == null) return;

            if (ActionButtonComboBox.SelectedIndex == 0) // none
            {
                TestInfoBar2.ActionButton = null;
                DisplayButton.Value = string.Empty;
            }
            else if (ActionButtonComboBox.SelectedIndex == 1) // button
            {
                var button = new Button();
                button.Content = "Action";
                TestInfoBar2.ActionButton = button;
                DisplayButton.Value = @"<InfoBar.ActionButton>
            <Button Content=""Action"" Click=""InfoBarButton_Click"" />
    </InfoBar.ActionButton> ";

            }
            else if (ActionButtonComboBox.SelectedIndex == 2) // hyperlink
            {
                var link = new HyperlinkButton();
                link.NavigateUri = new Uri("http://www.microsoft.com/");
                link.Content = "Informational link";
                TestInfoBar2.ActionButton = link;
                DisplayButton.Value = @"<InfoBar.ActionButton>
            <HyperlinkButton Content=""Informational link"" NavigateUri=""https://www.example.com"" />
    </InfoBar.ActionButton>";
            }
        }
    }

    
}
