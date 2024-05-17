using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Reflection;
using Windows.Globalization;
using Windows.UI.Popups;
using WinUIGallery.Common;
using System.Diagnostics.CodeAnalysis;

namespace WinUIGallery.ControlPages
{
    public sealed partial class CalendarViewPage : Page
    {
        // ICustomProperty provider is not AOT safe yet, so declaring here that we make use of public properties with reflection.
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(WinUIGallery.Common.LanguageList.Language))]
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
