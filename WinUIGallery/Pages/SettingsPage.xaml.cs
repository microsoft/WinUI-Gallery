// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using WinUIGallery.Helpers;

namespace WinUIGallery.Pages;

/// <summary>
/// A page that displays the app's settings.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public string Version
    {
        get
        {
            return ProcessInfoHelper.GetVersion() is Version version
                ? string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision)
                : string.Empty;
        }
    }

    public string WinAppSdkRuntimeDetails => VersionHelper.WinAppSdkRuntimeDetails;
    private int lastNavigationSelectionMode = 0;

    public SettingsPage()
    {
        this.InitializeComponent();
        Loaded += OnSettingsPageLoaded;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }

    private void CheckRecentAndFavoriteButtonStates()
    {
        ClearRecentBtn.IsEnabled = SettingsHelper.Current.RecentlyVisited.Count > 0;
        UnfavoriteBtn.IsEnabled = SettingsHelper.Current.Favorites.Count > 0;
    }

    private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
    {
        CheckRecentAndFavoriteButtonStates();
        var currentTheme = ThemeHelper.RootTheme;
        switch (currentTheme)
        {
            case ElementTheme.Light:
                themeMode.SelectedIndex = 0;
                break;
            case ElementTheme.Dark:
                themeMode.SelectedIndex = 1;
                break;
            case ElementTheme.Default:
                themeMode.SelectedIndex = 2;
                break;
        }

        if (App.MainWindow.NavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto)
        {
            navigationLocation.SelectedIndex = 0;
        }
        else
        {
            navigationLocation.SelectedIndex = 1;
        }

        lastNavigationSelectionMode = navigationLocation.SelectedIndex;

        if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
            soundToggle.IsOn = true;
        if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On)
            spatialSoundBox.IsOn = true;
    }

    private void themeMode_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not UIElement senderUiLement ||
            (themeMode.SelectedItem as ComboBoxItem)?.Tag.ToString() is not string selectedTheme ||
            WindowHelper.GetWindowForElement(this) is not Window window)
        {
            return;
        }

        ThemeHelper.RootTheme = EnumHelper.GetEnum<ElementTheme>(selectedTheme);
        var elementThemeResolved = ThemeHelper.RootTheme == ElementTheme.Default ? ThemeHelper.ActualTheme : ThemeHelper.RootTheme;
        TitleBarHelper.ApplySystemThemeToCaptionButtons(window, elementThemeResolved);

        // announce visual change to automation
        UIHelper.AnnounceActionForAccessibility(
            senderUiLement,
            $"Theme changed to {elementThemeResolved}",
            "ThemeChangedNotificationActivityId");
    }

    private void soundToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (soundToggle.IsOn == true)
        {
            SpatialAudioCard.IsEnabled = true;
            ElementSoundPlayer.State = ElementSoundPlayerState.On;
        }
        else
        {
            SpatialAudioCard.IsEnabled = false;
            spatialSoundBox.IsOn = false;

            ElementSoundPlayer.State = ElementSoundPlayerState.Off;
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
        }
    }

    private void navigationLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Since setting the left mode does not look at the old setting we 
        // need to check if this is an actual update
        if (navigationLocation.SelectedIndex != lastNavigationSelectionMode)
        {
            NavigationOrientationHelper.IsLeftModeForElement(navigationLocation.SelectedIndex == 0);
            lastNavigationSelectionMode = navigationLocation.SelectedIndex;
        }
    }

    private void spatialSoundBox_Toggled(object sender, RoutedEventArgs e)
    {
        if (soundToggle.IsOn == true)
        {
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
        }
        else
        {
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }
    }

    private void soundPageHyperlink_Click(object sender, RoutedEventArgs e)
    {
        App.MainWindow.Navigate(typeof(ItemPage), "Sound");
    }

    private void toCloneRepoCard_Click(object sender, RoutedEventArgs e)
    {
        DataPackage package = new DataPackage();
        package.SetText(gitCloneTextBlock.Text);
        Clipboard.SetContent(package);
    }

    private async void bugRequestCard_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/WinUI-Gallery/issues/new/choose"));

    }
    private async void UnfavoriteBtn_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Remove all favorites?",
            PrimaryButtonText = "Remove",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = "This will unfavorite all your samples.",
            RequestedTheme = this.ActualTheme
        };
        dialog.PrimaryButtonClick += (s, args) =>
        {
            SettingsHelper.Current.UpdateFavorites(items => items.Clear());
            CheckRecentAndFavoriteButtonStates();
        };
        var result = await dialog.ShowAsync();
    }

    private async void ClearRecentBtn_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Clear recently visited samples?",
            PrimaryButtonText = "Clear",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = "This will remove all samples from your recent history.",
            RequestedTheme = this.ActualTheme
        };
        dialog.PrimaryButtonClick += (s, args) =>
        {
            SettingsHelper.Current.UpdateRecentlyVisited(items => items.Clear());
            CheckRecentAndFavoriteButtonStates();
        };
        var result = await dialog.ShowAsync();
    }
}
