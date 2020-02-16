using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Documents;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ImagePage : Page
    {
        public ImagePage()
        {
            this.InitializeComponent();
        }

        private void ImageStretch_Checked(object sender, RoutedEventArgs e)
        {
            if (StretchImage != null)
            {
                var strStretch = (sender as RadioButton).Content.ToString();
                var stretch = (Stretch)Enum.Parse(typeof(Stretch), strStretch);
                StretchImage.Stretch = stretch;
            }
        }
    }
}
