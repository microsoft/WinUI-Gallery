using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ClipboardPage : Page
    {

        private string textToCopy = "";

        public ClipboardPage()
        {
            this.InitializeComponent();
            richEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "This text will be copied to the clipboard.");

        }

        private void CopyText_Click(object sender, RoutedEventArgs args)
        {
            richEditBox.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out textToCopy);
            var package = new DataPackage();
            package.SetText(textToCopy);
            Clipboard.SetContent(package);

            VisualStateManager.GoToState(this, "ConfirmationClipboardVisible", false);
            Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            // Automatically hide the confirmation text after 2 seconds
            if (dispatcherQueue != null)
            {
                dispatcherQueue.TryEnqueue(async () =>
                {
                    await Task.Delay(2000);
                    VisualStateManager.GoToState(this, "ConfirmationClipboardCollapsed", false);
                });
            }

        }

        private async void PasteText_Click(object sender, RoutedEventArgs args)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                var text = await package.GetTextAsync();
                PasteClipboard2.Text = text;
            }

        }

    }
}
