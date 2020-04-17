//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AppBarPage : Page
    {
        public AppBarPage()
        {
            this.InitializeComponent();
        }

        private void topAppBar_Opened(object sender, object e)
        {

                CommandBar headerTopAppBar = NavigationRootPage.Current.PageHeader.TopCommandBar;
                headerTopAppBar.IsOpen = false;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            topAppBar.IsOpen = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            topAppBar.IsOpen = false;
        }

        private void NavBarButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonBase b = (ButtonBase)sender;

            if (Window.Current.Content is Frame rootFrame && b.Tag != null)
            {
                if (b.Tag.ToString() == "Home")
                {
                    rootFrame.Navigate(typeof(AppUIBasics.AllControlsPage));
                }
                else
                {
                    rootFrame.Navigate(typeof(SectionPage), b.Tag.ToString());
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppBarContentPanel.Children[0] is Button homeButton && homeButton.Tag.ToString() != "Home")
            {
                homeButton = new Button
                {
                    Content = "Home",
                    Tag = "Home"
                };
                homeButton.Click += NavBarButton_Click;

                AppBarContentPanel.Children.Insert(0, homeButton);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveHomeButton();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            RemoveHomeButton();
            base.OnNavigatingFrom(e);
        }

        private void RemoveHomeButton()
        {
            if (AppBarContentPanel.Children[0] is Button homeButton && homeButton.Tag.ToString() == "Home")
            {
                homeButton.Click -= NavBarButton_Click;
                AppBarContentPanel.Children.RemoveAt(0);
            }
        }
    }
}
