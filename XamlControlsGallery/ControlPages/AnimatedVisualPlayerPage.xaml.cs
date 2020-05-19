using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AnimatedVisualPlayerPage : Page
    {
        public AnimatedVisualPlayerPage()
        {
            this.InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Set forward playback rate.
            // NOTE: This property is live, which means it takes effect even if the animation is playing.
            Player.PlaybackRate = 1;
            EnsurePlaying();
        }

        private void PauseButton_Checked(object sender, RoutedEventArgs e)
        {
            // Pause the animation, if playing.
            // NOTE: Pausing does not cause PlayAsync to complete.
            Player.Pause();
        }

        private void PauseButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Resume playing current animation.
            Player.Resume();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop the animation, which completes PlayAsync and resets to initial frame. 
            Player.Stop();
            PauseButton.IsChecked = false;
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
            if (PauseButton.IsChecked.Value)
            {
                // Resume playing the animation, if paused.
                PauseButton.IsChecked = false;
            }
            else
            {
                if (!Player.IsPlaying)
                {
                    // Play the animation at the currently specified playback rate.
                    _ = Player.PlayAsync(fromProgress: 0, toProgress: 1, looped: false);
                }
            }
        }
    }
}
