using System;
using Microsoft.UI.Xaml.Data;

namespace WinUIGallery.Converters;

public class NullableBooleanToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => value is bool? ? (bool)value : (object)false;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => value is bool ? (bool)value : (object)false;
}
