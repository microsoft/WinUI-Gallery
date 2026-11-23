// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

/// <summary>
/// UIElement.ProtectedCursor is protected, so a derived element exposes it.
/// </summary>
public sealed partial class CursorGrid : Grid
{
    public void SetCursor(InputCursor cursor)
    {
        ProtectedCursor = cursor;
    }
}

public sealed partial class InputCursorPage : Page
{
    public InputSystemCursorShape[] CursorShapes { get; } =
    [
        InputSystemCursorShape.Arrow,
        InputSystemCursorShape.Cross,
        InputSystemCursorShape.Hand,
        InputSystemCursorShape.Help,
        InputSystemCursorShape.IBeam,
        InputSystemCursorShape.Person,
        InputSystemCursorShape.Pin,
        InputSystemCursorShape.SizeAll,
        InputSystemCursorShape.SizeNortheastSouthwest,
        InputSystemCursorShape.SizeNorthSouth,
        InputSystemCursorShape.SizeNorthwestSoutheast,
        InputSystemCursorShape.SizeWestEast,
        InputSystemCursorShape.UniversalNo,
        InputSystemCursorShape.UpArrow,
        InputSystemCursorShape.Wait,
    ];

    public InputCursorPage()
    {
        InitializeComponent();
    }

    private void CursorArea_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateCursor();
    }

    private void CursorShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Don't change the cursor before the element has loaded.
        if (CursorArea?.IsLoaded == true)
        {
            UpdateCursor();
        }
    }

    private void UpdateCursor()
    {
        if (CursorShapeComboBox.SelectedItem is InputSystemCursorShape shape)
        {
            CursorArea.SetCursor(InputSystemCursor.Create(shape));
        }
    }
}