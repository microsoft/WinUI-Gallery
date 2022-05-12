using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class PipsPagerPage : Page
    {
        public List<string> Pictures = new List<string>()
        {
            "ms-appx:///Assets/SampleMedia/cliff.jpg",
            "ms-appx:///Assets/SampleMedia/grapes.jpg",
            "ms-appx:///Assets/SampleMedia/rainier.jpg",
            "ms-appx:///Assets/SampleMedia/sunset.jpg",
            "ms-appx:///Assets/SampleMedia/valley.jpg",
        };
        public PipsPagerPage()
        {
            this.InitializeComponent();
        }

        private void OrientationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string orientation = e.AddedItems[0].ToString();

            switch (orientation)
            {
                case "Vertical":
                    TestPipsPager2.Orientation = Orientation.Vertical;
                    break;

                case "Horizontal":
                default:
                    TestPipsPager2.Orientation = Orientation.Horizontal;
                    break;
            }
        }

        private void PrevButtonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            string prevButtonVisibility = e.AddedItems[0].ToString();

            switch (prevButtonVisibility)
            {
                case "Visible":
                    TestPipsPager2.PreviousButtonVisibility = PipsPagerButtonVisibility.Visible;
                    break;

                case "VisibleOnPointerOver":
                    TestPipsPager2.PreviousButtonVisibility = PipsPagerButtonVisibility.VisibleOnPointerOver;
                    break;

                case "Collapsed":
                default:
                    TestPipsPager2.PreviousButtonVisibility = PipsPagerButtonVisibility.Collapsed;
                    break;
            }
        }

        private void NextButtonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string nextButtonVisibility = e.AddedItems[0].ToString();

            switch (nextButtonVisibility)
            {
                case "Visible":
                    TestPipsPager2.NextButtonVisibility = PipsPagerButtonVisibility.Visible;
                    break;

                case "VisibleOnPointerOver":
                    TestPipsPager2.NextButtonVisibility = PipsPagerButtonVisibility.VisibleOnPointerOver;
                    break;

                case "Collapsed":
                default:
                    TestPipsPager2.NextButtonVisibility = PipsPagerButtonVisibility.Collapsed;
                    break;
            }
        }
    }
}
