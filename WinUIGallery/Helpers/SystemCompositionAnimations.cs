// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Numerics;
using Windows.UI.Composition;

namespace WinUIGallery.Helpers;

internal static class SystemCompositionAnimations
{
    public static void SetImplicitShowHide(UIElement element)
    {
        Compositor compositor = ElementCompositionPreview.GetElementVisual(element).Compositor;

        ElementCompositionPreview.SetImplicitShowAnimation(
            element,
            CreateAnimationGroup(
                compositor,
                new Vector3(0, 24, 0),
                Vector3.Zero,
                0,
                1,
                TimeSpan.FromMilliseconds(400),
                TimeSpan.FromMilliseconds(200)));

        ElementCompositionPreview.SetImplicitHideAnimation(
            element,
            CreateAnimationGroup(
                compositor,
                Vector3.Zero,
                new Vector3(0, 24, 0),
                1,
                0,
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(100)));
    }

    private static CompositionAnimationGroup CreateAnimationGroup(
        Compositor compositor,
        Vector3 offsetFrom,
        Vector3 offsetTo,
        float opacityFrom,
        float opacityTo,
        TimeSpan offsetDuration,
        TimeSpan opacityDuration)
    {
        CubicBezierEasingFunction easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0, 0),
            new Vector2(0.58f, 1));

        Vector3KeyFrameAnimation offset = compositor.CreateVector3KeyFrameAnimation();
        offset.Target = "Offset";
        offset.InsertKeyFrame(0, offsetFrom);
        offset.InsertKeyFrame(1, offsetTo, easing);
        offset.Duration = offsetDuration;

        ScalarKeyFrameAnimation opacity = compositor.CreateScalarKeyFrameAnimation();
        opacity.Target = "Opacity";
        opacity.InsertKeyFrame(0, opacityFrom);
        opacity.InsertKeyFrame(1, opacityTo, easing);
        opacity.Duration = opacityDuration;

        CompositionAnimationGroup group = compositor.CreateAnimationGroup();
        group.Add(offset);
        group.Add(opacity);
        return group;
    }
}
