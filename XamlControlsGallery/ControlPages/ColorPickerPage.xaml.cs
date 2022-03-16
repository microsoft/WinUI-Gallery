using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ColorPickerPage : Page
    {
        public ColorPickerPage()
        {
            this.InitializeComponent();
        }

        private void ColorSpectrumShapeRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(ColorSpectrumShapeRadioButtons.SelectedItem)
            {
                case "Box":
                    colorPicker.ColorSpectrumShape = Microsoft.UI.Xaml.Controls.ColorSpectrumShape.Box;
                    break;
                default:
                    colorPicker.ColorSpectrumShape = Microsoft.UI.Xaml.Controls.ColorSpectrumShape.Ring;
                    break;
            }
        }
    }
}
