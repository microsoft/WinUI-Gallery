// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace WinUIGallery.Converters;

public sealed partial class BooleanToValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isChecked && isChecked)
        {
            if (parameter is string str)
            {
                // Check if it's a URL (basic check)
                if (str.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    str.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return new BitmapImage(new Uri(str));
                }

                // Return the string directly if not a URL (for DisplayName or Initials)
                return str;
            }

            return parameter;
        }

        // If not checked, return null (clears DisplayName, Initials, or Image)
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
