//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Samples.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class TitleBarPage : Page
{
    public TitleBarPage()
    {
        this.InitializeComponent();
    }

    private void CreateTitleBarWindowClick(object sender, RoutedEventArgs e)
    {
        TitleBarWindow titleBarWindow = new TitleBarWindow();
        titleBarWindow.Activate();
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

        (titleBarElement as Border).Background = new SolidColorBrush(currentBgColor); // Changing titlebar uielement's color.

        if (currentFgColor != Colors.Transparent)
        {
            (titleBarAppNameElement as TextBlock).Foreground = new SolidColorBrush(currentFgColor);
        }
        else
        {
            (titleBarAppNameElement as TextBlock).Foreground = Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
        }

        TitleBarHelper.SetCaptionButtonColors(window, currentFgColor);

        if (currentBgColor == Colors.Transparent)
        {
            // If the current background is null, we want to revert to the default titlebar which is achieved using null as color.
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
        SetTitleBar();
        // announce visual change to automation
        UIHelper.AnnounceActionForAccessibility(sender as UIElement, "TitleBar size and width changed", "TitleBarChangedNotificationActivityId");
    }
    private void defaultTitleBar_Click(object sender, RoutedEventArgs e)
    {
        SetTitleBar();

        // announce visual change to automation
        UIHelper.AnnounceActionForAccessibility(sender as UIElement, "TitleBar size and width changed", "TitleBarChangedNotificationActivityId");
    }

    private void setTxtBoxAsPasthrough(FrameworkElement txtBoxNonClientArea)
    {
        GeneralTransform transformTxtBox = txtBoxNonClientArea.TransformToVisual(null);
        Rect bounds = transformTxtBox.TransformBounds(new Rect(0, 0, txtBoxNonClientArea.ActualWidth, txtBoxNonClientArea.ActualHeight));

        var scale = WindowHelper.GetRasterizationScaleForElement(this);

        var transparentRect = new Windows.Graphics.RectInt32(
            _X: (int)Math.Round(bounds.X * scale),
            _Y: (int)Math.Round(bounds.Y * scale),
            _Width: (int)Math.Round(bounds.Width * scale),
            _Height: (int)Math.Round(bounds.Height * scale)
        );
        var rectArr = new Windows.Graphics.RectInt32[] { transparentRect };
        SetClickThruRegions(rectArr);
    }

    private void TitleBarHeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedHeight = titlebarHeight.SelectedItem.ToString();
        var window = WindowHelper.GetWindowForElement(this);

        if (selectedHeight != null && window != null && window.ExtendsContentIntoTitleBar)
        {
            window.AppWindow.TitleBar.PreferredHeightOption = EnumHelper.GetEnum<TitleBarHeightOption>(selectedHeight);
        }
    }

    private void AddInteractiveElements_Click(object sender, RoutedEventArgs e)
    {
        var txtBoxNonClientArea = UIHelper.FindElementByName(sender as UIElement, "AppTitleBarTextBox") as FrameworkElement;

        if (txtBoxNonClientArea.Visibility == Visibility.Visible)
        {
            ResetTitlebarSettings();
        }
        else
        {
            addInteractiveElements.Content = "Remove interactive control from titlebar";
            txtBoxNonClientArea.Visibility = Visibility.Visible;
            if (sizeChangedEventHandlerAdded)
            {
                setTxtBoxAsPasthrough(txtBoxNonClientArea);
            }
            else
            {
                sizeChangedEventHandlerAdded = true;
                // run this code when textbox has been made visible and its actual width and height has been calculated
                txtBoxNonClientArea.SizeChanged += (object sender, SizeChangedEventArgs e) =>
                {
                    if (txtBoxNonClientArea.Visibility != Visibility.Collapsed)
                    {
                        setTxtBoxAsPasthrough(txtBoxNonClientArea);
                    }
                };
            }

            // announce visual change to automation
            UIHelper.AnnounceActionForAccessibility(sender as UIElement, "TitleBar size and width changed", "TitleBarChangedNotificationActivityId");
        }
    }
}
