using Microsoft.UI.Xaml;

namespace WinUIGallery.Controls;

/// <summary>
/// Represents a selectable item in the <c>AdaptiveSelectorBar</c>.
/// Holds display text, an icon glyph, selection state, and optional user data.
/// </summary>
public partial class AdaptiveSelectorBarItem : DependencyObject
{
    /// <summary>
    /// Identifies the <see cref="DisplayText"/> dependency property.
    /// Used for the textual label shown on the item.
    /// </summary>
    public static readonly DependencyProperty DisplayTextProperty =
        DependencyProperty.Register(
            nameof(DisplayText),
            typeof(string),
            typeof(AdaptiveSelectorBarItem),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="FontIconGlyph"/> dependency property.
    /// Holds a Unicode glyph (e.g., from Segoe MDL2 Assets) used as the item's icon.
    /// </summary>
    public static readonly DependencyProperty FontIconGlyphProperty =
        DependencyProperty.Register(
            nameof(FontIconGlyph),
            typeof(string),
            typeof(AdaptiveSelectorBarItem),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> dependency property.
    /// Indicates whether this item is currently selected.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(AdaptiveSelectorBarItem),
            new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="Tag"/> dependency property.
    /// Allows arbitrary user-defined data to be associated with this item.
    /// </summary>
    public static readonly DependencyProperty TagProperty =
        DependencyProperty.Register(
            nameof(Tag),
            typeof(object),
            typeof(AdaptiveSelectorBarItem),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the display text shown for this item.
    /// </summary>
    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the glyph icon displayed alongside the text.
    /// Should be a Unicode character from a supported icon font.
    /// </summary>
    public string FontIconGlyph
    {
        get => (string)GetValue(FontIconGlyphProperty);
        set => SetValue(FontIconGlyphProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this item is currently selected.
    /// Used for styling and selection logic.
    /// </summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional value to associate with the item.
    /// Often used for IDs, data models, or navigation parameters.
    /// </summary>
    public object Tag
    {
        get => GetValue(TagProperty);
        set => SetValue(TagProperty, value);
    }
}