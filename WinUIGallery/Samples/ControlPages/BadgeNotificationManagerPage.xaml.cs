using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.BadgeNotifications;

namespace WinUIGallery.ControlPages;

public sealed partial class BadgeNotificationManagerPage : Page
{
    private IReadOnlyList<BadgeNotificationGlyph> badgeNotificationGlyphs = Enum.GetValues(typeof(BadgeNotificationGlyph)).Cast<BadgeNotificationGlyph>().ToList();
    private BadgeNotificationGlyph selectedGlyph = BadgeNotificationGlyph.Activity;
    private bool isBadgeSetted = false;

    public BadgeNotificationManagerPage()
    {
        this.InitializeComponent();
    }

    private void SetBadgeCountButton_Click(object sender, RoutedEventArgs e)
    {
        BadgeNotificationManager.Current.SetBadgeAsCount((uint)BadgeCountBox.Value);
        isBadgeSetted = true;
    }

    private void ClearBadgeButton_Click(object sender, RoutedEventArgs e)
    {
        BadgeNotificationManager.Current.ClearBadge();
    }

    private void SetBadgeGlyphButton_Click(object sender, RoutedEventArgs e)
    {
        BadgeNotificationManager.Current.SetBadgeAsGlyph(selectedGlyph);
        isBadgeSetted = true;
    }

    private void BadgeCountBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (isBadgeSetted)
        {
            BadgeNotificationManager.Current.SetBadgeAsCount((uint)BadgeCountBox.Value);
        }
    }

    private void BadgeNotificationGlyphsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isBadgeSetted)
        {
            BadgeNotificationManager.Current.SetBadgeAsGlyph(selectedGlyph);
        }
    }
}
