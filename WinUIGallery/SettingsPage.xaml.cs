//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Helper;
using System;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using WinRT;
using System.Runtime.InteropServices;
using WinUIGallery.DesktopWap.Helper;

namespace AppUIBasics
{
    /// <summary>
    /// A page that displays the app's settings.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;

            if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
                soundToggle.IsOn = true;
            if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On)
                spatialSoundBox.IsChecked = true;

            ScreenshotSettingsGrid.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
            args.NavigationRootPage.NavigationView.Header = "Settings";
        }

        private async void OnFeedbackButtonClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("feedback-hub:"));
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeHelper.RootTheme.ToString();
            (ThemePanel.Children.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme)).IsChecked = true;

            NavigationRootPage navigationRootPage = NavigationRootPage.GetForElement(this);
            if (navigationRootPage != null)
            {
                if (navigationRootPage.NavigationView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto)
                {
                    navigationLocation.SelectedIndex = 0;
                }
                else
                {
                    navigationLocation.SelectedIndex = 1;
                }
            }
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            var res = Microsoft.UI.Xaml.Application.Current.Resources;
            Action<Windows.UI.Color> SetTitleBarButtonForegroundColor = (Windows.UI.Color color) => { res["WindowCaptionForeground"] = color; };

            if (selectedTheme != null)
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
                if (selectedTheme == "Dark")
                {
                    SetTitleBarButtonForegroundColor(Colors.White);
                }
                else if (selectedTheme == "Light")
                {
                    SetTitleBarButtonForegroundColor(Colors.Black);
                }
                else
                {
                    if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        SetTitleBarButtonForegroundColor(Colors.White);
                    }
                    else
                    {
                        SetTitleBarButtonForegroundColor(Colors.Black);
                    }
                }
            }
            var window = WindowHelper.GetWindowForElement(this);
            TitleBarHelper.triggerTitleBarRepaint(window);

        }

        private void OnThemeRadioButtonKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Up)
            {
                NavigationRootPage.GetForElement(this).PageHeader.Focus(FocusState.Programmatic);
            }
        }
        private void spatialSoundBox_Checked(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                spatialSoundBox.IsEnabled = true;
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else
            {
                spatialSoundBox.IsEnabled = false;
                spatialSoundBox.IsChecked = false;

                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
        }

        private void screenshotModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            UIHelper.IsScreenshotMode = screenshotModeToggle.IsOn;
        }

        private void spatialSoundBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
        }

        private void navigationLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationOrientationHelper.IsLeftModeForElement(navigationLocation.SelectedIndex == 0, this);
        }

        private async void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add(".png"); // meaningless, but you have to have something
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                UIHelper.ScreenshotStorageFolder = folder;
                screenshotFolderLink.Content = UIHelper.ScreenshotStorageFolder.Path;
            }
        }

        private async void screenshotFolderLink_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(UIHelper.ScreenshotStorageFolder);
        }

        private void OnResetTeachingTipsButtonClick(object sender, RoutedEventArgs e)
        {
            ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip = true;
        }

        private void soundPageHyperlink_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            this.Frame.Navigate(typeof(ItemPage), new NavigationRootPageArgs() { Parameter = "Sound", NavigationRootPage = NavigationRootPage.GetForElement(this) });
        }
    }
}
