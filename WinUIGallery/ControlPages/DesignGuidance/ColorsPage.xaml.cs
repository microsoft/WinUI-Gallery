// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinUIGallery.DesktopWap.Controls.DesignGuidance.ColorSections;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ColorsPage : Page
    {
        public ColorsPage()
        {
            this.InitializeComponent();
            NavigationFrame.Navigate(typeof(TextSection));
        }

        private void ToggleButton_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is ToggleButton item)
            {
                switch (item.Content)
                {
                    case "Text":
                        NavigationFrame.Navigate(typeof(TextSection));
                        break;
                    case "Fill":
                        NavigationFrame.Navigate(typeof(FillSection));
                        break;
                    case "Stroke":
                        NavigationFrame.Navigate(typeof(StrokeSection));
                        break;
                    case "Background":
                        NavigationFrame.Navigate(typeof(BackgroundSection));
                        break;
                }
            }
            foreach(var child in SelectionElement.Children)
            {
                if (child != sender)
                {
                    if (child is ToggleButton button)

                        button.IsChecked = false;
                }
            }
        }
    }
}
