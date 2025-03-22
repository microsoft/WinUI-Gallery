using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
        switch (name)
        {
            case "AnimatedBackVisualSource": return new AnimatedBackVisualSource();
            case "AnimatedChevronDownSmallVisualSource": return new AnimatedChevronDownSmallVisualSource();
            case "AnimatedChevronRightDownSmallVisualSource": return new AnimatedChevronRightDownSmallVisualSource();
            case "AnimatedChevronUpDownSmallVisualSource": return new AnimatedChevronUpDownSmallVisualSource();
            case "AnimatedFindVisualSource": return new AnimatedFindVisualSource();
            case "AnimatedGlobalNavigationButtonVisualSource": return new AnimatedGlobalNavigationButtonVisualSource();
            case "AnimatedSettingsVisualSource": return new AnimatedSettingsVisualSource();
            default: return null;
        }
    }
}
