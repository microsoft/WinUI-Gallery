// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUIGallery.Samples.ControlPages.Fundamentals.Controls;

public sealed partial class TemperatureConverterControl : UserControl, INotifyPropertyChanged
{
    private bool _isConvertButtonEnabled = false;

    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsConvertButtonEnabled
    {
        get => _isConvertButtonEnabled;
        set
        {
            if (_isConvertButtonEnabled != value)
            {
                _isConvertButtonEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public TemperatureConverterControl()
    {
        this.InitializeComponent();
        if (InputTextBox != null)
        {
            InputTextBox.TextChanged += InputTextBox_TextChanged;
            UpdateButtonState();
        }
    }

    private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        IsConvertButtonEnabled = InputTextBox != null && !string.IsNullOrWhiteSpace(InputTextBox.Text);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string input = InputTextBox.Text;
        double celsius = 0;

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

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
