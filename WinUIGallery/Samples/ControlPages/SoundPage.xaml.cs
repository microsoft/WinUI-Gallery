// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class SoundPage : Page
{
    public SoundPage()
    {
        this.InitializeComponent();

        if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
            soundToggle.IsOn = true;
        if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On && soundToggle.IsOn == true)
            spatialAudioBox.IsChecked = true;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var tagInt = int.Parse((string)(sender as Button).Tag);
        ElementSoundPlayer.Play((ElementSoundKind)tagInt);
    }

    private void spatialAudioBox_Checked(object sender, RoutedEventArgs e)
    {
        if (soundToggle.IsOn == true)
        {
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }
    }

    private void spatialAudioBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (soundToggle.IsOn == true)
        {
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
        }
    }

    private void soundToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (soundToggle.IsOn == true)
        {
            spatialAudioBox.IsEnabled = true;
            ElementSoundPlayer.State = ElementSoundPlayerState.On;
        }
        else
        {
            spatialAudioBox.IsEnabled = false;
            spatialAudioBox.IsChecked = false;

            ElementSoundPlayer.State = ElementSoundPlayerState.Off;
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
        }
    }
}
