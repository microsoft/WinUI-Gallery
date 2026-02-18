using System;
using System.Collections.Generic;

namespace WinUIGallery.Helpers;

public partial class SamplesNavigationPageMappings
{
    public static Dictionary<string, Type> PageDictionary { get; } = new Dictionary<string, Type>
    {
        {"WinUIGallery.SamplePages.CardPage", typeof(WinUIGallery.SamplePages.CardPage)},
        {"WinUIGallery.SamplePages.XamlStylesPage", typeof(WinUIGallery.SamplePages.CollectionPage)},
        {"WinUIGallery.SamplePages.BindingPage", typeof(WinUIGallery.SamplePages.DetailedInfoPage)},
        {"WinUIGallery.SamplePages.SampleCompactSizingPage", typeof(WinUIGallery.SamplePages.SampleCompactSizingPage)},
        {"WinUIGallery.SamplePages.SamplePage1", typeof(WinUIGallery.SamplePages.SamplePage1)},
        {"WinUIGallery.SamplePages.SamplePage2", typeof(WinUIGallery.SamplePages.SamplePage2)},
        {"WinUIGallery.SamplePages.SamplePage3", typeof(WinUIGallery.SamplePages.SamplePage3)},
        {"WinUIGallery.SamplePages.SamplePage4", typeof(WinUIGallery.SamplePages.SamplePage4)},
        {"WinUIGallery.SamplePages.SamplePage5", typeof(WinUIGallery.SamplePages.SamplePage5)},
        {"WinUIGallery.SamplePages.SamplePage6", typeof(WinUIGallery.SamplePages.SamplePage6)},
        {"WinUIGallery.SamplePages.SamplePage7", typeof(WinUIGallery.SamplePages.SamplePage7)},
        {"WinUIGallery.SamplePages.SampleStandardSizingPage", typeof(WinUIGallery.SamplePages.SampleStandardSizingPage)},
        {"WinUIGallery.SamplePages.TabViewWindowingSamplePage", typeof(WinUIGallery.SamplePages.TabViewWindowingSamplePage)},
    };
}
