//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using WinUIGallery.Helper;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinUIGallery.DesktopWap.Helper;
using Microsoft.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Input;
using System.IO;
using Windows.Foundation;
using System;
using WinUIGallery.SamplePages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.ControlPages
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TitleBarPage : Page
    {
        private Windows.UI.Color currentBgColor = Colors.Transparent;
        private Windows.UI.Color currentFgColor = ThemeHelper.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;

        public TitleBarPage()
        {
            this.InitializeComponent();
            Loaded += (object sender, RoutedEventArgs e) =>
            {
                // Known Preview bug: Parts get delay loaded. If you have the parts, make them visible.
                VisualStateManager.GoToState(ControlsTitleBar, "SubtitleTextVisible", false);
                VisualStateManager.GoToState(ControlsTitleBar, "HeaderVisible", false);
                VisualStateManager.GoToState(ControlsTitleBar, "ContentVisible", false);
                VisualStateManager.GoToState(ControlsTitleBar, "FooterVisible", false);

                // Run layout so we re-calculate the drag regions.
                ControlsTitleBar.InvalidateMeasure();

                VisualStateManager.GoToState(NavViewTitleBar, "ContentVisible", false);
                NavViewTitleBar.InvalidateMeasure();
            };
        }

        private void NavViewTitleBar_BackRequested(TitleBar sender, object args)
        {
            if (NavFrame.CanGoBack)
            {
                NavFrame.GoBack();
            }
        }

        private void NavViewTitleBar_PaneToggleRequested(TitleBar sender, object args)
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavFrame.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                string pageName = "WinUIGallery.SamplePages." + ((string)selectedItem.Tag);
                Type pageType = Type.GetType(pageName);

                NavFrame.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
            }
        }
        
        private void SetTitleBar(UIElement titlebar)
        {
            var window = WindowHelper.GetWindowForElement(this as UIElement);
            var titleBarElement = UIHelper.FindElementByName(this as UIElement, "AppTitleBar");

            if (!window.ExtendsContentIntoTitleBar)
            {
                titleBarElement.Visibility = Visibility.Visible;
                window.ExtendsContentIntoTitleBar = true;
                window.SetTitleBar(titlebar);
            }
            else
            {
                titleBarElement.Visibility = Visibility.Collapsed;
                window.ExtendsContentIntoTitleBar = false;
                window.SetTitleBar(null);
            }
        }

        private void ToggleTitleBar_Click(object sender, RoutedEventArgs e)
        {
            UIElement titleBarElement = UIHelper.FindElementByName(sender as UIElement, "AppTitleBar");
            SetTitleBar(titleBarElement);

            // Announce visual change to automation.
            UIHelper.AnnounceActionForAccessibility(sender as UIElement, "TitleBar size and width changed", "TitleBarChangedNotificationActivityId");
        }

        private void BgGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rect = (Rectangle)e.ClickedItem;
            var color = ((SolidColorBrush)rect.Fill).Color;
            BackgroundColorElement.Background = new SolidColorBrush(color);

            currentBgColor = color;
            UpdateTitleBarColor();

            // Delay required to circumvent GridView bug: https://github.com/microsoft/microsoft-ui-xaml/issues/6350
            Task.Delay(10).ContinueWith(_ => myBgColorButton.Flyout.Hide(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void FgGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rect = (Rectangle)e.ClickedItem;
            var color = ((SolidColorBrush)rect.Fill).Color;

            ForegroundColorElement.Background = new SolidColorBrush(color);

            currentFgColor = color;
            UpdateTitleBarColor();

            // Delay required to circumvent GridView bug: https://github.com/microsoft/microsoft-ui-xaml/issues/6350
            Task.Delay(10).ContinueWith(_ => myFgColorButton.Flyout.Hide(), TaskScheduler.FromCurrentSynchronizationContext());
        }


        public void UpdateTitleBarColor()
        {
            var window = WindowHelper.GetWindowForElement(this);
            var titleBar = UIHelper.FindElementByName(this, "AppTitleBar");

            (titleBar as TitleBar).Background = new SolidColorBrush(currentBgColor);

            if(currentFgColor != Colors.Transparent)
            {
                (titleBar as TitleBar).Foreground = new SolidColorBrush(currentFgColor);
            }
            else
            {
                (titleBar as TitleBar).Foreground = Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
            }

            TitleBarHelper.SetForegroundColor(window, currentFgColor);

            if (currentBgColor == Colors.Transparent)
            {
                // If the current background is null, we want to revert to the default titlebar which is achieved using null as color.
                TitleBarHelper.SetBackgroundColor(window, null);
            }
            else
            {
                TitleBarHelper.SetBackgroundColor(window, currentBgColor);
            }
        }
    }
}
