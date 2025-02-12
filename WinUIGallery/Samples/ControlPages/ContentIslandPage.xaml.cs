using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Shapes;

namespace WinUIGallery.ControlPages;

public sealed partial class ContentIslandPage : Page
{
    public ContentIslandPage()
    {
        this.InitializeComponent();
    }

    int idx = 0;

    Rectangle GetNextHostElement()
    {
        if (idx < _rectanglePanel.Children.Count)
        {
            return ((Rectangle)_rectanglePanel.Children[idx++]);
        }

        return null;
    }

    public async void LoadModel()
    {
        ContentIsland parentIsland = this.XamlRoot.TryGetContentIsland();

        Rectangle rect = GetNextHostElement();
        if (rect == null)
        {
            return;
        }

        ContainerVisual placementVisual = (ContainerVisual)ElementCompositionPreview.GetElementVisual(rect);
        Vector2 size = rect.ActualSize;

        ChildSiteLink childSiteLink = ChildSiteLink.Create(parentIsland, placementVisual);

        placementVisual.Size = size;
        childSiteLink.ActualSize = size;

        ContentIsland helmetIsland = await HelmetScenario.CreateIsland(placementVisual.Compositor);

        childSiteLink.Connect(helmetIsland);
    }

    private void LoadModel_Click(object sender, RoutedEventArgs e)
    {
        LoadModel();
    }
}
