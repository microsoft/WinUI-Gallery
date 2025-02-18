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
        SampleWindow1 appWindow = new SampleWindow1((string)WindowTitle.SelectedItem, (Int32)WindowWidth.Value, (Int32)WindowHeight.Value, (Int32)XPoint.Value, (Int32)YPoint.Value);
        appWindow.Activate();
    }
}
