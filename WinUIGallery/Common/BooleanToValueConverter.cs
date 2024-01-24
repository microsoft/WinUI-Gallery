using System;
using Microsoft.UI.Xaml.Data;

namespace WinUIGallery.Common
{
    public sealed partial class BooleanToValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? parameter : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
