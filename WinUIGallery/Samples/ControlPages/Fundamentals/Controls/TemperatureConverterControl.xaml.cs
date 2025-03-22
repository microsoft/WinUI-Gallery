using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Samples.ControlPages.Fundamentals.Controls;

public sealed partial class TemperatureConverterControl : UserControl
{
    public TemperatureConverterControl()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string input = InputTextBox.Text;
        double celsius;
        bool isNumber = double.TryParse(input, out celsius);

        if (isNumber)
        {
            double fahrenheit = (celsius * 9 / 5) + 32;
            ResultTextBlock.Text = "Fahrenheit: " + fahrenheit.ToString("F2") + "°F";
        }
        else
        {
            ResultTextBlock.Text = "Invalid input!";
        }
    }
}
