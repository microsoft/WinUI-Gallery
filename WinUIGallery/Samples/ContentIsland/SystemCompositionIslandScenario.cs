// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;

namespace WinUIGallery.ControlPages;

internal static class SystemCompositionIslandScenario
{
    public static Task<ContentIsland> CreateIslandAsync(Compositor compositor, Vector2 size)
    {
        ContainerVisual root = compositor.CreateContainerVisual();
        root.Size = size;

        SpriteVisual background = compositor.CreateSpriteVisual();
        background.Size = size;

        CompositionLinearGradientBrush backgroundBrush = compositor.CreateLinearGradientBrush();
        backgroundBrush.StartPoint = Vector2.Zero;
        backgroundBrush.EndPoint = Vector2.One;
        backgroundBrush.ColorStops.Add(
            compositor.CreateColorGradientStop(0, Color.FromArgb(255, 0, 99, 177)));
        backgroundBrush.ColorStops.Add(
            compositor.CreateColorGradientStop(1, Color.FromArgb(255, 80, 230, 210)));
        background.Brush = backgroundBrush;
        root.Children.InsertAtBottom(background);

        float diameter = MathF.Max(48, MathF.Min(size.X, size.Y) * 0.42f);
        SpriteVisual accent = compositor.CreateSpriteVisual();
        accent.Size = new Vector2(diameter);
        accent.Offset = new Vector3((size.X - diameter) / 2, (size.Y - diameter) / 2, 0);
        accent.CenterPoint = new Vector3(diameter / 2, diameter / 2, 0);
        accent.Brush = compositor.CreateColorBrush(Color.FromArgb(220, 255, 255, 255));
        root.Children.InsertAtTop(accent);

        ScalarKeyFrameAnimation rotation = compositor.CreateScalarKeyFrameAnimation();
        rotation.InsertKeyFrame(0, 0);
        rotation.InsertKeyFrame(1, 360);
        rotation.Duration = TimeSpan.FromSeconds(8);
        rotation.IterationBehavior = AnimationIterationBehavior.Forever;
        accent.StartAnimation("RotationAngleInDegrees", rotation);

        ContentIsland island = ContentIsland.CreateForSystemVisual(
            DispatcherQueue.GetForCurrentThread(),
            root);
        return Task.FromResult(island);
    }
}
