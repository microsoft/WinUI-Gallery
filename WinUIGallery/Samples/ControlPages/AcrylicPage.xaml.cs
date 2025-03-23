using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using WinUIGallery.Pages;

namespace WinUIGallery.ControlPages;

public sealed partial class AcrylicPage : Page
{
    public AcrylicPage()
    {
        InitializeComponent();
        Loaded += AcrylicPage_Loaded;
    }

    private void AcrylicPage_Loaded(object sender, RoutedEventArgs e)
    {
        ColorSelectorInApp.SelectedIndex = 0;
        FallbackColorSelectorInApp.SelectedIndex = 0;
        OpacitySliderInApp.Value = OpacitySliderLumin.Value = 0.8;
        LuminositySlider.Value = 0.8;
    }

    private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        Rectangle shape = CustomAcrylicShapeInApp;
        if ((Slider)sender == OpacitySliderLumin)
            shape = CustomAcrylicShapeLumin;

        ((AcrylicBrush)shape.Fill).TintOpacity = e.NewValue;
    }

    private void ColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Rectangle shape = CustomAcrylicShapeInApp;
        ((AcrylicBrush)shape.Fill).TintColor = ((SolidColorBrush)e.AddedItems.First()).Color;
    }

    private void FallbackColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Rectangle shape = CustomAcrylicShapeInApp;
        ((AcrylicBrush)shape.Fill).FallbackColor = ((SolidColorBrush)e.AddedItems.First()).Color;
    }

    private void LuminositySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        Rectangle shape = CustomAcrylicShapeLumin;
        ((AcrylicBrush)shape.Fill).TintLuminosityOpacity = e.NewValue;
    }

    private void SystemBackdropLink_Click(Hyperlink sender, HyperlinkClickEventArgs args) => NavigationRootPage.GetForElement(this).Navigate(typeof(ItemPage), "SystemBackdrops");
}
