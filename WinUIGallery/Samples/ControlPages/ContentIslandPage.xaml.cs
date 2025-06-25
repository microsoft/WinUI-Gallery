// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Composition;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Numerics;

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
        ContentIsland parentIsland = this.XamlRoot.ContentIsland;

        Rectangle rect = GetNextHostElement();
        if (rect == null)
        {
            return;
        }

        ContainerVisual placementVisual = (ContainerVisual)ElementCompositionPreview.GetElementVisual(rect);
        Vector2 size = rect.ActualSize;

        ChildSiteLink childSiteLink = ChildSiteLink.Create(parentIsland, placementVisual);

        // We also need to keep the offset of the ChildContentLink within the parent ContentIsland in sync
        // with that of the placementElement for UIA to work correctly.
        var layoutUpdatedEventHandler = new EventHandler<object>((s, e) =>
        {
            // NOTE: Do as little work in here as possible because it gets called for every
            // xaml layout change on this thread!
            var transform = rect.TransformToVisual(null);
            var point = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
            childSiteLink.LocalToParentTransformMatrix = System.Numerics.Matrix4x4.CreateTranslation(
                (float)(point.X),
                (float)(point.Y),
                0);
        });
        rect.LayoutUpdated += layoutUpdatedEventHandler;
        layoutUpdatedEventHandler.Invoke(null, null);

        placementVisual.Size = size;
        childSiteLink.ActualSize = size;

        ContentIsland helmetIsland = await HelmetScenario.CreateIsland(placementVisual.Compositor);

        childSiteLink.Connect(helmetIsland);
    }

    private void LoadModel_Click(object sender, RoutedEventArgs e)
    {
        _rectanglePanel.Visibility = Visibility.Visible;
        LoadModel();
    }
}
