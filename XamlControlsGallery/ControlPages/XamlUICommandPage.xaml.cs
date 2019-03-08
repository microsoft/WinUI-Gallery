using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace AppUIBasics.ControlPages
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
        }
        private void ControlExample_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
