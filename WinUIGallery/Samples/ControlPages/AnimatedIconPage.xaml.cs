// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
using Microsoft.UI.Xaml.Input;

namespace WinUIGallery.ControlPages;

public sealed partial class AnimatedIconPage : Page
{
    public AnimatedIconPage()
    {
        this.InitializeComponent();
    }

    private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        AnimatedIcon.SetState(this.SearchAnimatedIcon, "PointerOver");
    }

    private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        AnimatedIcon.SetState(this.SearchAnimatedIcon, "Normal");
    }

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
