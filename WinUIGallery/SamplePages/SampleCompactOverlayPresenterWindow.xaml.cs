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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.SamplePages
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SampleCompactOverlayPresenterWindow : Window
    {
        public SampleCompactOverlayPresenterWindow(CompactOverlaySize size)
        {
            this.InitializeComponent();
            var presenter = CompactOverlayPresenter.Create();
            presenter.InitialSize = size;
            AppWindow.SetPresenter(presenter);
            SizeText.Text = sizeText(size);
        }

        private static string sizeText(CompactOverlaySize size)
        {
            switch (size)
            {
                case CompactOverlaySize.Large: return "Large";
                case CompactOverlaySize.Medium: return "Medium";
                case CompactOverlaySize.Small: return "Small";
            }
            return "";
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ExtendsContentIntoTitleBar = !ExtendsContentIntoTitleBar;
        }
    }
}
