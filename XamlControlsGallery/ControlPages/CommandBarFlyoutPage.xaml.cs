using Windows.Foundation.Metadata;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class CommandBarFlyoutPage : Page
    {
        public CommandBarFlyoutPage()
        {
            this.InitializeComponent();
        }

        private void OnElementClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Do custom logic
            SelectedOptionText.Text = "You clicked: " + (sender as AppBarButton).Label;
        }

        private void ShowMenu(bool isTransient)
        {
            if(ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                FlyoutShowOptions myOption = new FlyoutShowOptions();
                myOption.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
                myOption.Placement = FlyoutPlacementMode.RightEdgeAlignedTop;
                CommandBarFlyout1.ShowAt(Image1, myOption);
            }
            else
            {
                CommandBarFlyout1.ShowAt(Image1);
            }
        }

        private void MyImageButton_ContextRequested(Microsoft.UI.Xaml.UIElement sender, ContextRequestedEventArgs args)
        {   
            // always show a context menu in standard mode
            ShowMenu(false);
        }

        private void MyImageButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ShowMenu((sender as Button).IsPointerOver);
        }
    }
}
