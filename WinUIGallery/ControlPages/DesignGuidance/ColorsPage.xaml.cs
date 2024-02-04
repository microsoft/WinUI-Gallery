// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using WinUIGallery.DesktopWap.Controls.DesignGuidance.ColorSections;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ColorsPage : Page
    {
        public ColorsPage()
        {
            this.InitializeComponent();
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (PageSelector.SelectedIndex)
            {
                case 0:
                    NavigationFrame.Navigate(typeof(TextSection));
                    break;
                case 1:
                    NavigationFrame.Navigate(typeof(FillSection));
                    break;
                case 2:
                    NavigationFrame.Navigate(typeof(StrokeSection));
                    break;
                case 3:
                    NavigationFrame.Navigate(typeof(BackgroundSection));
                    break;
                case 4:
                    NavigationFrame.Navigate(typeof(SignalSection));
                    break;
                case 5:
                    NavigationFrame.Navigate(typeof(HighContrastSection));
                    break;
            }
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            PageSelector.SelectedItem = PageSelector.Items[0];
        }
    }
}
