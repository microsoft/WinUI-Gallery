using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Composition;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContentIslandPage : Page
    {
        public ContentIslandPage()
        {
            this.InitializeComponent();
        }

        List<object> _keepAlive = new List<object>();
        int idx = 0;

        Microsoft.UI.Xaml.Shapes.Rectangle GetNextHostElement()
        {
            if (idx < _rectanglePanel.Children.Count)
            {
                return ((Microsoft.UI.Xaml.Shapes.Rectangle)_rectanglePanel.Children[idx++]);
            }

            return null;
        }

        public async void SetupHelmet()
        {
            ContentIsland parentIsland = this.XamlRoot.TryGetContentIsland();

            Microsoft.UI.Xaml.Shapes.Rectangle rect = GetNextHostElement();
            if (rect == null)
            {
                return;
            }

            ContainerVisual placementVisual = (ContainerVisual)ElementCompositionPreview.GetElementVisual(rect);
            Vector2 size = rect.ActualSize;

            ChildContentLink childContentLink = ChildContentLink.Create(parentIsland, placementVisual);

            placementVisual.Size = size;
            childContentLink.ActualSize = size;

            ContentIsland helmetIsland = await HelmetScenario.CreateIsland(placementVisual.Compositor);

            childContentLink.Connect(helmetIsland);

            _keepAlive.Add(childContentLink);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetupHelmet();
        }
    }
}
