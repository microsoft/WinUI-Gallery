using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AnimatedVisualPlayerPage : Page
    {
        private bool wasPaused = false;

        public AnimatedVisualPlayerPage()
        {
            this.InitializeComponent();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop the animation, which completes PlayAsync and resets to initial frame. 
            Player.Stop();
            Player.SetProgress(0);
            SetIsPlayingIndicator(false);
            wasPaused = false;
        }

        private void ReverseButton_Click(object sender, RoutedEventArgs e)
        {
            // Set reverse playback rate.
            // NOTE: This property is live, which means it takes effect even if the animation is playing.
            Player.PlaybackRate = -1;
            EnsurePlaying();
        }

        private void EnsurePlaying()
        {
            if (!Player.IsPlaying)
            {
                // Play the animation at the currently specified playback rate.
                Player.PlayAsync(fromProgress: 0, toProgress: 1, looped: false).GetAwaiter().OnCompleted(
                    () => {
                        SetIsPlayingIndicator(false);
                    }
                );
                SetIsPlayingIndicator(true);
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            // Is the player playing and we did not get paused?
            if(Player.IsPlaying && !wasPaused)
            {
                Player.Pause();
                SetIsPlayingIndicator(false);
                wasPaused = true;
            }
            else
            {
                // Either not playing or paused
                // If paused, resume and set paused to false
                if(wasPaused)
                {
                    Player.Resume();
                    wasPaused = false;
                }
                // Not playing, start animation now
                else
                {
                    Player.PlaybackRate = 1;
                    Player.PlayAsync(fromProgress: 0, toProgress: 1, looped: false).GetAwaiter().OnCompleted(
                        () => {
                            SetIsPlayingIndicator(false);
                        }
                    );
                    wasPaused = false;
                }
                SetIsPlayingIndicator(true);
            }
        }

        private void SetIsPlayingIndicator(bool isPlaying)
        {
            if(isPlaying)
            {
                PlayIcon.Visibility = Visibility.Collapsed;
                PauseIcon.Visibility = Visibility.Visible;
                ToolTipService.SetToolTip(PlayPauseButton, "Pause");
                PlayPauseButton.SetValue(AutomationProperties.NameProperty, "Pause");
            }
            else
            {
                PlayIcon.Visibility = Visibility.Visible;
                PauseIcon.Visibility = Visibility.Collapsed;
                ToolTipService.SetToolTip(PlayPauseButton, "Play");
                PlayPauseButton.SetValue(AutomationProperties.NameProperty, "Play");
            }
        }
    }
}
