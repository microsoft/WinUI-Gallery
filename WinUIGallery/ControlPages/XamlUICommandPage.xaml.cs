using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinUIGallery.Helper;

namespace WinUIGallery.ControlPages
{
    public sealed partial class XamlUICommandPage : Page
    {
        public XamlUICommandPage()
        {
            this.InitializeComponent();
        }

        private void CustomXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            XamlUICommandOutput.Text = "You fired the custom command";
            UIHelper.AnnounceActionForAccessibility(CustomButton, "Activated custom XAML UI Command", "CustomXamlUICommandNotificationActivityId");
        }
    }
}
