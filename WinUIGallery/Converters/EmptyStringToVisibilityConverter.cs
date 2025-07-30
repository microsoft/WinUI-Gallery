// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace WinUIGallery.Converters;

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
        else if (value is string stringValue && stringValue != "")
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
