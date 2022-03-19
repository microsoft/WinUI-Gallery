using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AcrylicPage : Page
    {
        public AcrylicPage()
        {
            this.InitializeComponent();
            Loaded += AcrylicPage_Loaded;
        }

        private void AcrylicPage_Loaded(object sender, RoutedEventArgs e)
        {
            ColorSelector.SelectedIndex = ColorSelectorInApp.SelectedIndex = 0;
            FallbackColorSelector.SelectedIndex = FallbackColorSelectorInApp.SelectedIndex = 0;
            OpacitySlider.Value = OpacitySliderInApp.Value = OpacitySliderLumin.Value = 0.8;
            LuminositySlider.Value = 0.8;
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Rectangle shape = (sender == OpacitySliderInApp) ? CustomAcrylicShapeInApp : CustomAcrylicShape;

            if (sender == OpacitySliderLumin)
                shape = CustomAcrylicShapeLumin;

            ((Microsoft.UI.Xaml.Media.AcrylicBrush)shape.Fill).TintOpacity = e.NewValue;
        }

        private void ColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Rectangle shape = (sender == ColorSelectorInApp) ? CustomAcrylicShapeInApp : CustomAcrylicShape;
            ((Microsoft.UI.Xaml.Media.AcrylicBrush)shape.Fill).TintColor = ((SolidColorBrush)e.AddedItems.First()).Color;
        }

        private void FallbackColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Rectangle shape = (sender == FallbackColorSelectorInApp) ? CustomAcrylicShapeInApp : CustomAcrylicShape;
            ((Microsoft.UI.Xaml.Media.AcrylicBrush)shape.Fill).FallbackColor = ((SolidColorBrush)e.AddedItems.First()).Color;
        }

        private void LuminositySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Rectangle shape = CustomAcrylicShapeLumin;
            ((Microsoft.UI.Xaml.Media.AcrylicBrush)shape.Fill).TintLuminosityOpacity = e.NewValue;
        }

        private void Grid_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            var requestedElement = sender as FrameworkElement;

            if (args.TryGetPosition(requestedElement, out Point point))
            {
                //sharedFlyout.ShowAt(requestedElement, point);
            }
            else
            {
                //sharedFlyout.ShowAt(requestedElement);
            }
        }
    }
}
