// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using Windows.Media.Core;
using Windows.Storage;

namespace WinUIGallery.ControlPages;

public sealed partial class MediaPlayerElementPage : Page
{
    public MediaPlayerElementPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Needed if this page is getting cached due to the navigation stack.
        Player2.MediaPlayer.Play();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        // Pause media playback since we are no longer visible to the user
        Player1.MediaPlayer.Pause();
        Player2.MediaPlayer.Pause();
    }

    private async void OpenFileButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var picker = new FileOpenPicker((sender as Button).XamlRoot.ContentIslandEnvironment.AppWindowId);
        var file = await picker.PickSingleFileAsync();
        if (file == null)
            return;

        var mediaSource = MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(file.Path));
        Player1.Source = mediaSource;
    }
}
