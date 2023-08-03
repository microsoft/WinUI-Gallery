using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace AppUIBasics.Common
{
    public sealed class BooleanToInvertedVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Collapsed;
        }
    }
}
