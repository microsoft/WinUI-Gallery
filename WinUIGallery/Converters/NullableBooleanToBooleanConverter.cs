using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace WinUIGallery.Converters;

public class NullableBooleanToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is bool? ? (bool)value : (object)false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is bool ? (bool)value : (object)false;
    }
}
