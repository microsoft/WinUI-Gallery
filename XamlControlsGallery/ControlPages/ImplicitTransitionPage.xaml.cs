using System;
using System.Numerics;
using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ImplicitTransitionPage : Page
    {
        public ImplicitTransitionPage()
        {
            this.InitializeComponent();

            SetupImplicitTransitionsIfAPIAvailable();
        }

        void SetupImplicitTransitionsIfAPIAvailable()
        {
            OpacityRectangle.OpacityTransition = new ScalarTransition();
            RotationRectangle.RotationTransition = new ScalarTransition();
            ScaleRectangle.ScaleTransition = new Vector3Transition();
            TranslateRectangle.TranslationTransition = new Vector3Transition();
            BrushPresenter.BackgroundTransition = new BrushTransition();
            ThemeExampleGrid.BackgroundTransition = new BrushTransition();
        }

        private void OpacityButton_Click(object sender, RoutedEventArgs e)
        {
            var customValue = 0.0;
            try
            {
                customValue = Convert.ToDouble(OpacityTextBox.Text);
            }
            catch (FormatException)
            {
                GenerateErrorFlyout(OpacityTextBox);
                return;
            }

            if (customValue >= 0.0 && customValue <= 1.0)
            {
                OpacityRectangle.Opacity = customValue;
                OpacityValue.Value = customValue;
            }
            else
            {
                GenerateErrorFlyout(OpacityTextBox, "Input must be between 0 and 1");
            }
        }
        private void RotationButton_Click(object sender, RoutedEventArgs e)
        {
            RotationRectangle.CenterPoint = new System.Numerics.Vector3((float)RotationRectangle.ActualWidth / 2, (float)RotationRectangle.ActualHeight / 2, 0f);

            float customValue = 0;
            try
            {
                customValue = (float)Convert.ToDouble(RotationTextBox.Text);
            }
            catch (FormatException)
            {
                GenerateErrorFlyout(RotationTextBox);
                return;
            }

            if (customValue >= 0.0 && customValue <= 360.0)
            {
                RotationRectangle.Rotation = customValue;
            }
            else
            {
                RotationRectangle.Rotation = 0;
                GenerateErrorFlyout(RotationTextBox, "Input must be between 0 and 360");
            }
            RotationValue.Value = RotationRectangle.Rotation;
        }
        private void ScaleButton_Click(object sender, RoutedEventArgs e)
        {
            var _scaleTransition = ScaleRectangle.ScaleTransition;

            _scaleTransition.Components = ((ScaleX.IsChecked == true) ? Vector3TransitionComponents.X : 0) |
                                         ((ScaleY.IsChecked == true) ? Vector3TransitionComponents.Y : 0) |
                                         ((ScaleZ.IsChecked == true) ? Vector3TransitionComponents.Z : 0);

            float customValue;

            if (sender != null && (sender as Button).Tag != null)
            {
                try
                {
                    customValue = (float)Convert.ToDouble((sender as Button).Tag);
                }
                catch (FormatException)
                {
                    GenerateErrorFlyout(sender as Button);
                    return;
                }
            }
            else
            {
                try
                {
                    customValue = (float)Convert.ToDouble(ScaleTextBox.Text);
                }
                catch (FormatException)
                {
                    GenerateErrorFlyout(ScaleTextBox);
                    return;
                }
            }

            if (customValue > 0.0 && customValue <= 5)
            {
                ScaleRectangle.Scale = new Vector3(customValue);
                ScaleValue.Value = customValue;
            }
            else
            {
                GenerateErrorFlyout(ScaleTextBox, "The number must be between 0 (not equal to 0) and 5 ");
            }
        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            var _translationTransition = TranslateRectangle.TranslationTransition;

            _translationTransition.Components = ((TranslateX.IsChecked == true) ? Vector3TransitionComponents.X : 0) |
                                         ((TranslateY.IsChecked == true) ? Vector3TransitionComponents.Y : 0) |
                                         ((TranslateZ.IsChecked == true) ? Vector3TransitionComponents.Z : 0);

            float customValue;
            if (sender != null && (sender as Button).Tag != null)
            {
                try
                {
                    customValue = (float)Convert.ToDouble((sender as Button).Tag);
                }
                catch (FormatException)
                {
                    GenerateErrorFlyout(sender as Button);
                    return;
                }
            }
            else
            {
                try
                {

                    customValue = (float)Convert.ToDouble(TranslationTextBox.Text);
                }
                catch (FormatException)
                {
                    GenerateErrorFlyout(TranslationTextBox);
                    return;
                }
            }

            if (customValue >= 0.0 && customValue <= 200.0)
            {
                TranslateRectangle.Translation = new Vector3(customValue);
                TranslationValue.Value = customValue;
            }
            else
            {
                GenerateErrorFlyout(TranslationTextBox, "THe input must be between 0 and 200");
            }
        }

        /// <summary>
        /// Generates a flyout for the given element. 
        /// If no message is specified the default message "The input must be a number" will be used.
        /// </summary>
        /// <param name="element">The element to apply the flyout to</param>
        /// <param name="message">The message that will be displayed</param>
        private void GenerateErrorFlyout(FrameworkElement element, string message = "The input must be a number")
        {
            Flyout formatFlyout = new Flyout();
            formatFlyout.Content = new TextBlock();
            formatFlyout.FlyoutPresenterStyle = FlyoutPresenterStyle;
            (formatFlyout.Content as TextBlock).Text = message;
            formatFlyout.Placement = FlyoutPlacementMode.Top;
            formatFlyout.ShowAt(element);
            (formatFlyout.Content as TextBlock).Focus(FocusState.Programmatic);
        }
        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if ((String)(sender as TextBox).Header == "Opacity (0.0 to 1.0)")
                {
                    OpacityButton_Click(null, null);
                }
                if ((String)(sender as TextBox).Header == "Rotation (0.0 to 360.0)")
                {
                    RotationButton_Click(null, null);
                }
                if ((String)(sender as TextBox).Header == "Scale (0.0 to 5.0)")
                {
                    ScaleButton_Click(null, null);
                }
                if ((String)(sender as TextBox).Header == "Translation (0.0 to 200.0)")
                {
                    TranslateButton_Click(null, null);
                }
            }
        }

        private void BackgroundButton_Click(object sender, RoutedEventArgs e)
        {

            if ((BrushPresenter.Background as SolidColorBrush).Color == Microsoft.UI.Colors.Blue)
            {
                BrushPresenter.Background = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
            }
            else
            {
                BrushPresenter.Background = new SolidColorBrush(Microsoft.UI.Colors.Blue);
            }
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeExampleGrid.RequestedTheme = ThemeExampleGrid.RequestedTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}
