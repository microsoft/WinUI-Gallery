using System;
using System.Numerics;
using Windows.Foundation.Metadata;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace AppUIBasics.ControlPages
{
    public sealed partial class XamlCompInteropPage : Page
    {
        public XamlCompInteropPage()
        {
            this.InitializeComponent();
        }

        Compositor _compositor = Window.Current.Compositor;
        private SpringVector3NaturalMotionAnimation _springAnimation;

        private void NaturalMotionExample_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSpringAnimation(1.0f);
        }

        private void UpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
            }

            _springAnimation.FinalValue = new Vector3(finalValue);
            _springAnimation.DampingRatio = GetDampingRatio();
            _springAnimation.Period = GetPeriod();
        }

        float GetDampingRatio()
        {
            foreach (RadioButton rb in DampingStackPanel.Children)
            {
                if (rb.IsChecked == true)
                {
                    return (float)Convert.ToDouble(rb.Content);
                }
            }
            return 0.6f;
        }

        TimeSpan GetPeriod()
        {
            return TimeSpan.FromMilliseconds(PeriodSlider.Value);
        }

        private void StartAnimationIfAPIPresent(UIElement sender, Microsoft.UI.Composition.CompositionAnimation animation)
        {
            (sender as UIElement).StartAnimation(animation);
        }

        private void element_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UpdateSpringAnimation(1.5f);

            StartAnimationIfAPIPresent((sender as UIElement), _springAnimation);
        }

        private void element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UpdateSpringAnimation(1f);

            StartAnimationIfAPIPresent((sender as UIElement), _springAnimation);
        }


        private void ExpressionSample_Loaded(object sender, RoutedEventArgs e)
        {
            var anim = _compositor.CreateExpressionAnimation();
            anim.Expression = "Vector3(1/scaleElement.Scale.X, 1/scaleElement.Scale.Y, 1)";
            anim.Target = "Scale";

            anim.SetExpressionReferenceParameter("scaleElement", rectangle);            

            StartAnimationIfAPIPresent(ellipse, anim);
        }

        private void StackedButtonsExample_Loaded(object sender, RoutedEventArgs e)
        {
            var anim = _compositor.CreateExpressionAnimation();
            anim.Expression = "(above.Scale.Y - 1) * 50 + above.Translation.Y % (50 * index)";
            anim.Target = "Translation.Y";

            anim.SetExpressionReferenceParameter("above", ExpressionButton1);
            anim.SetScalarParameter("index", 1);
            ExpressionButton2.StartAnimation(anim);

            anim.SetExpressionReferenceParameter("above", ExpressionButton2);
            anim.SetScalarParameter("index", 2);
            ExpressionButton3.StartAnimation(anim);

            anim.SetExpressionReferenceParameter("above", ExpressionButton3);
            anim.SetScalarParameter("index", 3);
            ExpressionButton4.StartAnimation(anim);
        }

        private void ActualSizeExample_Loaded(object sender, RoutedEventArgs e)
        {
            // We will lay out some buttons in a circle.
            // The formulas we will use are:
            //   X = radius * cos(theta) + xOffset
            //   Y = radius * sin(theta) + yOffset
            //   radius = 1/2 the width and height of the parent container
            //   theta = the angle for each element. The starting value of theta depends on both the number of elements and the relative index of each element.
            //   xOffset = The starting horizontal offset for the element. 
            //   yOffset = The starting vertical offset for the element.

            String radius = "(source.ActualSize.X / 2)"; // Since the layout is a circle, width and height are equivalent meaning we could use X or Y. We'll use X.
            String theta = ".02 * " + radius + " + ((2 * Pi)/total)*index"; // The first value is the rate of angular change based on radius. The last value spaces the buttons equally.
            String xOffset = radius; // We offset x by radius because the buttons naturally layout along the left edge. We need to move them to center of the circle first.
            String yOffset = "0"; // We don't need to offset y because the buttons naturally layout vertically centered.

            // We combine X, Y, and Z subchannels into a single animation because we can only start a single animation on Translation.
            String expression = String.Format("Vector3({0}*cos({1})+{2}, {0}*sin({1})+{3},0)", radius, theta, xOffset, yOffset);

            int totalElements = 8;
            for (int i = 0; i < totalElements; i++)
            {
                Button element = new Button() { Content = "Button" };
                AutomationProperties.SetName(element, "Button " + i);

                LayoutPanel.Children.Add(element);

                var anim = _compositor.CreateExpressionAnimation();

                anim.Expression = expression;
                anim.SetScalarParameter("index", i + 1);
                anim.SetScalarParameter("total", totalElements);
                anim.Target = "Translation";
                anim.SetExpressionReferenceParameter("source", LayoutPanel);

                element.StartAnimation(anim);
            }
        }

        private void RadiusSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (LayoutPanel == null) return;
            LayoutPanel.Width = LayoutPanel.Height = e.NewValue;
        }

        private void ActualOffsetExample_Loaded(object sender, RoutedEventArgs e)
        {
            // This sample positions a popup relative to a block of text that has variable layout size based on font size.
            var anim = _compositor.CreateExpressionAnimation();

            anim.Expression = "Vector3(source.ActualOffset.X + source.ActualSize.X, source.ActualOffset.Y + source.ActualSize.Y / 2 - 25, 0)";
            anim.Target = "Translation";
            anim.SetExpressionReferenceParameter("source", PopupTarget);

            Popup.StartAnimation(anim);

            Popup.IsOpen = true;
        }
    }
}
