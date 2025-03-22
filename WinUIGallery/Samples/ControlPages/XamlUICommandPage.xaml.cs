using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class XamlUICommandPage : Page
{
    public XamlUICommandPage()
    {
        InitializeComponent();
    }

    private void CustomXamlUICommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        XamlUICommandOutput.Text = "You fired the custom command";
        UIHelper.AnnounceActionForAccessibility(CustomButton, "Activated custom XAML UI Command", "CustomXamlUICommandNotificationActivityId");
    }
}
