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
            // If the implicit animation API is not present, simply no-op. 
            if (!(ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))) return;
            var customValue = EnsureValueIsNumber(OpacityNumberBox);
            OpacityRectangle.Opacity = customValue;
            OpacityValue.Value = customValue;
        }
        private void RotationButton_Click(object sender, RoutedEventArgs e)
        {
            RotationRectangle.CenterPoint = new System.Numerics.Vector3((float)RotationRectangle.ActualWidth / 2, (float)RotationRectangle.ActualHeight / 2, 0f);

            RotationRectangle.Rotation = EnsureValueIsNumber(RotationNumberBox);
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
                customValue = (float)Convert.ToDouble((sender as Button).Tag);
            }
            else
            {
                customValue = EnsureValueIsNumber(ScaleNumberBox);
            }

            ScaleRectangle.Scale = new Vector3(customValue);
            ScaleValue.Value = customValue;
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
                customValue = (float)Convert.ToDouble((sender as Button).Tag);
            }
            else
            {
                customValue = EnsureValueIsNumber(TranslationNumberBox);
            }

            TranslateRectangle.Translation = new Vector3(customValue);
            TranslationValue.Value = customValue;
        }

        private void NumberBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if ((string)(sender as NumberBox).Header == "Opacity (0.0 to 1.0)")
                {
                    OpacityButton_Click(null, null);
                }
                if ((string)(sender as NumberBox).Header == "Rotation (0.0 to 360.0)")
                {
                    RotationButton_Click(null, null);
                }
                if ((string)(sender as NumberBox).Header == "Scale (0.0 to 5.0)")
                {
                    ScaleButton_Click(null, null);
                }
                if ((string)(sender as NumberBox).Header == "Translation (0.0 to 200.0)")
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

        private float EnsureValueIsNumber(NumberBox numberBox)
        {
            if(double.IsNaN(numberBox.Value))
            {
                numberBox.Value = 0;
            }
            return (float)numberBox.Value;
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeExampleGrid.RequestedTheme = ThemeExampleGrid.RequestedTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}
