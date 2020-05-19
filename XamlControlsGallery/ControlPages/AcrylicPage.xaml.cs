using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
    }
}
