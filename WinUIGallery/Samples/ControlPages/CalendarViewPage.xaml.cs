using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class CalendarViewPage : Page
{
    public CalendarViewPage()
    {
        this.InitializeComponent();

        List<string> calendarIdentifiers = new List<string>()
        {
            CalendarIdentifiers.Gregorian,
            CalendarIdentifiers.Hebrew,
            CalendarIdentifiers.Hijri,
            CalendarIdentifiers.Japanese,
            CalendarIdentifiers.Julian,
            CalendarIdentifiers.Korean,
            CalendarIdentifiers.Persian,
            CalendarIdentifiers.Taiwan,
            CalendarIdentifiers.Thai,
            CalendarIdentifiers.UmAlQura,
        };

        calendarIdentifier.ItemsSource = calendarIdentifiers;
        calendarIdentifier.SelectedItem = CalendarIdentifiers.Gregorian;

        var langs = new LanguageList();
        calendarLanguages.ItemsSource = langs.Languages;
    }

    private void SelectionMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Enum.TryParse<CalendarViewSelectionMode>((sender as ComboBox).SelectedItem.ToString(), out CalendarViewSelectionMode selectionMode))
        {
            Control1.SelectionMode = selectionMode;
        }
    }

    private void calendarLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selectedLang = calendarLanguages.SelectedValue.ToString();
        if (Windows.Globalization.Language.IsWellFormed(selectedLang))
        {
            Control1.Language = selectedLang;
        }
    }
}
