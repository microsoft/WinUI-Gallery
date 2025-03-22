using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace WinUIGallery.ControlPages;

public class NamedEasingFunction
{
    public string Name { get; private set; }
    public EasingFunctionBase EasingFunctionBase { get; private set; }
    public NamedEasingFunction(string name, EasingFunctionBase easingFunctionBase)
    {
        Name = name;
        EasingFunctionBase = easingFunctionBase;
    }
}

public sealed partial class EasingFunctionPage : Page
{
    private List<NamedEasingFunction> EasingFunctions { get; } =
        [
        new("BackEase", new BackEase()),
        new("BounceEase", new BounceEase()),
        new("CircleEase", new CircleEase()),
        new("CubicEase", new CubicEase()),
        new("ElasticEase", new ElasticEase()),
        new("ExponentialEase", new ExponentialEase()),
        new("PowerEase", new PowerEase()),
        new("QuadraticEase", new QuadraticEase()),
        new("QuarticEase", new QuarticEase()),
        new("QuinticEase", new QuinticEase()),
        new("SineEase", new SineEase())
        ];

    public EasingFunctionPage()
    {
        InitializeComponent();
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

    private void EasingComboBox_Loaded(object sender, RoutedEventArgs e) => EasingComboBox.SelectedIndex = 0;
}
