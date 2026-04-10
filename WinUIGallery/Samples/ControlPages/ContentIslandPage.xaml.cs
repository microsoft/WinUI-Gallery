// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Composition;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace WinUIGallery.ControlPages;

public sealed partial class ContentIslandPage : Page
{
    private sealed class IslandEntry(ChildSiteLink childSiteLink, Rectangle rect, EventHandler<object?> layoutUpdatedHandler)
    {
        public ChildSiteLink ChildSiteLink { get; } = childSiteLink;

        public ContentIsland? HelmetIsland { get; set; }

        public Rectangle Rect { get; } = rect;

        public EventHandler<object?> LayoutUpdatedHandler { get; } = layoutUpdatedHandler;
    }

    private readonly List<IslandEntry> _islandEntries = [];

    public ContentIslandPage()
    {
        this.InitializeComponent();
        Unloaded += OnUnloaded;
    }

    private int NextHostElementIndex { get; set; } = 0;

    private async void LoadModel_Click(object sender, RoutedEventArgs e)
    {
        _rectanglePanel.Visibility = Visibility.Visible;
        await LoadModel();
    }

    private Rectangle? GetNextHostElement()
    {
        if (NextHostElementIndex < _rectanglePanel.Children.Count)
        {
            return ((Rectangle)_rectanglePanel.Children[NextHostElementIndex++]);
        }

        return null;
    }

    public async Task LoadModel()
    {
        ContentIsland parentIsland = this.XamlRoot.ContentIsland;

        Rectangle? rect = GetNextHostElement();
        if (rect == null)
        {
            return;
        }

        ContainerVisual placementVisual = (ContainerVisual)ElementCompositionPreview.GetElementVisual(rect);
        Vector2 size = rect.ActualSize;

        // The ChildSiteLink must live as long as the visual connection between parent and child islands.
#pragma warning disable CA2000 // Dispose objects before losing scope
        ChildSiteLink childSiteLink = ChildSiteLink.Create(parentIsland, placementVisual);
#pragma warning restore CA2000

        // We also need to keep the offset of the ChildContentLink within the parent ContentIsland in sync
        // with that of the placementElement for UIA to work correctly.
        var layoutUpdatedEventHandler = new EventHandler<object?>((s, e) =>
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

        // Track the entry before the await so OnUnloaded can dispose it even if the page
        // is navigated away while the island is still loading.
        var entry = new IslandEntry(childSiteLink, rect, layoutUpdatedEventHandler);
        _islandEntries.Add(entry);

        // The ContentIsland is connected to the ChildSiteLink and must outlive this method.
#pragma warning disable CA2000 // Dispose objects before losing scope
        ContentIsland helmetIsland = await HelmetScenario.CreateIsland(placementVisual.Compositor);
#pragma warning restore CA2000

        if (!childSiteLink.IsClosed)
        {
            entry.HelmetIsland = helmetIsland;
            childSiteLink.Connect(helmetIsland);
        }
        else
        {
            // The page was unloaded while the island was loading; release it immediately.
            helmetIsland.Dispose();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        foreach (IslandEntry entry in _islandEntries)
        {
            entry.Rect.LayoutUpdated -= entry.LayoutUpdatedHandler;
            entry.ChildSiteLink.Dispose();
            entry.HelmetIsland?.Dispose();
        }

        _islandEntries.Clear();
        NextHostElementIndex = 0;
    }
}