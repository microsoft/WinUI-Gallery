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
        switch(ColorSpectrumShapeRadioButtons.SelectedItem)
        {
            case "Box":
                colorPicker.ColorSpectrumShape = ColorSpectrumShape.Box;
                break;
            default:
                colorPicker.ColorSpectrumShape = ColorSpectrumShape.Ring;
                break;
        }
    }
}
