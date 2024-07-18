using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIGallery.SamplePages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppWindowPage : Page
    {
        public AppWindowPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new SampleOverlappedPresenterWindow().Activate();
        }

        private void CreateWithFullScreenPresenterButton_Click(object sender, RoutedEventArgs e)
        {
            new SampleFullScreenPresenterWindow().Activate();
        }

        private void CreateWithCompactOverlayPresenterMenu_Click(object sender, RoutedEventArgs e)
        {
            switch((sender as MenuFlyoutItem).Tag)
            {
                case "Large":
                    new SampleCompactOverlayPresenterWindow(Microsoft.UI.Windowing.CompactOverlaySize.Large).Activate();
                    break;
                case "Medium":
                    new SampleCompactOverlayPresenterWindow(Microsoft.UI.Windowing.CompactOverlaySize.Medium).Activate();
                    break;
                case "Small":
                    new SampleCompactOverlayPresenterWindow(Microsoft.UI.Windowing.CompactOverlaySize.Small).Activate();
                    break;
            }
        }
    }
}
