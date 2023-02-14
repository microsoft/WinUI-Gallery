// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Numerics;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Composition;
using Microsoft.UI;
using CommunityToolkit.WinUI.UI.Animations;
using Windows.UI;
using System;
using CommunityToolkit.WinUI.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppUIBasics.Controls
{
    // ATTRIBUTION: @RykenApps
    public sealed partial class HomePageHeaderImage : UserControl
    {
        private Compositor _compositor;
        private CompositionLinearGradientBrush _imageGridBottomGradientBrush;
        private CompositionEffectBrush _imageGridEffectBrush;
        private ExpressionAnimation _imageGridSizeAnimation;
        private ExpressionAnimation _bottomGradientStartPointAnimation;
        private SpriteVisual _imageGridSpriteVisual;
        private CompositionSurfaceBrush _imageGridSurfaceBrush;
        private Visual _imageGridVisual;
        private CompositionVisualSurface _imageGridVisualSurface;
        private const string GradientSizeKey = "GradientSize";
        public HomePageHeaderImage()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _imageGridVisual = ElementCompositionPreview.GetElementVisual(ImageGrid);
            _compositor = _imageGridVisual.Compositor;

            _imageGridSizeAnimation = _compositor.CreateExpressionAnimation("Visual.Size");
            _imageGridSizeAnimation.SetReferenceParameter("Visual", _imageGridVisual);

            _imageGridVisualSurface = _compositor.CreateVisualSurface();
            _imageGridVisualSurface.SourceVisual = _imageGridVisual;
            _imageGridVisualSurface.StartAnimation(nameof(CompositionVisualSurface.SourceSize), _imageGridSizeAnimation);

            _imageGridSurfaceBrush = _compositor.CreateSurfaceBrush(_imageGridVisualSurface);
            _imageGridSurfaceBrush.Stretch = CompositionStretch.UniformToFill;

            _bottomGradientStartPointAnimation = CreateExpressionAnimation(nameof(CompositionLinearGradientBrush.StartPoint), $"Vector2(0.5, Visual.Size.Y - this.{GradientSizeKey})");
            SetBottomGradientStartPoint();

            _imageGridBottomGradientBrush = _compositor.CreateLinearGradientBrush();
            _imageGridBottomGradientBrush.MappingMode = CompositionMappingMode.Absolute;
            _imageGridBottomGradientBrush.StartAnimation(_bottomGradientStartPointAnimation);
            _imageGridBottomGradientBrush.StartAnimation(CreateExpressionAnimation(nameof(CompositionLinearGradientBrush.EndPoint), "Vector2(0.5, Visual.Size.Y)"));
            _imageGridBottomGradientBrush.CreateColorStopsWithEasingFunction(EasingType.Sine, EasingMode.EaseInOut, 0f, 1f);

            var alphaMask = new AlphaMaskEffect
            {
                Source = new CompositionEffectSourceParameter("ImageGrid"),
                AlphaMask = new CompositionEffectSourceParameter("Gradient")
            };

            var effectFactory = _compositor.CreateEffectFactory(alphaMask);
            _imageGridEffectBrush = effectFactory.CreateBrush();
            _imageGridEffectBrush.SetSourceParameter("ImageGrid", _imageGridSurfaceBrush);
            _imageGridEffectBrush.SetSourceParameter("Gradient", _imageGridBottomGradientBrush);

            _imageGridSpriteVisual = _compositor.CreateSpriteVisual();
            _imageGridSpriteVisual.RelativeSizeAdjustment = Vector2.One;
            _imageGridSpriteVisual.Brush = _imageGridEffectBrush;

            ElementCompositionPreview.GetElementVisual(ImageGridSurfaceRec).ParentForTransform = _imageGridVisual;

            ElementCompositionPreview.SetElementChildVisual(ImageGridSurfaceRec, _imageGridSpriteVisual);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ElementCompositionPreview.SetElementChildVisual(ImageGridSurfaceRec, null);
            _imageGridSpriteVisual?.Dispose();
            _imageGridEffectBrush?.Dispose();
            _imageGridSurfaceBrush?.Dispose();
            _imageGridVisualSurface?.Dispose();
            _imageGridSizeAnimation?.Dispose();
            _bottomGradientStartPointAnimation?.Dispose();
            _bottomGradientStartPointAnimation = null;
        }
        private void OnLoading(FrameworkElement sender, object args)
        {
            if (HeroImage.Source == null)
            {
                HeroImage.GetVisual().Opacity = 0;
            }
            else
            {
                AnimateImage();
            }
        }
        private void SetBottomGradientStartPoint()
        {
            _bottomGradientStartPointAnimation?.Properties.InsertScalar(GradientSizeKey, 180);
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            AnimateImage();
        }

        private void AnimateImage()
        {
            AnimationBuilder.Create()
                .Opacity(1, 0, duration: TimeSpan.FromMilliseconds(300), easingMode: EasingMode.EaseOut)
                .Scale(1, 1.1f, duration: TimeSpan.FromMilliseconds(400), easingMode: EasingMode.EaseOut)
                .Start(HeroImage);

            AnimationBuilder.Create()
                .Opacity(0.5, 0, duration: TimeSpan.FromMilliseconds(300), easingMode: EasingMode.EaseOut)
                .Scale(1, 1.1f, duration: TimeSpan.FromMilliseconds(400), easingMode: EasingMode.EaseOut)
                .Start(HeroOverlayImage);
        }


        private ExpressionAnimation CreateExpressionAnimation(string target, string expression)
        {
            var ani = _compositor.CreateExpressionAnimation(expression);
            ani.SetReferenceParameter("Visual", _imageGridVisual);
            ani.Target = target;
            return ani;
        }
    }
}

public static class CompositionGradientBrushExtensions
{
    /// <summary>
    /// Create color stops by using easing function
    /// </summary>
    public static void CreateColorStopsWithEasingFunction(this CompositionGradientBrush compositionGradientBrush, EasingType easingType, EasingMode easingMode, float colorStopBegin, float colorStopEnd, float gap = 0.05f)
    {
        var compositor = compositionGradientBrush.Compositor;
        var easingFunc = easingType.ToEasingFunction(easingMode);
        if (easingFunc != null)
        {
            for (float i = colorStopBegin; i < colorStopEnd; i += gap)
            {
                var progress = (i - colorStopBegin) / (colorStopEnd - colorStopBegin);

                var colorStop = compositor.CreateColorGradientStop(i, Color.FromArgb((byte)(0xff * easingFunc.Ease(1 - progress)), 0, 0, 0));
                compositionGradientBrush.ColorStops.Add(colorStop);
            }
        }
        else
        {
            compositionGradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(colorStopBegin, Colors.Black));
        }

        compositionGradientBrush.ColorStops.Add(compositor.CreateColorGradientStop(colorStopEnd, Colors.Transparent));
    }
}
