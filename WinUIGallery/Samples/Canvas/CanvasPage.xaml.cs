// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Automation;

namespace WinUIGallery.ControlPages;

public sealed partial class CanvasPage : Page
{
    public CanvasPage()
    {
        this.InitializeComponent();
        UpdateZSliderAutomationName();
    }

    private void ZSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        UpdateZSliderAutomationName();
    }

    private void UpdateZSliderAutomationName()
    {
        int currentValue = (int)ZSlider.Value;
        int minimumValue = (int)ZSlider.Minimum;
        int maximumValue = (int)ZSlider.Maximum;

        string automationName = $"Canvas.ZIndex value {currentValue} of range {minimumValue} to {maximumValue}";
        AutomationProperties.SetName(ZSlider, automationName);
    }
}
