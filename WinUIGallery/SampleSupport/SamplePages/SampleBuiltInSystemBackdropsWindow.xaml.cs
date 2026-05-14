// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WinUIGallery.Helpers;

namespace WinUIGallery.SamplePages;

public sealed partial class SampleBuiltInSystemBackdropsWindow : Window
{
    BackdropType currentBackdrop;

    public SampleBuiltInSystemBackdropsWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon(@"Assets\Tiles\GalleryIcon.ico");
        ExtendsContentIntoTitleBar = true;
        backdropComboBox.SelectedIndex = 0;
        themeComboBox.SelectedIndex = 0;

        ((FrameworkElement)Content).RequestedTheme = ThemeHelper.RootTheme;
    }

    public enum BackdropType
    {
        None,
        Mica,
        MicaAlt,
        Acrylic
    }

    public void SetBackdrop(BackdropType type)
    {
        // Reset to default color. If the requested type is supported, we'll update to that.
        // Note: This sample completely removes any previous controller to reset to the default
        //       state. This is done so this sample can show what is expected to be the most
        //       common pattern of an app simply choosing one controller type which it sets at
        //       startup. If an app wants to toggle between Mica and Acrylic it could simply
        //       call RemoveSystemBackdropTarget() on the old controller and then setup the new
        //       controller, reusing any existing configurationSource and Activated/Closed
        //       event handlers.

        //Reset the backdrop
        currentBackdrop = BackdropType.None;
        tbChangeStatus.Text = "";
        SystemBackdrop = null;

        //Set the backdrop
        if (type == BackdropType.Mica)
        {
            if (TrySetMicaBackdrop(false))
                currentBackdrop = type;
            else
            {
                // Mica isn't supported. Try Acrylic.
                type = BackdropType.Acrylic;
                tbChangeStatus.Text += "  Mica isn't supported. Trying Acrylic.";
            }
        }
        if (type == BackdropType.MicaAlt)
        {
            if (TrySetMicaBackdrop(true))
                currentBackdrop = type;
            else
            {
                // MicaAlt isn't supported. Try Acrylic.
                type = BackdropType.Acrylic;
                tbChangeStatus.Text += "  MicaAlt isn't supported. Trying Acrylic.";
            }
        }
        if (type == BackdropType.Acrylic)
        {
            if (TrySetAcrylicBackdrop())
                currentBackdrop = type;
            else
            {
                // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                tbChangeStatus.Text += "  Acrylic isn't supported. Switching to default color.";
            }
        }

        //Fix the none backdrop
        SetNoneBackdropBackground();

        // Announce visual change to automation.
        UIHelper.AnnounceActionForAccessibility(backdropComboBox, $"Background changed to {currentBackdrop}", "BackgroundChangedNotificationActivityId");
    }

    bool TrySetMicaBackdrop(bool useMicaAlt)
    {
        if (MicaController.IsSupported())
        {
            SystemBackdrop = new MicaBackdrop { Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base }; ;
            return true;
        }

        return false; // Mica is not supported on this system
    }

    bool TrySetAcrylicBackdrop()
    {
        if (DesktopAcrylicController.IsSupported())
        {
            SystemBackdrop = new DesktopAcrylicBackdrop();
            return true;
        }

        return false; // Acrylic is not supported on this system
    }

    private void BackdropComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetBackdrop(backdropComboBox.SelectedIndex switch
        {
            1 => BackdropType.MicaAlt,
            2 => BackdropType.Acrylic,
            3 => BackdropType.None,
            _ => BackdropType.Mica
        });
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ((FrameworkElement)Content).RequestedTheme = Enum.GetValues<ElementTheme>()[themeComboBox.SelectedIndex];
        TitleBarHelper.ApplySystemThemeToCaptionButtons(this, ((FrameworkElement)Content).ActualTheme);
        SetNoneBackdropBackground();
    }

    //Fixes the background color not changing when switching between themes.
    void SetNoneBackdropBackground()
    {
        if (currentBackdrop == BackdropType.None && themeComboBox.SelectedIndex != 0)
            ((Grid)Content).Background = new SolidColorBrush(themeComboBox.SelectedIndex == 1 ? Colors.White : Colors.Black);
        else
            ((Grid)Content).Background = new SolidColorBrush(Colors.Transparent);
    }
}
