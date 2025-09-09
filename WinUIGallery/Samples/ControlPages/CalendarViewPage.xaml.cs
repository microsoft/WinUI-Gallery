// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Globalization;
using WinUIGallery.Helpers;
using Language = WinUIGallery.Helpers.Language;

namespace WinUIGallery.ControlPages;

public sealed partial class CalendarViewPage : Page
{
    public ObservableCollection<Language> Languages { get; set; } = new(new LanguageList().Languages);
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
        var selectedLang = calendarLanguages.SelectedItem as Language;
        if (Windows.Globalization.Language.IsWellFormed(selectedLang.Code) && selectedLang != null)
        {
            Control1.Language = selectedLang.Code;
        }
    }
}
