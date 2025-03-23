using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class MenuBarPage : Page
{
    public MenuBarPage()
    {
        InitializeComponent();
    }

    private void OnElementClicked(object sender, RoutedEventArgs e)
    {
        var selectedFlyoutItem = sender as MenuFlyoutItem;
        string exampleNumber = selectedFlyoutItem.Name.Substring(0, 1);
        if(exampleNumber == "o")
        {
            SelectedOptionText.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
        }
        else if(exampleNumber == "t")
        {
            SelectedOptionText1.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
        }
        else if(exampleNumber == "z")
        {
            SelectedOptionText2.Text = "You clicked: " + (sender as MenuFlyoutItem).Text;
        }
    }
}
