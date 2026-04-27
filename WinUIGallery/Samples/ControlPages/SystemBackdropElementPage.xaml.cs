// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Composition.SystemBackdrops;

namespace WinUIGallery.ControlPages;

public sealed partial class SystemBackdropElementPage : Page
{
    public SystemBackdropElementPage()
    {
        this.InitializeComponent();
    }

    private void BackdropTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DynamicBackdropHost == null || BackdropTypeComboBox == null)
            return;

        var selectedItem = BackdropTypeComboBox.SelectedItem as ComboBoxItem;
        if (selectedItem == null)
            return;

        string backdropType = selectedItem.Tag?.ToString() ?? "Acrylic";

        // Update the SystemBackdrop based on selection
        if (backdropType == "Acrylic")
        {
            DynamicBackdropHost.SystemBackdrop = new DesktopAcrylicBackdrop();
            if (Example1 != null)
            {
                Example1.XamlSource = "SystemBackdropElement/SystemBackdropElementAcrylic_xaml.txt";
            }
        }
        else if (backdropType == "Mica")
        {
            DynamicBackdropHost.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.Base };
            if (Example1 != null)
            {
                Example1.XamlSource = "SystemBackdropElement/SystemBackdropElementMica_xaml.txt";
            }
        }
        else if (backdropType == "MicaAlt")
        {
            DynamicBackdropHost.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
            if (Example1 != null)
            {
                Example1.XamlSource = "SystemBackdropElement/SystemBackdropElementMicaAlt_xaml.txt";
            }
        }
    }

    private void CornerRadiusSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (DynamicBackdropHost != null)
        {
            DynamicBackdropHost.CornerRadius = new CornerRadius(e.NewValue);
        }
    }
}
