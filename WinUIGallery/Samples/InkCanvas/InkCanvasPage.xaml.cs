// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using InkDrawingAttributes = Windows.UI.Input.Inking.InkDrawingAttributes;
using PenTipShape = Windows.UI.Input.Inking.PenTipShape;

namespace WinUIGallery.ControlPages;

public sealed partial class InkCanvasPage : Page, INotifyPropertyChanged
{
    private readonly InkPresenter _inkPresenter;

    public event PropertyChangedEventHandler? PropertyChanged;

    public InkCanvasPage()
    {
        InitializeComponent();

        _inkPresenter = BasicInkCanvas.InkPresenter;
        _inkPresenter.InputDeviceTypes =
            Windows.UI.Core.CoreInputDeviceTypes.Mouse |
            Windows.UI.Core.CoreInputDeviceTypes.Pen |
            Windows.UI.Core.CoreInputDeviceTypes.Touch;

        UpdatePen();
    }

    public string PenColorName => PenColorComboBox?.SelectedValue?.ToString() ?? "Black";

    public string StrokeSizeText => StrokeSizeSlider is null ? "5" : StrokeSizeSlider.Value.ToString("0");

    public string DrawAsHighlighterText => HighlighterCheckBox?.IsChecked == true ? "true" : "false";

    public string PenTipName => CirclePenTipRadioButton?.IsChecked == true ? "Circle" : "Rectangle";

    private void PenOption_Changed(object sender, RoutedEventArgs e) => UpdatePen();

    private void StrokeSizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) => UpdatePen();

    private void ClearAllButton_Click(object sender, RoutedEventArgs e) => _inkPresenter.StrokeContainer.Clear();

    private void UpdatePen()
    {
        if (_inkPresenter is null)
        {
            return;
        }

        InkDrawingAttributes attributes = _inkPresenter.CopyDefaultDrawingAttributes();

        attributes.Color = PenColorName switch
        {
            "Red" => Colors.Red,
            "Blue" => Colors.Blue,
            "Green" => Colors.Green,
            _ => Colors.Black,
        };

        double size = StrokeSizeSlider.Value;
        attributes.Size = new Size(size, size);
        attributes.DrawAsHighlighter = HighlighterCheckBox.IsChecked == true;
        attributes.PenTip = CirclePenTipRadioButton.IsChecked == true ? PenTipShape.Circle : PenTipShape.Rectangle;

        _inkPresenter.UpdateDefaultDrawingAttributes(attributes);

        NotifyPropertyChanged(nameof(PenColorName));
        NotifyPropertyChanged(nameof(StrokeSizeText));
        NotifyPropertyChanged(nameof(DrawAsHighlighterText));
        NotifyPropertyChanged(nameof(PenTipName));
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
