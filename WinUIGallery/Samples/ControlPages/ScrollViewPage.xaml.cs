// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Windows.Globalization.NumberFormatting;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class ScrollViewPage : Page
{
    // Cache for storing Example3 C# sample code content
    private readonly Dictionary<string, string> _example3CodeCache = new();

    public ScrollViewPage()
    {
        this.InitializeComponent();

        this.Loaded += ScrollViewPage_Loaded;
    }

    // Example1
    private void ScrollViewPage_Loaded(object sender, RoutedEventArgs e)
    {
        scrollView1.ZoomTo(4.0f, null, new ScrollingZoomOptions(ScrollingAnimationMode.Enabled, ScrollingSnapPointsMode.Ignore));

        IncrementNumberRounder rounder = new IncrementNumberRounder
        {
            Increment = 0.1,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };

        DecimalFormatter formatter = new DecimalFormatter
        {
            IntegerDigits = 2,
            FractionDigits = 1,
            NumberRounder = rounder
        };
        nbZoomFactor.NumberFormatter = formatter;

        this.Loaded -= ScrollViewPage_Loaded;
    }

    private void CmbZoomMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (scrollView1 != null)
        {
            if (sender is ComboBox cmb)
            {
                scrollView1.ZoomMode = (ScrollingZoomMode)cmb.SelectedIndex;
            }
        }
    }

    private void NbZoomFactor_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (scrollView1 != null)
        {
            scrollView1.ZoomTo((float)e.NewValue, null);
        }
    }

    private void CmbHorizontalScrollMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (scrollView1 != null)
        {
            if (sender is ComboBox cmb)
            {
                scrollView1.HorizontalScrollMode = (ScrollingScrollMode)cmb.SelectedIndex;
            }
        }
    }

    private void CmbHorizontalScrollBarVisibility_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (scrollView1 != null)
        {
            if (sender is ComboBox cmb)
            {
                scrollView1.HorizontalScrollBarVisibility = (ScrollingScrollBarVisibility)cmb.SelectedIndex;
            }
        }
    }

    private void CmbVerticalScrollMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (scrollView1 != null)
        {
            if (sender is ComboBox cmb)
            {
                scrollView1.VerticalScrollMode = (ScrollingScrollMode)cmb.SelectedIndex;
            }
        }
    }

    private void CmbVerticalScrollBarVisibility_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (scrollView1 != null)
        {
            if (sender is ComboBox cmb)
            {
                scrollView1.VerticalScrollBarVisibility = (ScrollingScrollBarVisibility)cmb.SelectedIndex;
            }
        }
    }

    // Example2
    private void NbVerticalVelocity_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (double.IsNaN(e.OldValue))
        {
            return;
        }

        if (scrollView2 != null)
        {
            // Cancel previous constant scroll velocity
            scrollView2.ScrollBy(
                0, 0,
                new ScrollingScrollOptions(ScrollingAnimationMode.Disabled, ScrollingSnapPointsMode.Ignore));

            float verticalConstantVelocity = (float)nbVerticalVelocity.Value;

            if (e.NewValue <= 30.0 && e.NewValue >= -30)
            {
                // Only value smaller than -30 or greater than 30 trigger a scroll
                if (e.NewValue < e.OldValue)
                {
                    if (scrollView2.VerticalOffset == 0)
                    {
                        verticalConstantVelocity = 30;
                    }
                    else
                    {
                        verticalConstantVelocity = -30;
                    }
                }
                else
                {
                    if (scrollView2.VerticalOffset == scrollView2.ScrollableHeight)
                    {
                        verticalConstantVelocity = -30;
                    }
                    else
                    {
                        verticalConstantVelocity = 30;
                    }
                }
            }
            else if (e.NewValue < 30.0 && scrollView2.VerticalOffset == 0)
            {
                verticalConstantVelocity = 30;
            }
            else if (e.NewValue > 30.0 && scrollView2.VerticalOffset == scrollView2.ScrollableHeight)
            {
                verticalConstantVelocity = -30;
            }

            nbVerticalVelocity.Value = verticalConstantVelocity;

            scrollView2.AddScrollVelocity(
                new Vector2(0f, verticalConstantVelocity),
                new Vector2() /*empty inertia decay rate for a constant velocity*/);
        }
    }

    // Example3
    private void BtnScrollWithAnimation_Click(object sender, RoutedEventArgs e)
    {
        if (scrollView3 != null)
        {
            scrollView3.ScrollTo(scrollView3.HorizontalOffset, GetTargetVerticalOffset(), new ScrollingScrollOptions(ScrollingAnimationMode.Enabled, ScrollingSnapPointsMode.Ignore));
        }
    }

    private void ScrollView_ScrollAnimationStarting(ScrollView sender, ScrollingScrollAnimationStartingEventArgs e)
    {
        Vector3KeyFrameAnimation stockKeyFrameAnimation = e.Animation as Vector3KeyFrameAnimation;

        if (stockKeyFrameAnimation != null)
        {
            if (cmbVerticalAnimation.SelectedIndex == 0)
            {
                stockKeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(nbAnimationDuration.Value);
            }
            else
            {
                double targetVerticalOffset = GetTargetVerticalOffset();
                float targetVerticalPosition = (float)targetVerticalOffset;
                Vector3KeyFrameAnimation customKeyFrameAnimation = stockKeyFrameAnimation.Compositor.CreateVector3KeyFrameAnimation();

                if (cmbVerticalAnimation.SelectedIndex == 1)
                {
                    // Accordion case
                    float deltaVerticalPosition = 0.1f * (float)(targetVerticalOffset - scrollView3.VerticalOffset);

                    for (int step = 0; step < 3; step++)
                    {
                        customKeyFrameAnimation.InsertKeyFrame(
                            1.0f - (0.4f / (float)Math.Pow(2, step)),
                            new Vector3((float)scrollView3.HorizontalOffset, targetVerticalPosition + deltaVerticalPosition, 0.0f));
                        deltaVerticalPosition /= -2.0f;
                    }

                    customKeyFrameAnimation.InsertKeyFrame(1.0f, new Vector3((float)scrollView3.HorizontalOffset, targetVerticalPosition, 0.0f));
                }
                else
                {
                    // Teleportation case
                    float deltaVerticalPosition = (float)(targetVerticalOffset - scrollView3.VerticalOffset);

                    CubicBezierEasingFunction cubicBezierStart = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
                        new Vector2(1.0f, 0.0f),
                        new Vector2(1.0f, 0.0f));

                    StepEasingFunction step = stockKeyFrameAnimation.Compositor.CreateStepEasingFunction(1);

                    CubicBezierEasingFunction cubicBezierEnd = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
                        new Vector2(0.0f, 1.0f),
                        new Vector2(0.0f, 1.0f));

                    customKeyFrameAnimation.InsertKeyFrame(
                        0.499999f,
                        new Vector3((float)scrollView3.HorizontalOffset, targetVerticalPosition - 0.9f * deltaVerticalPosition, 0.0f),
                        cubicBezierStart);
                    customKeyFrameAnimation.InsertKeyFrame(
                        0.5f,
                        new Vector3((float)scrollView3.HorizontalOffset, targetVerticalPosition - 0.1f * deltaVerticalPosition, 0.0f),
                        step);
                    customKeyFrameAnimation.InsertKeyFrame(
                        1.0f,
                        new Vector3((float)scrollView3.HorizontalOffset, targetVerticalPosition, 0.0f),
                        cubicBezierEnd);
                }

                customKeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(nbAnimationDuration.Value);
                e.Animation = customKeyFrameAnimation;
            }
        }
    }

    private double GetTargetVerticalOffset()
    {
        if (scrollView3.VerticalOffset > scrollView3.ScrollableHeight / 2.0)
        {
            return scrollView3.ScrollableHeight / 5.0;
        }
        else
        {
            return 4.0 * scrollView3.ScrollableHeight / 5.0;
        }
    }

    private void cmbVerticalAnimation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateExample3Content();
    }

    private void nbAnimationDuration_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        UpdateExample3Content();
    }

    private void UpdateExample3Content()
    {
        string sampleCodeFileName = null;

        switch (cmbVerticalAnimation.SelectedIndex)
        {
            case 0:
                sampleCodeFileName = "ScrollViewSample3_DefaultAnimation_cs";
                break;
            case 1:
                sampleCodeFileName = "ScrollViewSample3_AccordionAnimation_cs";
                break;
            case 2:
                sampleCodeFileName = "ScrollViewSample3_TeleportationAnimation_cs";
                break;
            default:
                sampleCodeFileName = null;
                break;
        }

        if (sampleCodeFileName != null)
        {
            Example3.CSharp = GetExample3CodeContent(sampleCodeFileName);

            if (nbAnimationDuration != null)
            {
                Example3.CSharp = Example3.CSharp.Replace("nbAnimationDuration.Value", nbAnimationDuration.Value.ToString());
            }
        }
    }

    // Method to get sample code content (with caching)
    private string GetExample3CodeContent(string sampleCodeFileName)
    {
        if (!_example3CodeCache.TryGetValue(sampleCodeFileName, out var content))
        {
            string folderPath = AppContext.BaseDirectory;
            if (NativeMethods.IsAppPackaged)
            {
                folderPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            }
            string filePath = Path.Combine(folderPath, "Samples", "SampleCode", "ScrollView", $"{sampleCodeFileName}.txt");
            if (File.Exists(filePath))
            {
                content = File.ReadAllText(filePath);
                _example3CodeCache[sampleCodeFileName] = content; // Cache the content
            }
        }
        return content;
    }
}
