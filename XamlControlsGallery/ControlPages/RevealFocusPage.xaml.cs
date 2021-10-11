//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class RevealFocusPage : Page
    {
        public RevealFocusPage()
        {
            this.InitializeComponent();

            if (Spring2018 && Application.Current.FocusVisualKind == FocusVisualKind.Reveal)
            {
                RevealFocus.IsChecked = true;
                myPrimaryColorPicker.Color = (this.Resources["SystemControlRevealFocusVisualBrush"] as SolidColorBrush).Color;
                mySecondaryColorPicker.Color = (this.Resources["SystemControlFocusVisualSecondaryBrush"] as SolidColorBrush).Color;
                primaryColorPickerButton.Background = new SolidColorBrush(myPrimaryColorPicker.Color);
                secondaryColorPickerButton.Background = new SolidColorBrush(mySecondaryColorPicker.Color);
            }
            else
            {
                HighVisibility.IsChecked = true;
                primaryColorPickerButton.Background = new SolidColorBrush(myPrimaryColorPicker.Color);
                secondaryColorPickerButton.Background = new SolidColorBrush(mySecondaryColorPicker.Color);
            }
        }

        private void HighVisibility_Checked(object sender, RoutedEventArgs e)
        {
            if (exampleButton.ActualTheme == ElementTheme.Light)
            {
                myPrimaryColorPicker.Color = Colors.Black;
                mySecondaryColorPicker.Color = Colors.White;
            }
            else if (exampleButton.ActualTheme == ElementTheme.Dark)
            {
                myPrimaryColorPicker.Color = Colors.White;
                mySecondaryColorPicker.Color = Colors.Black;
            }

            primaryColorPickerButton.Background = new SolidColorBrush(myPrimaryColorPicker.Color);
            primaryBrushText.Value = "{StaticResource SystemControlFocusVisualPrimaryBrush}";
            primaryColorKeyText.Value = "SystemControlFocusVisualPrimaryBrush";
            Application.Current.FocusVisualKind = FocusVisualKind.HighVisibility;
            FocusVisualKindSubstitution.Value = "HighVisibility";
        }

        private void RevealFocus_Checked(object sender, RoutedEventArgs e)
        {
            if (Spring2018)
            {
                myPrimaryColorPicker.Color = (this.Resources["SystemControlRevealFocusVisualBrush"] as SolidColorBrush).Color;
                primaryColorPickerButton.Background = new SolidColorBrush(myPrimaryColorPicker.Color);
                primaryBrushText.Value = "{StaticResource SystemControlRevealFocusVisualBrush}";
                primaryColorKeyText.Value = "SystemControlRevealFocusVisualBrush";
                Application.Current.FocusVisualKind = FocusVisualKind.Reveal;
                FocusVisualKindSubstitution.Value = "Reveal";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Draw the focus visuals at the edge of the control
            // A negative FocusVisualMargin outsets the focus visual. A positive one insets the focus visual
            marginSlider.Value = -1 * (primarySlider.Value + secondarySlider.Value);
        }

        private void confirmColor_Click(object sender, RoutedEventArgs e)
        {
            AutomationProperties.SetName(primaryColorPickerButton,
                "Select primary color, currently selected: " + ColorHelper.ToDisplayName(myPrimaryColorPicker.Color) + " , " + myPrimaryColorPicker.Color.ToString());
            AutomationProperties.SetName(secondaryColorPickerButton,
                "Select secondary color, currently selected: " + ColorHelper.ToDisplayName(mySecondaryColorPicker.Color) + " , " + mySecondaryColorPicker.Color.ToString());

            primaryColorPickerButton.Background = new SolidColorBrush(myPrimaryColorPicker.Color);
            secondaryColorPickerButton.Background = new SolidColorBrush(mySecondaryColorPicker.Color);

            primaryColorPickerButton.Flyout.Hide();
            secondaryColorPickerButton.Flyout.Hide();
        }

        /// <summary>
        /// A property to identify if app is running on Spring2018 version of Windows
        /// </summary>
        public bool Spring2018
        {
            get
            {
                return ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6);
            }
        }

        private void Example2_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (Example2.ActualTheme == ElementTheme.Light)
            {
                myPrimaryColorPicker.Color = Colors.Black;
                mySecondaryColorPicker.Color = Colors.White;
            }
            else if (Example2.ActualTheme == ElementTheme.Dark)
            {
                myPrimaryColorPicker.Color = Colors.White;
                mySecondaryColorPicker.Color = Colors.Black;
            }
        }

        private void MoveFocusBtn_Click(object sender, RoutedEventArgs e)
        {
            // Set focus to button for better preview/demo of customization
            exampleButton.Focus(FocusState.Keyboard);
        }
    }

    public class MyConverters
    {
        public static Thickness IntToThickness(double UniformLength)
        {
            return new Thickness(UniformLength);
        }

        public static SolidColorBrush ColorToBrush(Color color)
        {
            return new SolidColorBrush(color);
        }
    }
}
