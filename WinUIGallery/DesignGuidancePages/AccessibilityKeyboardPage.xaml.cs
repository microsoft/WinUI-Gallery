using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using AppUIBasics;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIGallery.DesktopWap.DataModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.DesktopWap.DesignGuidancePages
{
    public sealed partial class AccessibilityKeyboardPage : ItemsPageBase
    {
        public AccessibilityKeyboardPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
            args.NavigationRootPage.NavigationView.Header = string.Empty;
        }

        private void MakeRedButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRectangle.Fill = new SolidColorBrush(Colors.Red);
        }
        private void MakeBlueButton_Click(object sender, RoutedEventArgs e)
        {
            ColorRectangle.Fill = new SolidColorBrush(Colors.Blue);
        }
    }
}
