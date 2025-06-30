// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

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
}
