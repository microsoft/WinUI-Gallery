// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.BadgeNotifications;
using System;
using System.Collections.Generic;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class BadgeNotificationManagerPage : Page
{
    private IReadOnlyList<BadgeNotificationGlyph> badgeNotificationGlyphs = new List<BadgeNotificationGlyph>(Enum.GetValues<BadgeNotificationGlyph>());
    private BadgeNotificationGlyph selectedGlyph = BadgeNotificationGlyph.Activity;
    private bool isBadgeSetted = false;

    public BadgeNotificationManagerPage()
    {
        this.InitializeComponent();
    }

    private void SetBadgeCountButton_Click(object sender, RoutedEventArgs e)
    {
        if (NativeMethods.IsAppPackaged)
        {
            BadgeNotificationManager.Current.SetBadgeAsCount((uint)BadgeCountBox.Value);
            isBadgeSetted = true;
        }
    }

    private void ClearBadgeButton_Click(object sender, RoutedEventArgs e)
    {
        if (NativeMethods.IsAppPackaged)
        {
            BadgeNotificationManager.Current.ClearBadge();
        }
    }

    private void SetBadgeGlyphButton_Click(object sender, RoutedEventArgs e)
    {
        if (NativeMethods.IsAppPackaged)
        {
            BadgeNotificationManager.Current.SetBadgeAsGlyph(selectedGlyph);
            isBadgeSetted = true;
        }
    }

    private void BadgeCountBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (NativeMethods.IsAppPackaged)
        {
            if (isBadgeSetted)
            {
                BadgeNotificationManager.Current.SetBadgeAsCount((uint)BadgeCountBox.Value);
            }
        }
    }

    private void BadgeNotificationGlyphsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NativeMethods.IsAppPackaged)
        {
            if (isBadgeSetted)
            {
                BadgeNotificationManager.Current.SetBadgeAsGlyph(selectedGlyph);
            }
        }
    }
}
