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
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Documents;

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
