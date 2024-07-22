using Microsoft.UI.Xaml.Media;
using WinRT;

namespace WinUIGallery.Common
{
    [BindableCustomProperty]
    public sealed partial class FontFamilyTuple
    {
        public string Name { get; }
        public FontFamily FontFamily { get; }

        public FontFamilyTuple(string name, FontFamily fontFamily)
        {
            Name = name;
            FontFamily = fontFamily;
        }
    }
}
