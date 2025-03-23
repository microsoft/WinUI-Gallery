using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;

namespace WinUIGallery.ControlPages;

public sealed partial class AnimatedIconPage : Page
{
    public AnimatedIconPage()
    {
        InitializeComponent();
    }

    private void Button_PointerEntered(object sender, PointerRoutedEventArgs e) => AnimatedIcon.SetState(SearchAnimatedIcon, "PointerOver");

    private void Button_PointerExited(object sender, PointerRoutedEventArgs e) => AnimatedIcon.SetState(SearchAnimatedIcon, "Normal");

    public static IAnimatedVisualSource2 GetAnimationSourceFromString(object selection)
    { 
        string name = (string)selection;
        return name switch
        {
            "AnimatedBackVisualSource" => new AnimatedBackVisualSource(),
            "AnimatedChevronDownSmallVisualSource" => new AnimatedChevronDownSmallVisualSource(),
            "AnimatedChevronRightDownSmallVisualSource" => new AnimatedChevronRightDownSmallVisualSource(),
            "AnimatedChevronUpDownSmallVisualSource" => new AnimatedChevronUpDownSmallVisualSource(),
            "AnimatedFindVisualSource" => new AnimatedFindVisualSource(),
            "AnimatedGlobalNavigationButtonVisualSource" => new AnimatedGlobalNavigationButtonVisualSource(),
            "AnimatedSettingsVisualSource" => new AnimatedSettingsVisualSource(),
            _ => null,
        };
    }
}
