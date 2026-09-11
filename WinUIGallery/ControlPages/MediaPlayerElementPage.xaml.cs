using System;
using System.IO;
using Windows.Media.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class MediaPlayerElementPage : Page
    {
        public MediaPlayerElementPage()
        {
            this.InitializeComponent();
            Loaded += MediaPlayerElementPage_Loaded;
            Unloaded += MediaPlayerElementPage_Unloaded;
        }

        private void MediaPlayerElementPage_Loaded(object sender, RoutedEventArgs e)
        {
            MediaPlayerWithControls.AutoPlay = false;
            AutoplayMediaPlayer.AutoPlay = true;

            // Resolve package assets to file URIs for single-project MSIX media playback.
            MediaPlayerWithControls.Source = MediaSource.CreateFromUri(
                new Uri(Path.Combine(AppContext.BaseDirectory, "Assets", "SampleMedia", "ladybug.wmv")));
            AutoplayMediaPlayer.Source = MediaSource.CreateFromUri(
                new Uri(Path.Combine(AppContext.BaseDirectory, "Assets", "SampleMedia", "fishes.wmv")));
        }

        private void MediaPlayerElementPage_Unloaded(object sender, RoutedEventArgs e)
        {
            CloseMediaPlayer(MediaPlayerWithControls);
            CloseMediaPlayer(AutoplayMediaPlayer);
        }

        private static void CloseMediaPlayer(MediaPlayerElement element)
        {
            var source = element.Source as MediaSource;
            var player = element.MediaPlayer;
            element.Source = null;
            element.SetMediaPlayer(null);
            player?.Dispose();
            source?.Dispose();
        }
    }
}
