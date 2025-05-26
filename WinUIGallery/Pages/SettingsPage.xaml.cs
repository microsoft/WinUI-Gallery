//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
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
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }

    public string WinAppSdkRuntimeDetails => VersionHelper.WinAppSdkRuntimeDetails;
    private int lastNavigationSelectionMode = 0;

    public SettingsPage()
    {
        this.InitializeComponent();
        Loaded += OnSettingsPageLoaded;
        ClearVisitedSamplesCard.IsEnabled = SettingsHelper.HasList(SettingsKeys.RecentlyVisited);
        UnfavoriteSamplesCard.IsEnabled = SettingsHelper.HasList(SettingsKeys.Favorites);
        SamplesSettingsExpander.IsExpanded = ClearVisitedSamplesCard.IsEnabled || UnfavoriteSamplesCard.IsEnabled;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }

    private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
    {
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

        NavigationRootPage navigationRootPage = NavigationRootPage.GetForElement(this);
        if (navigationRootPage != null)
        {
            if (navigationRootPage.NavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto)
            {
                navigationLocation.SelectedIndex = 0;
            }
            else
            {
                navigationLocation.SelectedIndex = 1;
            }
            lastNavigationSelectionMode = navigationLocation.SelectedIndex;
        }

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
                color = TitleBarHelper.ApplySystemThemeToCaptionButtons(window) == Colors.White  ? "Dark" : "Light";
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
            NavigationOrientationHelper.IsLeftModeForElement(navigationLocation.SelectedIndex == 0, this);
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
        this.Frame.Navigate(typeof(ItemPage), new NavigationRootPageArgs() { Parameter = "Sound", NavigationRootPage = NavigationRootPage.GetForElement(this) });
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

    private void ClearRecentlyVisitedSamples_Click(object sender, RoutedEventArgs e)
    {
        ClearRecentlyVisitedSamplesFlyout.Hide();
        SettingsHelper.ClearList(SettingsKeys.RecentlyVisited);
        ClearVisitedSamplesCard.IsEnabled = false;
        SamplesSettingsExpander.IsExpanded = !ClearVisitedSamplesCard.IsEnabled && !UnfavoriteSamplesCard.IsEnabled;
    }

    private void UnfavoriteAllSamples_Click(object sender, RoutedEventArgs e)
    {
        UnfavoriteAllSamplesFlyout.Hide();
        SettingsHelper.ClearList(SettingsKeys.Favorites);
        UnfavoriteSamplesCard.IsEnabled = false;
        SamplesSettingsExpander.IsExpanded = !ClearVisitedSamplesCard.IsEnabled && !UnfavoriteSamplesCard.IsEnabled;
    }
}
