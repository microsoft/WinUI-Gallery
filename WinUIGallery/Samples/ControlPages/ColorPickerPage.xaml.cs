using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class ColorPickerPage : Page
{
    public ColorPickerPage()
    {
        InitializeComponent();
    }

    private void ColorSpectrumShapeRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        colorPicker.ColorSpectrumShape = ColorSpectrumShapeRadioButtons.SelectedItem switch
        {
            "Box" => ColorSpectrumShape.Box,
            _ => ColorSpectrumShape.Ring,
        };
    }
}
