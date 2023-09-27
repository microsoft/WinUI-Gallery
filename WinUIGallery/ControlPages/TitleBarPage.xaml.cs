//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Helper;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinUIGallery.DesktopWap.Helper;
using Microsoft.UI.Xaml.Shapes;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppUIBasics.ControlPages
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TitleBarPage : Page
    {
        private Windows.UI.Color currentBgColor = Colors.Transparent;
        private Windows.UI.Color currentFgColor = Colors.Transparent;

        public TitleBarPage()
        {
            this.InitializeComponent();
            Loaded += (object sender, RoutedEventArgs e) =>
            {
                (sender as TitleBarPage).UpdateTitleBarColor();
                UpdateButtonText();
            };
        }


        private void SetTitleBar(UIElement titlebar)
        {
            var window = WindowHelper.GetWindowForElement(this as UIElement);
            var titleBarElement = UIHelper.FindElementByName(this, "AppTitleBar");

            if (!window.ExtendsContentIntoTitleBar)
            {
                titleBarElement.Visibility = Visibility.Visible;
                window.ExtendsContentIntoTitleBar = true;
                window.SetTitleBar(titlebar);
                TitleBarHelper.SetCaptionButtonBackgroundColors(window, Colors.Transparent);
            }
            else
            {
                titleBarElement.Visibility = Visibility.Collapsed;
                window.ExtendsContentIntoTitleBar = false;
                window.SetTitleBar(null);
                TitleBarHelper.SetCaptionButtonBackgroundColors(window, null);
            }
            UpdateButtonText();
            UpdateTitleBarColor();
        }

        public void UpdateButtonText()
        {
            var window = WindowHelper.GetWindowForElement(this as UIElement);
            
            if (window.ExtendsContentIntoTitleBar)
            {
                customTitleBar.Content = "Reset to system TitleBar";
                defaultTitleBar.Content = "Reset to system TitleBar";
            }
            else
            {
                customTitleBar.Content = "Set Custom TitleBar";
                defaultTitleBar.Content = "Set Fallback Custom TitleBar";
            }

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
            var titleBarElement = UIHelper.FindElementByName(this, "AppTitleBar");
            var titleBarAppNameElement = UIHelper.FindElementByName(this, "AppTitle");

            (titleBarElement as Border).Background = new SolidColorBrush(currentBgColor); // changing titlebar uielement's color
            if(currentFgColor != Colors.Transparent)
            {
                (titleBarAppNameElement as TextBlock).Foreground = new SolidColorBrush(currentFgColor);
            }
            else
            {
                (titleBarAppNameElement as TextBlock).Foreground = Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
            }
            TitleBarHelper.SetCaptionButtonColors(window, currentFgColor);
            if(currentBgColor == Colors.Transparent)
            {
                // If the current background is null, we want to revert to the default titlebar which is achieved using null as color
                TitleBarHelper.SetBackgroundColor(window, null);
            }
            else
            {
                TitleBarHelper.SetBackgroundColor(window, currentBgColor);
            }
            TitleBarHelper.SetForegroundColor(window, currentFgColor);
        }

        private void customTitleBar_Click(object sender, RoutedEventArgs e)
        {
            UIElement titleBarElement = UIHelper.FindElementByName(sender as UIElement, "AppTitleBar");
            SetTitleBar(titleBarElement);

            // announce visual change to automation
            UIHelper.AnnounceActionForAccessibility(sender as UIElement, "TitleBar size and width changed", "TitleBarChangedNotificationActivityId");
        }
        private void defaultTitleBar_Click(object sender, RoutedEventArgs e)
        {
            SetTitleBar(null);

            // announce visual change to automation
            UIHelper.AnnounceActionForAccessibility(sender as UIElement, "TitleBar size and width changed", "TitleBarChangedNotificationActivityId");
        }
    }
}
