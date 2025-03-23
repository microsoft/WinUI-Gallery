using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Samples.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class AppWindowPage : Page
{
    public AppWindowPage()
    {
        InitializeComponent();
    }

    private void ShowSampleWindow1(object sender, RoutedEventArgs e)
    {
        SampleWindow1 window = new(WindowTitle.Text, (int)WindowWidth.Value, (int)WindowHeight.Value, (int)XPoint.Value, (int)YPoint.Value);
        window.Activate();
    }

    private void ShowSampleWindow2(object sender, RoutedEventArgs e)
    {
        SampleWindow2 window = new();
        window.Activate();
    }

    private void ShowSampleWindow3(object sender, RoutedEventArgs e)
    {
        SampleWindow3 window = new(IsAlwaysOnTop.IsOn, IsMaximizable.IsOn, IsMinimizable.IsOn, IsResizable.IsOn, HasBorder.IsOn, HasTitleBar.IsOn);
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
        ModalWindow window = new();
        window.Activate();
    }

    private void ShowSampleWindow5(object sender, RoutedEventArgs e)
    {
        SampleWindow5 window = new();
        window.Activate();
    }

    private void ShowSampleWindow6(object sender, RoutedEventArgs e)
    {
        SampleWindow6 window = new((string)InitialSize.SelectedItem);
        window.Activate();
    }

    private void InitialSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (InitialSizeDescription == null)
        {
            return;
        }

        string size = InitialSize.SelectedItem.ToString();
        string percentage = size switch
        {
            "Small" => "5%",
            "Medium" => "15%",
            "Large" => "25%",
            _ => "Unknown"
        };

        InitialSizeDescription.Text = $"{size}: Window size is approximately {percentage} of the display's work area.";
    }
}
