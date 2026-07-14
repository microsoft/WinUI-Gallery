// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Numerics;

namespace WinUIGallery.ControlPages;

public sealed partial class CompositionBasicsPage : Page
{
    private Visual? _elementVisual;
    private SpriteVisual? _childVisual;
    private Visual? _transformMatrixVisual;
    private InsetClip? _insetClip;

    public CompositionBasicsPage()
    {
        InitializeComponent();
        Unloaded += CompositionBasicsPage_Unloaded;
    }

    private void ElementVisualExample_Loaded(object sender, RoutedEventArgs e)
    {
        _elementVisual = ElementCompositionPreview.GetElementVisual(ElementVisualTarget);
        UpdateElementVisualCenterPoint();
        UpdateElementVisualTransform();
    }

    private void ElementVisualTarget_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateElementVisualCenterPoint();
    }

    private void ElementVisualTransform_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        UpdateElementVisualTransform();
    }

    private void UpdateElementVisualCenterPoint()
    {
        if (_elementVisual is null)
        {
            return;
        }

        _elementVisual.CenterPoint = new Vector3(ElementVisualTarget.ActualSize / 2, 0);
    }

    private void UpdateElementVisualTransform()
    {
        if (_elementVisual is null)
        {
            return;
        }

        ElementVisualTarget.Translation = new Vector3(
            (float)TranslationXSlider.Value,
            (float)TranslationYSlider.Value,
            0);
        _elementVisual.RotationAngleInDegrees = (float)RotationSlider.Value;
    }

    private void ChildVisualExample_Loaded(object sender, RoutedEventArgs e)
    {
        if (_childVisual is null)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(ChildVisualHost).Compositor;
            _childVisual = compositor.CreateSpriteVisual();
            _childVisual.Size = new Vector2(80);
            _childVisual.Brush = compositor.CreateColorBrush(Windows.UI.Color.FromArgb(255, 0, 120, 212));
            ElementCompositionPreview.SetElementChildVisual(ChildVisualHost, _childVisual);
        }

        UpdateChildVisual();
    }

    private void ChildVisualHost_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateChildVisual();
    }

    private void AnchorPointSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        UpdateChildVisual();
    }

    private void UpdateChildVisual()
    {
        if (_childVisual is null)
        {
            return;
        }

        _childVisual.AnchorPoint = new Vector2(
            (float)AnchorXSlider.Value,
            (float)AnchorYSlider.Value);
        _childVisual.Offset = new Vector3(ChildVisualHost.ActualSize / 2, 0);
    }

    private void TransformMatrixExample_Loaded(object sender, RoutedEventArgs e)
    {
        _transformMatrixVisual = ElementCompositionPreview.GetElementVisual(TransformMatrixTarget);
        UpdateTransformMatrixCenterPoint();
        UpdateTransformMatrix();
    }

    private void TransformMatrixTarget_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateTransformMatrixCenterPoint();
    }

    private void TransformMatrixSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        UpdateTransformMatrix();
    }

    private void UpdateTransformMatrixCenterPoint()
    {
        if (_transformMatrixVisual is null)
        {
            return;
        }

        _transformMatrixVisual.CenterPoint = new Vector3(TransformMatrixTarget.ActualSize / 2, 0);
    }

    private void UpdateTransformMatrix()
    {
        if (_transformMatrixVisual is null)
        {
            return;
        }

        float skewRadians = (float)(SkewXSlider.Value * Math.PI / 180);
        Matrix4x4 skewMatrix = Matrix4x4.Identity;
        skewMatrix.M21 = MathF.Tan(skewRadians);
        _transformMatrixVisual.TransformMatrix =
            Matrix4x4.CreateScale((float)ScaleXSlider.Value, 1, 1) *
            skewMatrix;
    }

    private void ClipExample_Loaded(object sender, RoutedEventArgs e)
    {
        Visual visual = ElementCompositionPreview.GetElementVisual(ClipTarget);
        _insetClip ??= visual.Compositor.CreateInsetClip();
        visual.Clip = _insetClip;
        UpdateClip();
    }

    private void ClipSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        UpdateClip();
    }

    private void UpdateClip()
    {
        if (_insetClip is null)
        {
            return;
        }

        float horizontalInset = (float)HorizontalClipSlider.Value;
        float verticalInset = (float)VerticalClipSlider.Value;
        _insetClip.LeftInset = horizontalInset;
        _insetClip.RightInset = horizontalInset;
        _insetClip.TopInset = verticalInset;
        _insetClip.BottomInset = verticalInset;
    }

    private void CompositionBasicsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        ElementCompositionPreview.SetElementChildVisual(ChildVisualHost, null);
        _childVisual?.Dispose();
        _childVisual = null;
    }
}
