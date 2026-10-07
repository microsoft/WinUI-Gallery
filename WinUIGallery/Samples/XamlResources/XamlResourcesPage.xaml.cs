// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinUIGallery.Pages;

namespace WinUIGallery.ControlPages;

public sealed partial class XamlResourcesPage : Page
{
    public XamlResourcesPage()
    {
        InitializeComponent();

        // One-time lookup: the brushes are resolved once and don't update when the theme changes.
        OneTimeLookupBorder.Background = (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
        OneTimeLookupText.Foreground = (Brush)Application.Current.Resources["TextOnAccentFillColorPrimaryBrush"];

        // Theme resource binding: the brushes are re-resolved whenever the element's ActualTheme changes.
        ThemeResourceBindingBorder.SetThemeResourceBinding(Border.BackgroundProperty, "AccentFillColorDefaultBrush");
        ThemeResourceBindingText.SetThemeResourceBinding(TextBlock.ForegroundProperty, "TextOnAccentFillColorPrimaryBrush");
    }

    private void SwitchThemeButton_Click(object sender, RoutedEventArgs e)
    {
        ThemeResourceFromCodeRoot.RequestedTheme = ThemeResourceFromCodeRoot.ActualTheme == ElementTheme.Dark
            ? ElementTheme.Light
            : ElementTheme.Dark;
    }

    private void Hyperlink_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
    {
        App.MainWindow.Navigate(typeof(ItemPage), "Color");
    }
}
