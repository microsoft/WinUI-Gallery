// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using WinUIGallery.DesktopWap.Controls.DesignGuidance.ColorSections;
using WinUIGallery.SamplePages;

namespace WinUIGallery.ControlPages
{
    public sealed partial class ColorsPage : Page
    {
        int previousSelectedIndex = 0;

        public ColorsPage()
        {
            this.InitializeComponent();
        }

        private void PageSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectedItem = sender.SelectedItem;
            int currentSelectedIndex = sender.Items.IndexOf(selectedItem);
            Type pageType;

            switch (currentSelectedIndex)
            {
                case 0:
                    pageType = typeof(TextSection);
                    break;
                case 1:
                    pageType = typeof(FillSection);
                    break;
                case 2:
                    pageType = typeof(StrokeSection);
                    break;
                case 3:
                    pageType = typeof(BackgroundSection);
                    break;
                case 4:
                    pageType = typeof(SignalSection);
                    break;
                case 5:
                    pageType = typeof(HighContrastSection);
                    break;
                default:
                    pageType = typeof(TextSection);
                    break;
            }

            var slideNavigationTransitionEffect = currentSelectedIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

            NavigationFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });

            previousSelectedIndex = currentSelectedIndex;
        }

        private void PageSelector_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            PageSelector.SelectedItem = PageSelector.Items[0];
        }
    }
}
