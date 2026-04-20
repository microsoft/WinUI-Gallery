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
