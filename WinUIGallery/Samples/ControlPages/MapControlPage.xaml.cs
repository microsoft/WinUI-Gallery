// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace WinUIGallery.ControlPages;

public sealed partial class MapControlPage : Page
{
    public MapControlPage()
    {
        this.InitializeComponent();

        this.Loaded += MapControlPage_Loaded;
        this.Unloaded += MapControlPage_Unloaded;
    }

    private void MapControlPage_Unloaded(object sender, RoutedEventArgs e)
    {
        // MapControl is internally backed by a WebView2 hosting Azure Maps.
        // When the page is navigated away from, the WebView2's UIA tree
        // (a Pane containing a Chromium RootWebArea with a long
        // data:text/html;base64 Name) can outlive the page and contaminate
        // subsequent Axe.Windows accessibility scans across the process.
        // Explicitly tearing down the MapControl on Unloaded forces the
        // framework to dispose the embedded WebView2 and remove its UIA
        // subtree from the process tree.
        this.Loaded -= MapControlPage_Loaded;
        this.Unloaded -= MapControlPage_Unloaded;

        if (map1 is not null)
        {
            map1.Layers.Clear();
            if (map1.Parent is Panel parent)
            {
                parent.Children.Remove(map1);
            }
        }
    }

    private void MapControlPage_Loaded(object sender, RoutedEventArgs e)
    {
        var myLandmarks = new List<MapElement>();

        BasicGeoposition centerPosition = new BasicGeoposition { Latitude = 0, Longitude = 0 };
        Geopoint centerPoint = new Geopoint(centerPosition);

        map1.Center = centerPoint;
        map1.ZoomLevel = 1;

        BasicGeoposition position = new BasicGeoposition { Latitude = -30.034647, Longitude = -51.217659 };
        Geopoint point = new Geopoint(position);

        var icon = new MapIcon
        {
            Location = point,
        };

        myLandmarks.Add(icon);

        var LandmarksLayer = new MapElementsLayer
        {
            MapElements = myLandmarks
        };

        map1.Layers.Add(LandmarksLayer);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        map1.MapServiceToken = MapToken.Password;
    }

    private void MapToken_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            map1.MapServiceToken = MapToken.Password;
        }
    }
}
