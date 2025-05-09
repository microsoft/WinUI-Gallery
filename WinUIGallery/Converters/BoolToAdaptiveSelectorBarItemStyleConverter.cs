using Microsoft.UI.Xaml.Data;
using System;

namespace WinUIGallery.Converters;

/// <summary>
/// Converts a boolean value (IsSelected) into a corresponding button style.
/// Used to apply different styles for selected vs unselected items in the AdaptiveSelectorBar.
/// </summary>
public partial class BoolToAdaptiveSelectorBarItemButtonStyleConverter : IValueConverter
{
    /// <summary>
    /// Returns the style resource based on the boolean input.
    /// </summary>
    /// <param name="value">Expected to be a bool (true if selected).</param>
    /// <returns>Selected or unselected style from app resources.</returns>
    public object Convert(object value, Type targetType, object parameter, string language) =>
        App.Current.Resources[(value is bool b && b)
            ? "SelectedAdaptiveSelectorBarItemStyle"
            : "UnselectedAdaptiveSelectorBarItemStyle"];

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException();
}