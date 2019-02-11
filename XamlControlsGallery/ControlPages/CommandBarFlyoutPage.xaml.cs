using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class CommandBarFlyoutPage : Page
    {
        public CommandBarFlyoutPage()
        {
            this.InitializeComponent();
        }

        private void OnElementClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

        private void MyImageButton_ContextRequested(Windows.UI.Xaml.UIElement sender, ContextRequestedEventArgs args)
        {   
            // always show a context menue in standard mode
            ShowMenu(false);
        }

        private void MyImageButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ShowMenu((sender as Button).IsPointerOver);
        }
    }
}
