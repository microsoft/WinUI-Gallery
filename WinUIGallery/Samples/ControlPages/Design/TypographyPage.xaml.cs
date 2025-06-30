// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class TypographyPage : Page
{
    public TypographyPage()
    {
        this.InitializeComponent();
    }

    private void ShowTypographyButtonClick1(object sender, RoutedEventArgs e)
    {
        ShowTypographyInfoTooltip1.IsOpen = !ShowTypographyInfoTooltip1.IsOpen;
    }

    private void ShowTypographyButtonClick2(object sender, RoutedEventArgs e)
    {
        ShowTypographyInfoTooltip2.IsOpen = !ShowTypographyInfoTooltip2.IsOpen;
    }

    private void ShowTypographyButtonClick3(object sender, RoutedEventArgs e)
    {
        ShowTypographyInfoTooltip3.IsOpen = !ShowTypographyInfoTooltip3.IsOpen;
    }

    private void ShowTypographyButtonClick4(object sender, RoutedEventArgs e)
    {
        ShowTypographyInfoTooltip4.IsOpen = !ShowTypographyInfoTooltip4.IsOpen;
    }
    private void ShowTypographyButtonClick5(object sender, RoutedEventArgs e)
    {
        ShowTypographyInfoTooltip5.IsOpen = !ShowTypographyInfoTooltip5.IsOpen;
    }
}
