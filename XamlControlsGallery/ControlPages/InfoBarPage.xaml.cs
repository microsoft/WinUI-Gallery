using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                DisplayButton.Value = @"<muxc:InfoBar.ActionButton>
            <Button Content=""Action"" Click=""InfoBarButton_Click"" />
    </muxc:InfoBar.ActionButton> ";

            }
            else if (ActionButtonComboBox.SelectedIndex == 2) // hyperlink
            {
                var link = new HyperlinkButton();
                link.NavigateUri = new Uri("http://www.microsoft.com/");
                link.Content = "Informational link";
                TestInfoBar2.ActionButton = link;
                DisplayButton.Value = @"<muxc:InfoBar.ActionButton>
            <HyperlinkButton Content=""Informational link"" NavigateUri=""https://www.example.com"" />
    </muxc:InfoBar.ActionButton>";
            }
        }
    }

    
}
