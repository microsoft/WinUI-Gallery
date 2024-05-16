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
        // Workaround an issue in CsWinRT where it doesn't handle nested types correctly
        // in its Vtable lookup by defining our own entry for it.  Will be addressed in
        // upcoming CsWinRT versions.
        static CalendarViewPage()
        {
            WinRT.ComWrappersSupport.RegisterTypeComInterfaceEntriesLookup(LookupVtableEntries);
            WinRT.ComWrappersSupport.RegisterTypeRuntimeClassNameLookup(new Func<Type, string>(LookupRuntimeClassName));

            static System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[] LookupVtableEntries(Type type)
            {
                if (type.ToString() == "System.Collections.Generic.List`1[WinUIGallery.Common.LanguageList+Language]")
                {
                    _ = WinRT.GenericHelpers.IReadOnlyList_object.Initialized;
                    _ = WinRT.GenericHelpers.IEnumerable_object.Initialized;

                    return new System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry[]
                    {
                        new System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
                        {
                            IID = ABI.System.Collections.Generic.IReadOnlyListMethods<object>.IID,
                            Vtable = ABI.System.Collections.Generic.IReadOnlyListMethods<object>.AbiToProjectionVftablePtr
                        },
                        new System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
                        {
                            IID = ABI.System.Collections.Generic.IEnumerableMethods<object>.IID,
                            Vtable = ABI.System.Collections.Generic.IEnumerableMethods<object>.AbiToProjectionVftablePtr
                        },
                        new System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
                        {
                            IID = ABI.System.Collections.IListMethods.IID,
                            Vtable = ABI.System.Collections.IListMethods.AbiToProjectionVftablePtr
                        },
                        new System.Runtime.InteropServices.ComWrappers.ComInterfaceEntry
                        {
                            IID = ABI.System.Collections.IEnumerableMethods.IID,
                            Vtable = ABI.System.Collections.IEnumerableMethods.AbiToProjectionVftablePtr
                        },
                    };
                }

                return default;
            }

            static string LookupRuntimeClassName(Type type)
            {
                if (type.ToString() == "System.Collections.Generic.List`1[WinUIGallery.Common.LanguageList+Language]")
                {
                    return "Windows.Foundation.Collections.IVectorView`1<Object>";
                }

                return default;
            }
        }

        // ICustomProperty provider is today not AOT safe yet, so declaring here that we make use of public properties with reflection.
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
