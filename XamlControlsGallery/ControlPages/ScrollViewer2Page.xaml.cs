using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using mux = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScrollViewer2Page : ItemsPageBase
    {
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

        public ScrollViewer2Page()
        {
            this.InitializeComponent();

            this.scroller1.StateChanged += Scroller_StateChanged;
        }

        #region Converters

        private mux::ScrollBarVisibility ObjectToScrollControllerVisibility(object value)
        {
            Enum.TryParse<mux::ScrollBarVisibility>(value as string, out mux::ScrollBarVisibility output);
            return output;
        }

        private mux::ScrollMode ObjectToScrollMode(object value)
        {
            Enum.TryParse<mux::ScrollMode>(value as string, out mux::ScrollMode output);
            return output;
        }

        private mux::ContentOrientation ObjectToContentOrientation(object value)
        {
            Enum.TryParse<mux::ContentOrientation>(value as string, out mux::ContentOrientation output);
            return output;
        }

        #endregion

        #region Property values

        public string[] ZoomModes => Enum.GetNames(typeof(mux::ZoomMode));

        public string[] ScrollModes => Enum.GetNames(typeof(mux::ScrollMode));

        public string[] ScrollBarVisibility => Enum.GetNames(typeof(mux::ScrollBarVisibility));

        public string[] ContentOrientation => Enum.GetNames(typeof(mux::ContentOrientation));

        public string[] ScrollingAnimations => Enum.GetNames(typeof(ScrollAnimationOptions));

        public string[] ZoomingAnimations => Enum.GetNames(typeof(ZoomAnimationOptions));
        #endregion

        #region Zooming

        private void ZoomModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (scroller1 != null && ZoomSlider != null)
            {
                ComboBox cb = sender as ComboBox;
                if (cb != null)
                {
                    switch (cb.SelectedIndex)
                    {
                        case 0: // Enabled
                            scroller1.ZoomMode = mux::ZoomMode.Enabled;
                            ZoomSlider.IsEnabled = true;
                            break;
                        default: // Disabled
                            scroller1.ZoomMode = mux::ZoomMode.Disabled;
                            scroller1.ZoomTo(1.0f, new Vector2());
                            ZoomSlider.Value = 1;
                            ZoomSlider.IsEnabled = false;
                            break;
                    }
                }
            }
        }

        private void Scroller_StateChanged(mux.ScrollViewer sender, object args)
        {
            // each time it comes to rest update the slider to reflect the current zoom factor
            if (sender.State == mux.InteractionState.Idle)
            {
                ZoomSlider.Value = Math.Round(sender.ZoomFactor, (int)(10 * ZoomSlider.StepFrequency));
                ZoomSlider.ValueChanged += ZoomSlider_ValueChanged;
            }
            else 
            {
                ZoomSlider.ValueChanged -= ZoomSlider_ValueChanged;
            }
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var center = new Vector2((float)this.scroller1.ActualWidth / 2, (float)this.scroller1.ActualHeight / 2);
            if (scroller1 != null)
            {
                // Zoom based on the center point of the current viewport
                scroller1.ZoomTo((float)e.NewValue, center);
            }
        }

        #endregion

        #region Custom Animations & Scroll Anchoring
        private void image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            xposSlider.Maximum = image.ActualWidth - scroller3.ActualWidth;
            yposSlider.Maximum = image.ActualHeight - scroller3.ActualHeight;
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            var newX = (xposSlider.Value / xposSlider.Maximum) * (image.ActualWidth * scroller3.ZoomFactor - scroller3.ActualWidth);
            var newY = (yposSlider.Value / yposSlider.Maximum) * (image.ActualHeight * scroller3.ZoomFactor - scroller3.ActualHeight);

            // Scroll
            scroller3.ScrollTo(newX, newY);

            // Zoom
            scroller3.ZoomTo((float)zoomFactorSlider.Value, new Vector2((float)scroller3.ActualWidth / 2, (float)scroller3.ActualHeight / 2));

        }


        #region Adjust scrolling animation

        private void Scroller3_ScrollAnimationStarting(mux.ScrollViewer sender, mux.ScrollAnimationStartingEventArgs args)
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
                        //if (cmbOffsetsKind.SelectedIndex == 1)
                        //{
                        //    targetHorizontalOffset += scroller3.HorizontalOffset;
                        //}
                        float targetHorizontalPosition = ComputeHorizontalPositionFromOffset(targetHorizontalOffset);

                        double targetVerticalOffset = args.EndPosition.Y;
                        //if (cmbOffsetsKind.SelectedIndex == 1)
                        //{
                        //    targetVerticalOffset += scroller3.VerticalOffset;
                        //}
                        float targetVerticalPosition = ComputeVerticalPositionFromOffset(targetVerticalOffset);

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

        private void Scroller3_ZoomAnimationStarting(mux.ScrollViewer sender, mux.ZoomAnimationStartingEventArgs args)
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
                        //if (cmbZoomFactorKind.SelectedIndex == 1)
                        //{
                        //    targetZoomFactor += scroller.ZoomFactor;
                        //}

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


        #region Scroll anchoring

        private void scroller4_AnchorRequested(mux.ScrollViewer sender, mux.ScrollerAnchorRequestedEventArgs args)
        {
            // Raised each time the Scroller is searching for an anchor element to track its position before/after its arrange pass
            // which will then be used to determine how much automatic shift to apply to its viewport to maintain the relative position
            // of that anchor element

            // We'll just register the children of the StackPanel as potential candidates.  It will select the one nearest
            // the anchor point which is determined by its HorizontalAnchorRatio + VerticalAnchorRatio
            //if (tsAnchoringEnabled.IsOn)
            //{
                foreach (var child in this.stackPanel.Children)
                {
                    args.AnchorCandidates.Add(child);
                }
            //}
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Insert(0, 16);
        }

        private int operationCount = 0;
        //private Button currentAnchor = null;
        private UIElement anchorElement = null;
        private SolidColorBrush itemBorderBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private SolidColorBrush itemBackgroundBrush = new SolidColorBrush(Windows.UI.Colors.DarkGray);

        private void Insert(int newIndex, int newCount)
        {
            if (newIndex < 0 || newIndex > stackPanel.Children.Count || newCount <= 0)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < newCount; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "TB#" + stackPanel.Children.Count + "_" + operationCount;
                textBlock.Name = "textBlock" + stackPanel.Children.Count + "_" + operationCount;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                Button button = new Button();
                button.Name = "button" + stackPanel.Children.Count + "_" + operationCount;
                button.BorderThickness = button.Margin = new Thickness(3);
                button.BorderBrush = itemBorderBrush;
                button.Background = itemBackgroundBrush;
                //if (chkHorizontalOrientation.IsChecked == true)
                //{
                //    button.Width = 120;
                //    button.Height = 170;
                //}
                //else
                //{
                button.Width = 170;
                button.Height = 120;
                //}
                button.Content = textBlock;

                button.CanBeScrollAnchor = true;

                button.Click += AnchoringDemoButton_Click;

                stackPanel.Children.Insert(newIndex + i, button);
            }

            operationCount++;
        }

        private void AnchoringDemoButton_Click(object sender, RoutedEventArgs e)
        {
            var index = stackPanel.Children.IndexOf((Button)sender).ToString();

            tbInsertAt.Text = String.Empty;
            tbRemoveAt.Text = String.Empty;
            tbExpandAt.Text = String.Empty;
            tbShrinkAt.Text = String.Empty;

            if (Window.Current.CoreWindow.GetAsyncKeyState(Windows.System.VirtualKey.Shift).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
            {
                tbRemoveAt.Text = index;
            }
            else if (Window.Current.CoreWindow.GetAsyncKeyState(Windows.System.VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
            {
                tbExpandAt.Text = index;
            }
            else if (Window.Current.CoreWindow.GetAsyncKeyState(Windows.System.VirtualKey.Menu).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
            {
                tbShrinkAt.Text = index;
            }
            else
            {
                tbInsertAt.Text = index;
            }
        }

        private void Remove(int oldIndex, int oldCount)
        {
            if (oldIndex < 0 || oldIndex > stackPanel.Children.Count - oldCount || oldCount <= 0)
            {
                throw new ArgumentException();
            }

            bool isAnchorRemoved = false;

            for (int i = 0; i < oldCount; i++)
            {
                if (!isAnchorRemoved && anchorElement == stackPanel.Children[oldIndex])
                {
                    isAnchorRemoved = true;
                }
                stackPanel.Children.RemoveAt(oldIndex);
            }

            //if (isAnchorRemoved)
            //{
            //    BtnSetAnchorElement_Click(null, null);
            //}

            operationCount++;
        }

        private void Replace(int oldIndex, int oldCount, int newCount)
        {
            if (oldIndex < 0 || oldIndex > stackPanel.Children.Count - oldCount || oldCount <= 0 || newCount <= 0)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < oldCount; i++)
            {
                stackPanel.Children.RemoveAt(oldIndex);
            }

            for (int i = 0; i < newCount; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "Item #" + stackPanel.Children.Count + "_" + operationCount;
                textBlock.Name = "textBlock" + stackPanel.Children.Count + "_" + operationCount;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                Button button = new Button();
                button.Name = "border" + stackPanel.Children.Count + "_" + operationCount;
                button.BorderThickness = button.Margin = new Thickness(3);
                button.BorderBrush = itemBorderBrush;
                button.Background = itemBackgroundBrush;
                //if (chkHorizontalOrientation.IsChecked == true)
                //{
                //    button.Width = 120;
                //    button.Height = 170;
                //}
                //else
                //{
                button.Width = 170;
                button.Height = 120;
                //}
                button.Content = textBlock;

                stackPanel.Children.Insert(oldIndex + i, button);
            }
        }

        private void Shrink(int index, int amount)
        {
            if (index < 0 || index >= stackPanel.Children.Count)
            {
                throw new ArgumentException();
            }

            Button button = stackPanel.Children[index] as Button;
            button.Height = Math.Max(20, button.Height - 20 * amount);
        }

        private void Expand(int index, int amount)
        {
            if (index < 0 || index >= stackPanel.Children.Count)
            {
                throw new ArgumentException();
            }

            Button button = stackPanel.Children[index] as Button;
            button.Height += 20 * amount;
        }

        #endregion

        #region Helper methods
        private float ComputeHorizontalPositionFromOffset(double offset)
        {
            return (float)(offset + ComputeMinHorizontalPosition(scroller3.ZoomFactor));
        }

        private float ComputeVerticalPositionFromOffset(double offset)
        {
            return (float)(offset + ComputeMinVerticalPosition(scroller3.ZoomFactor));
        }

        private float ComputeMinHorizontalPosition(float zoomFactor)
        {
            UIElement child = scroller3.Content;

            if (child == null)
            {
                return 0;
            }

            FrameworkElement childAsFE = child as FrameworkElement;

            if (childAsFE == null)
            {
                return 0;
            }

            Thickness childMargin = childAsFE.Margin;
            Visual scrollerVisual = Windows.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(scroller3);
            double childWidth = double.IsNaN(childAsFE.Width) ? childAsFE.ActualWidth : childAsFE.Width;
            float minPosX = 0.0f;
            float extentWidth = Math.Max(0.0f, (float)(childWidth + childMargin.Left + childMargin.Right));

            if (childAsFE.HorizontalAlignment == HorizontalAlignment.Center ||
                childAsFE.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                minPosX = Math.Min(0.0f, (extentWidth * zoomFactor - scrollerVisual.Size.X) / 2.0f);
            }
            else if (childAsFE.HorizontalAlignment == HorizontalAlignment.Right)
            {
                minPosX = Math.Min(0.0f, extentWidth * zoomFactor - scrollerVisual.Size.X);
            }

            return minPosX;
        }

        private float ComputeMinVerticalPosition(float zoomFactor)
        {
            UIElement child = scroller3.Content;

            if (child == null)
            {
                return 0;
            }

            FrameworkElement childAsFE = child as FrameworkElement;

            if (childAsFE == null)
            {
                return 0;
            }

            Thickness childMargin = childAsFE.Margin;
            Visual scrollerVisual = Windows.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(scroller3);
            double childHeight = double.IsNaN(childAsFE.Height) ? childAsFE.ActualHeight : childAsFE.Height;
            float minPosY = 0.0f;
            float extentHeight = Math.Max(0.0f, (float)(childHeight + childMargin.Top + childMargin.Bottom));

            if (childAsFE.VerticalAlignment == VerticalAlignment.Center ||
                childAsFE.VerticalAlignment == VerticalAlignment.Stretch)
            {
                minPosY = Math.Min(0.0f, (extentHeight * zoomFactor - scrollerVisual.Size.Y) / 2.0f);
            }
            else if (childAsFE.VerticalAlignment == VerticalAlignment.Bottom)
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

        private void horizRatioSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Canvas.SetLeft(rectAnchorPoint, scroller2.ActualWidth * scroller2.HorizontalAnchorRatio /*- scroller2.HorizontalOffset - rectAnchorPoint.Width / 2*/);
        }

        private void vertRatioSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Canvas.SetTop(rectAnchorPoint, scroller2.ActualHeight * scroller2.VerticalAnchorRatio /*- scroller2.VerticalOffset - rectAnchorPoint.Height / 2*/);
        }

        private void Doit_Click(object sender, RoutedEventArgs e)
        {
            var remove = Int32.TryParse(tbRemoveAt.Text, out int removalIndex);
            var insert = Int32.TryParse(tbInsertAt.Text, out int insertionIndex);
            var expand = Int32.TryParse(tbExpandAt.Text, out int expandIndex);
            var shrink = Int32.TryParse(tbShrinkAt.Text, out int shrinkIndex);

            Random rand = new Random();
            if (remove && IsValidIndex(removalIndex)) Remove(removalIndex, 1);
            if (insert && IsValidIndex(insertionIndex)) Insert(insertionIndex, 1);
            if (expand && IsValidIndex(expandIndex)) Expand(expandIndex, rand.Next(1, 4));
            if (shrink && IsValidIndex(shrinkIndex)) Shrink(shrinkIndex, rand.Next(1, 4));
        }

        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < this.stackPanel.Children.Count;
        }

        //private void tsAnchoringEnabled_Toggled(object sender, RoutedEventArgs e)
        //{
        //    var toggleSwitch = sender as ToggleSwitch;
        //    scroller2.IsAnchoredAtHorizontalExtent = tsAnchoringEnabled.IsOn;
        //    scroller2.IsAnchoredAtVerticalExtent = tsAnchoringEnabled.IsOn;
        //}

        #endregion
    }
}
