using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace WinUIGallery.Common
{
    public partial class EmptyStringToVisibilityConverter : IValueConverter
    {
        public Visibility EmptyValue { get; set; } = Visibility.Collapsed;
        public Visibility NonEmptyValue { get; set; } = Visibility.Visible;


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return EmptyValue;
            }
            else if(value is string stringValue && stringValue != "")
            {
                return NonEmptyValue;
            }
            else
            {
                return EmptyValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
