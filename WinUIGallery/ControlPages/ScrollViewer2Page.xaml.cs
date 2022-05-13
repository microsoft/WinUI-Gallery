using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScrollViewer2Page : ItemsPageBase
    {
        private bool scroller2MouseIsOver = false;
        public ScrollViewer2Page()
        {
            this.InitializeComponent();

            this.scroller2.StateChanged += Scroller_StateChanged;
            this.scroller2.PointerEntered += this.Scroller2_PointerEntered;
            this.scroller2.PointerExited += this.Scroller2_PointerExited;
        }

        private void Scroller2_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            scroller2MouseIsOver = false;
        }

        private void Scroller2_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            scroller2MouseIsOver = true;
        }


        #region Zooming

        private void ZoomModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (scroller2 != null && ZoomSlider != null)
            {
                ComboBox cb = sender as ComboBox;
                if (cb != null)
                {
                    switch (cb.SelectedIndex)
                    {
                        case 0: // Enabled
                            scroller2.ZoomMode = muxc.ZoomMode.Enabled;
                            ZoomSlider.IsEnabled = true;
                            break;
                        default: // Disabled
                            scroller2.ZoomMode = muxc.ZoomMode.Disabled;
                            scroller2.ZoomTo(1.0f, new Vector2());
                            ZoomSlider.Value = 1;
                            ZoomSlider.IsEnabled = false;
                            break;
                    }
                }
            }
        }

        private void Scroller_StateChanged(muxc.ScrollViewer sender, object args)
        {
            
            // Checking if sender is idle and the scrollviewer has the mouse over itsself 
            // since when using slider, the sender may sometimes still be idle while
            // user is still changing the slider
            if (sender.State == muxc.InteractionState.Idle && scroller2MouseIsOver)
            {
                ZoomSlider.Value = Math.Round(sender.ZoomFactor, (int)(10 * ZoomSlider.StepFrequency));
            }
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (scroller2 != null)
            {
                // Zoom based on the center point of the current viewport
                scroller2.ZoomTo((float)e.NewValue, null);
            }
        }

        #endregion

        #region Custom Animations

        public double Sample4MaximumXViewportPosition
        {
            get { return (double)GetValue(Sample4MaximumXViewportPositionProperty); }
            set { SetValue(Sample4MaximumXViewportPositionProperty, value); }
        }

        public static readonly DependencyProperty Sample4MaximumXViewportPositionProperty =
            DependencyProperty.Register(
                nameof(Sample4MaximumXViewportPosition),
                typeof(double),
                typeof(ScrollViewer2Page),
                new PropertyMetadata(0.0));


        public double Sample4MaximumYViewportPosition
        {
            get { return (double)GetValue(Sample4MaximumYViewportPositionProperty); }
            set { SetValue(Sample4MaximumYViewportPositionProperty, value); }
        }

        public static readonly DependencyProperty Sample4MaximumYViewportPositionProperty =
            DependencyProperty.Register(
                nameof(Sample4MaximumYViewportPosition),
                typeof(double),
                typeof(ScrollViewer2Page),
                new PropertyMetadata(0.0));

        private void Sample4_ImageLoaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            Sample4MaximumXViewportPosition = image.ActualWidth - ((FrameworkElement)image.Parent).ActualWidth;
            Sample4MaximumYViewportPosition = image.ActualHeight - ((FrameworkElement)image.Parent).ActualHeight;
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            var newZoomFactor = zoomFactorSlider.Value;

            var newX = (xposSlider.Value / xposSlider.Maximum) * (image.ActualWidth * newZoomFactor - scroller4.ActualWidth);
            var newY = (yposSlider.Value / yposSlider.Maximum) * (image.ActualHeight * newZoomFactor - scroller4.ActualHeight);

            // Scroll
            scroller4.ScrollTo(newX, newY);

            // Zoom
            scroller4.ZoomTo((float)newZoomFactor, null);

        }

        #region Adjust scrolling animation

        private void Scroller4_ScrollAnimationStarting(muxc.ScrollViewer sender, muxc.ScrollAnimationStartingEventArgs args)
        {
            try
            {
                Vector3KeyFrameAnimation stockKeyFrameAnimation = args.Animation as Vector3KeyFrameAnimation;

                if (stockKeyFrameAnimation != null)
                {
                    Vector3KeyFrameAnimation customKeyFrameAnimation = stockKeyFrameAnimation;

                    if (nameof(ScrollAnimationOptions.Default) != (string)cbAnimation.SelectedItem)
                    {
                        double targetHorizontalOffset = args.EndPosition.X;
                        float targetHorizontalPosition = ComputeHorizontalPositionFromOffset(sender.Content, targetHorizontalOffset, sender.ZoomFactor);

                        double targetVerticalOffset = args.EndPosition.Y;
                        float targetVerticalPosition = ComputeVerticalPositionFromOffset(sender.Content, targetVerticalOffset, sender.ZoomFactor);

                        customKeyFrameAnimation = stockKeyFrameAnimation.Compositor.CreateVector3KeyFrameAnimation();

                        float deltaHorizontalPosition = (float)(targetHorizontalOffset - sender.HorizontalOffset);
                        float deltaVerticalPosition = (float)(targetVerticalOffset - sender.VerticalOffset);

                        switch ((string)cbAnimation.SelectedItem)
                        {
                            case nameof(ScrollAnimationOptions.Custom1):
                                // "Accordion" case
                                for (int keyframe = 0; keyframe < 3; keyframe++)
                                {
                                    customKeyFrameAnimation.InsertKeyFrame(
                                        1.0f - (0.4f / (float)Math.Pow(2, keyframe)),
                                        new Vector3(targetHorizontalPosition + 0.1f * deltaHorizontalPosition, targetVerticalPosition + 0.1f * deltaVerticalPosition, 0.0f));

                                    deltaHorizontalPosition /= -2.0f;
                                    deltaVerticalPosition /= -2.0f;
                                }

                                customKeyFrameAnimation.InsertKeyFrame(1.0f, new Vector3(targetHorizontalPosition, targetVerticalPosition, 0.0f));
                                break;
                            case nameof(ScrollAnimationOptions.Custom2):
                                // "Teleportation" case
                                CubicBezierEasingFunction cubicBezierStart = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
                                    new Vector2(1.0f, 0.0f),
                                    new Vector2(1.0f, 0.0f));

                                StepEasingFunction step = stockKeyFrameAnimation.Compositor.CreateStepEasingFunction(1);

                                CubicBezierEasingFunction cubicBezierEnd = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
                                    new Vector2(0.0f, 1.0f),
                                    new Vector2(0.0f, 1.0f));

                                customKeyFrameAnimation.InsertKeyFrame(
                                    0.499999f,
                                    new Vector3(targetHorizontalPosition - 0.75f * deltaHorizontalPosition, targetVerticalPosition - 0.75f * deltaVerticalPosition, 0.0f),
                                    cubicBezierStart);
                                customKeyFrameAnimation.InsertKeyFrame(
                                    0.5f,
                                    new Vector3(targetHorizontalPosition - 0.25f * deltaHorizontalPosition, targetVerticalPosition - 0.25f * deltaVerticalPosition, 0.0f),
                                    step);
                                customKeyFrameAnimation.InsertKeyFrame(
                                    1.0f,
                                    new Vector3(targetHorizontalPosition, targetVerticalPosition, 0.0f),
                                    cubicBezierEnd);
                                break;
                            default:
                                break;
                        }

                        customKeyFrameAnimation.Duration = stockKeyFrameAnimation.Duration;
                    }

                    if (!string.IsNullOrWhiteSpace(tbAnimDuration.Text))
                    {
                        // Override animation duration
                        double durationOverride = Convert.ToDouble(tbAnimDuration.Text);
                        customKeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(durationOverride);
                    }

                    args.Animation = customKeyFrameAnimation;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Adjust zooming animation

        private void Scroller4_ZoomAnimationStarting(muxc.ScrollViewer sender, muxc.ZoomAnimationStartingEventArgs args)
        {
            try
            {
                ScalarKeyFrameAnimation stockKeyFrameAnimation = args.Animation as ScalarKeyFrameAnimation;

                if (stockKeyFrameAnimation != null)
                {
                    ScalarKeyFrameAnimation customKeyFrameAnimation = stockKeyFrameAnimation;

                    if (nameof(ZoomAnimationOptions.Default) != (string)cbZoomAnimation.SelectedItem)
                    {
                        float targetZoomFactor = (float)zoomFactorSlider.Value;

                        customKeyFrameAnimation = stockKeyFrameAnimation.Compositor.CreateScalarKeyFrameAnimation();
                        float deltaZoomFactor = (float)(targetZoomFactor - sender.ZoomFactor);

                        switch ((string)cbZoomAnimation.SelectedItem)
                        {

                            case nameof(ZoomAnimationOptions.Custom1):
                                // "Accordion" case
                                for (int step = 0; step < 3; step++)
                                {
                                    customKeyFrameAnimation.InsertKeyFrame(
                                        1.0f - (0.4f / (float)Math.Pow(2, step)),
                                        targetZoomFactor + 0.1f * deltaZoomFactor);
                                    deltaZoomFactor /= -2.0f;
                                }

                                customKeyFrameAnimation.InsertKeyFrame(1.0f, targetZoomFactor);
                                break;
                            case nameof(ZoomAnimationOptions.Custom2):
                                // "Teleportation" case

                                CubicBezierEasingFunction cubicBezierStart = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
                                    new Vector2(1.0f, 0.0f),
                                    new Vector2(1.0f, 0.0f));

                                StepEasingFunction stepEasingFunc = stockKeyFrameAnimation.Compositor.CreateStepEasingFunction(1);

                                CubicBezierEasingFunction cubicBezierEnd = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
                                    new Vector2(0.0f, 1.0f),
                                    new Vector2(0.0f, 1.0f));

                                customKeyFrameAnimation.InsertKeyFrame(
                                    0.499999f,
                                    targetZoomFactor - 0.75f * deltaZoomFactor,
                                    cubicBezierStart);
                                customKeyFrameAnimation.InsertKeyFrame(
                                    0.5f,
                                    targetZoomFactor - 0.25f * deltaZoomFactor,
                                    stepEasingFunc);
                                customKeyFrameAnimation.InsertKeyFrame(
                                    1.0f,
                                    targetZoomFactor,
                                    cubicBezierEnd);
                                break;
                            default:
                                break;
                        }

                        customKeyFrameAnimation.Duration = stockKeyFrameAnimation.Duration;
                        args.Animation = customKeyFrameAnimation;
                    }

                    if (!string.IsNullOrWhiteSpace(tbZoomDuration.Text))
                    {
                        double durationOverride = Convert.ToDouble(tbZoomDuration.Text);
                        customKeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(durationOverride);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Helper methods

        private float ComputeHorizontalPositionFromOffset(UIElement content, double offset, float zoomFactor)
        {
            return (float)(offset + ComputeMinHorizontalPosition(content, zoomFactor));
        }

        private float ComputeVerticalPositionFromOffset(UIElement content, double offset, float zoomFactor)
        {
            return (float)(offset + ComputeMinVerticalPosition(content, zoomFactor));
        }

        private float ComputeMinHorizontalPosition(UIElement content, float zoomFactor)
        {
            if (content == null)
            {
                return 0;
            }

            FrameworkElement contentAsFE = content as FrameworkElement;

            if (contentAsFE == null)
            {
                return 0;
            }

            Thickness childMargin = contentAsFE.Margin;
            Visual scrollerVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(scroller4);
            double childWidth = double.IsNaN(contentAsFE.Width) ? contentAsFE.ActualWidth : contentAsFE.Width;
            float minPosX = 0.0f;
            float extentWidth = Math.Max(0.0f, (float)(childWidth + childMargin.Left + childMargin.Right));

            if (contentAsFE.HorizontalAlignment == HorizontalAlignment.Center ||
                contentAsFE.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                minPosX = Math.Min(0.0f, (extentWidth * zoomFactor - scrollerVisual.Size.X) / 2.0f);
            }
            else if (contentAsFE.HorizontalAlignment == HorizontalAlignment.Right)
            {
                minPosX = Math.Min(0.0f, extentWidth * zoomFactor - scrollerVisual.Size.X);
            }

            return minPosX;
        }

        private float ComputeMinVerticalPosition(UIElement content, float zoomFactor)
        {
            if (content == null)
            {
                return 0;
            }

            FrameworkElement contentAsFE = content as FrameworkElement;

            if (contentAsFE == null)
            {
                return 0;
            }

            Thickness childMargin = contentAsFE.Margin;
            Visual scrollerVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(scroller4);
            double childHeight = double.IsNaN(contentAsFE.Height) ? contentAsFE.ActualHeight : contentAsFE.Height;
            float minPosY = 0.0f;
            float extentHeight = Math.Max(0.0f, (float)(childHeight + childMargin.Top + childMargin.Bottom));

            if (contentAsFE.VerticalAlignment == VerticalAlignment.Center ||
                contentAsFE.VerticalAlignment == VerticalAlignment.Stretch)
            {
                minPosY = Math.Min(0.0f, (extentHeight * zoomFactor - scrollerVisual.Size.Y) / 2.0f);
            }
            else if (contentAsFE.VerticalAlignment == VerticalAlignment.Bottom)
            {
                minPosY = Math.Min(0.0f, extentHeight * zoomFactor - scrollerVisual.Size.Y);
            }

            return minPosY;
        }

        public string FormatDouble(double value)
        {
            return value.ToString("G2");
        }

        #endregion

        #endregion

        #region x:Bind Converter Helpers

        private muxc.ScrollBarVisibility ObjectToScrollControllerVisibility(object value)
        {
            Enum.TryParse<muxc.ScrollBarVisibility>(value as string, out muxc.ScrollBarVisibility output);
            return output;
        }

        private muxc.ScrollMode ObjectToScrollMode(object value)
        {
            Enum.TryParse<muxc.ScrollMode>(value as string, out muxc.ScrollMode output);
            return output;
        }

        private muxc.ContentOrientation ObjectToContentOrientation(object value)
        {
            Enum.TryParse<muxc.ContentOrientation>(value as string, out muxc.ContentOrientation output);
            return output;
        }

        private void LvIgnoredInputKinds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;

            var stringified = String.Join(',', listView.SelectedItems);
            Enum.TryParse<muxc.InputKind>(stringified, out muxc.InputKind output);

            scroller3.IgnoredInputKind = output;
        }

        #endregion

        #region Property values for binding available options

        private enum ScrollAnimationOptions
        {
            Default,
            Custom1,
            Custom2
        }

        public enum ZoomAnimationOptions
        {
            Default,
            Custom1,
            Custom2
        }


        public string[] ZoomModes => Enum.GetNames(typeof(muxc.ZoomMode));

        public string[] ScrollModes => Enum.GetNames(typeof(muxc.ScrollMode));

        public string[] ScrollBarVisibility => Enum.GetNames(typeof(muxc.ScrollBarVisibility));

        public string[] ContentOrientation => Enum.GetNames(typeof(muxc.ContentOrientation));

        public string[] ScrollingAnimations => Enum.GetNames(typeof(ScrollAnimationOptions));

        public string[] ZoomingAnimations => Enum.GetNames(typeof(ZoomAnimationOptions));

        public string[] InputKinds => Enum.GetNames(typeof(muxc.InputKind));

        #endregion

        private void Scroller_HandleKeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Swallow up and down for gamepad / keyboard input when focused to prevent the Page's ScrollViewer from scrollling
            switch (e.Key)
            {
                case VirtualKey.Up:
                case VirtualKey.Down:
                    e.Handled = true;
                    break;
            }
        }
    }
}
