// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using WinUIGallery.ControlPages;
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
            var version = ProcessInfoHelper.GetVersion();
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
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
        ClearRecentBtn.IsEnabled = SettingsHelper.Exists(SettingsKeys.RecentlyVisited);
        UnfavoriteBtn.IsEnabled = SettingsHelper.Exists(SettingsKeys.Favorites);
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
        var selectedTheme = ((ComboBoxItem)themeMode.SelectedItem)?.Tag?.ToString();
        var window = WindowHelper.GetWindowForElement(this);
        string color;
        if (selectedTheme != null)
        {
            ThemeHelper.RootTheme = EnumHelper.GetEnum<ElementTheme>(selectedTheme);
            if (selectedTheme == "Dark")
            {
                TitleBarHelper.SetCaptionButtonColors(window, Colors.White);
                color = selectedTheme;
            }
            else if (selectedTheme == "Light")
            {
                TitleBarHelper.SetCaptionButtonColors(window, Colors.Black);
                color = selectedTheme;
            }
            else
            {
                color = TitleBarHelper.ApplySystemThemeToCaptionButtons(window, this.ActualTheme) == Colors.White ? "Dark" : "Light";
            }
            // announce visual change to automation
            UIHelper.AnnounceActionForAccessibility(sender as UIElement, $"Theme changed to {color}",
                                                                            "ThemeChangedNotificationActivityId");
        }
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
            SettingsHelper.Delete(SettingsKeys.Favorites);
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
            SettingsHelper.Delete(SettingsKeys.RecentlyVisited);
            CheckRecentAndFavoriteButtonStates();
        };
        var result = await dialog.ShowAsync();
    }
}
