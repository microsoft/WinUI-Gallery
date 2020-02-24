using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Reflection;
using Windows.Globalization;
using Windows.UI.Popups;
using AppUIBasics.Common;

namespace AppUIBasics.ControlPages
{
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
}
