using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIGallery.Samples.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class AppWindowPage : Page
{
    public AppWindowPage()
    {
        this.InitializeComponent();
    }

    private void ShowSampleWindow1(object sender, RoutedEventArgs e)
    {
        SampleWindow1 window = new SampleWindow1(WindowTitle.Text, (Int32)WindowWidth.Value, (Int32)WindowHeight.Value, (Int32)XPoint.Value, (Int32)YPoint.Value);
        window.Activate();
    }

    private void ShowSampleWindow2(object sender, RoutedEventArgs e)
    {
        SampleWindow2 window = new SampleWindow2(IsAlwaysOnTop.IsOn, IsMaximizable.IsOn, IsMinimizable.IsOn, IsResizable.IsOn, HasBorder.IsOn, HasTitleBar.IsOn);
        window.Activate();
    }

    private void HasBorder_Toggled(object sender, RoutedEventArgs e)
    {
        if(!HasBorder.IsOn)
        {
            HasTitleBar.IsOn = false;
        }
    }

    private void HasTitleBar_Toggled(object sender, RoutedEventArgs e)
    {
        if (HasTitleBar.IsOn)
        {
            HasBorder.IsOn = true;
        }
    }

    string BoolToLowerString(bool value) => value.ToString().ToLower();

    private void ShowSampleWindow4(object sender, RoutedEventArgs e)
    {
        SampleWindow4 window = new SampleWindow4();
        window.Activate();
    }
}
