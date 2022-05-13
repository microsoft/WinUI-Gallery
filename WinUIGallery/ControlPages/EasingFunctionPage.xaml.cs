using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public class NamedEasingFunction
    {
        public string Name { get; private set; }
        public EasingFunctionBase EasingFunctionBase { get; private set; }
        public NamedEasingFunction(string name, EasingFunctionBase easingFunctionBase)
        {
            this.Name = name;
            this.EasingFunctionBase = easingFunctionBase;
        }
    }

    public sealed partial class EasingFunctionPage : Page
    {
        private List<NamedEasingFunction> EasingFunctions { get; } = new List<NamedEasingFunction>()
            {
            new NamedEasingFunction("BackEase", new BackEase()),
            new NamedEasingFunction("BounceEase", new BounceEase()),
            new NamedEasingFunction("CircleEase", new CircleEase()),
            new NamedEasingFunction("CubicEase", new CubicEase()),
            new NamedEasingFunction("ElasticEase", new ElasticEase()),
            new NamedEasingFunction("ExponentialEase", new ExponentialEase()),
            new NamedEasingFunction("PowerEase", new PowerEase()),
            new NamedEasingFunction("QuadraticEase", new QuadraticEase()),
            new NamedEasingFunction("QuarticEase", new QuarticEase()),
            new NamedEasingFunction("QuinticEase", new QuinticEase()),
            new NamedEasingFunction("SineEase", new SineEase())
            };

        public EasingFunctionPage()
        {
            this.InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Storyboard1.Children[0].SetValue(DoubleAnimation.FromProperty, Translation1.X);
            Storyboard1.Children[0].SetValue(DoubleAnimation.ToProperty, Translation1.X > 0 ? 0 : 200);
            Storyboard1.Begin();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Storyboard2.Children[0].SetValue(DoubleAnimation.FromProperty, Translation2.X);
            Storyboard2.Children[0].SetValue(DoubleAnimation.ToProperty, Translation2.X > 0 ? 0 : 200);
            Storyboard2.Begin();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Storyboard3.Children[0].SetValue(DoubleAnimation.FromProperty, Translation3.X);
            Storyboard3.Children[0].SetValue(DoubleAnimation.ToProperty, Translation3.X > 0 ? 0 : 200);
            Storyboard3.Begin();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            var easingFunction = EasingComboBox.SelectedValue as EasingFunctionBase;
            easingFunction.EasingMode = GetEaseValue();
            (Storyboard4.Children[0] as DoubleAnimation).EasingFunction = easingFunction;

            Storyboard4.Children[0].SetValue(DoubleAnimation.FromProperty, Translation4.X);
            Storyboard4.Children[0].SetValue(DoubleAnimation.ToProperty, Translation4.X > 0 ? 0 : 200);
            Storyboard4.Begin();
        }

        EasingMode GetEaseValue()
        {
            if (easeOutRB.IsChecked == true) return EasingMode.EaseOut;
            else if (easeInRB.IsChecked == true) return EasingMode.EaseIn;
            else return EasingMode.EaseInOut;
        }

        private void EasingComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            EasingComboBox.SelectedIndex = 0;
        }
    }
}
