// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class InkToolbarPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public InkToolbarPage()
    {
        InitializeComponent();
    }

    public string OrientationName => OrientationComboBox?.SelectedValue?.ToString() ?? "Horizontal";

    private void ToolbarOption_Changed(object sender, SelectionChangedEventArgs e)
    {
        BasicInkToolbar.Orientation = OrientationName == "Vertical"
            ? Orientation.Vertical
            : Orientation.Horizontal;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrientationName)));
    }

    private void ClearAllButton_Click(object sender, RoutedEventArgs e)
    {
        BasicInkCanvas.InkPresenter.StrokeContainer.Clear();
    }
}
