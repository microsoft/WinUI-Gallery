using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class PipsPagerPage : Page
{
    public List<string> Pictures =
    [
        "ms-appx:///Assets/SampleMedia/LandscapeImage1.jpg",
        "ms-appx:///Assets/SampleMedia/LandscapeImage2.jpg",
        "ms-appx:///Assets/SampleMedia/LandscapeImage3.jpg",
        "ms-appx:///Assets/SampleMedia/LandscapeImage4.jpg",
        "ms-appx:///Assets/SampleMedia/LandscapeImage5.jpg",
        "ms-appx:///Assets/SampleMedia/LandscapeImage6.jpg",
        "ms-appx:///Assets/SampleMedia/LandscapeImage7.jpg",
        "ms-appx:///Assets/SampleMedia/LandscapeImage8.jpg",
    ];
    public PipsPagerPage()
    {
        InitializeComponent();
    }

    private void OrientationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string orientation = e.AddedItems[0].ToString();

        TestPipsPager2.Orientation = orientation switch
        {
            "Vertical" => Orientation.Vertical,
            _ => Orientation.Horizontal,
        };
    }

    private void PrevButtonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    { 
        string prevButtonVisibility = e.AddedItems[0].ToString();

        TestPipsPager2.PreviousButtonVisibility = prevButtonVisibility switch
        {
            "Visible" => PipsPagerButtonVisibility.Visible,
            "VisibleOnPointerOver" => PipsPagerButtonVisibility.VisibleOnPointerOver,
            _ => PipsPagerButtonVisibility.Collapsed,
        };
    }

    private void NextButtonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string nextButtonVisibility = e.AddedItems[0].ToString();

        TestPipsPager2.NextButtonVisibility = nextButtonVisibility switch
        {
            "Visible" => PipsPagerButtonVisibility.Visible,
            "VisibleOnPointerOver" => PipsPagerButtonVisibility.VisibleOnPointerOver,
            _ => PipsPagerButtonVisibility.Collapsed,
        };
    }
}
