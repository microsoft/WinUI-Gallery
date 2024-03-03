using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace WinUIGallery.Common
{
    partial class DoubleToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double?)
            {
                var val = (double)value;
                return new Thickness(val);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
