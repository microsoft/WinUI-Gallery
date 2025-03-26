using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Composition;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.Shaders
{
    public sealed partial class OverlayPanel : UserControl
    {
        private Compositor compositor;

        public OverlayPanel()
        {
            this.InitializeComponent();
            compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        public int ChildCount => overlayRootCanvas.Children.Count;

        public void AddOverlay(UIElement uiElement, Point offset)
        {
            var overlayVisual = ElementCompositionPreview.GetElementVisual(overlayRootCanvas);
            overlayVisual.Opacity = 1.0f;
            overlayRootCanvas.Children.Add(uiElement);
            uiElement.SetValue(Canvas.LeftProperty, offset.X);
            uiElement.SetValue(Canvas.TopProperty, offset.Y);
        }

        public void ClearOverlays()
        {
            var overlayRoot = ElementCompositionPreview.GetElementChildVisual(overlayRootCanvas) as ContainerVisual;
            if (overlayRoot != null)
            {
                overlayRoot.Children.RemoveAll();
            }

            overlayRootCanvas.Children.Clear();
        }

        public void ClearOverlay(UIElement uiElement)
        {
            if (overlayRootCanvas.Children.Contains(uiElement))
            {
                overlayRootCanvas.Children.Remove(uiElement);
            }
        }
    }
}
