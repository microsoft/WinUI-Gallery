// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using WinUIGallery.Helpers;
using WinUIGallery.ControlPages;

namespace WinUIGallery.UnitTests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public async Task TestControlInfoDataSource()
    {
        var groups = await ControlInfoDataSource.Instance.GetGroupsAsync();
        var groupsList = groups.ToList();

        var expectedGroups = new List<string>
        {
            "Fundamentals",
            "Design",
            "Accessibility",
            "Menus & toolbars",
            "Collections",
            "Date & time",
            "Basic input",
            "Status & info",
            "Dialogs & flyouts",
            "Scrolling",
            "Layout",
            "Navigation",
            "Media",
            "Styles",
            "Text",
            "Motion",
            "Windowing",
            "System"
        };

        Assert.AreEqual(expectedGroups.Count, groupsList.Count);

        int groupCount = expectedGroups.Count;
        for(int i = 0; i < groupCount; i++)
        {
            var actualTitle = groupsList[i].Title;
            Assert.AreEqual(expectedGroups[i], actualTitle);
        }
    }

    // Use the UITestMethod attribute for tests that need to run on the UI thread.
    [UITestMethod]
    public void TestWrapGrid()
    {
        Layouts.WrapPanel wrapPanel = new()
        {
            Width = 250,
            Height = 250
        };
        for (int i = 0; i < 4; i++) 
        {
            wrapPanel.Children.Add(new Button()
            {
                Width = 120,
                Height = 80,
                Content = $"Button {i}"
            });
        }

        UnitTestApp.UnitTestAppWindow.AddToVisualTree(wrapPanel);
        wrapPanel.UpdateLayout();

        List<Rect> expectedLayouts =
        [
            new Rect(0,    0, 120,  80),
            new Rect(120,  0, 120,  80),
            new Rect(0,   80, 120,  80),
            new Rect(120, 80, 120,  80)
        ];
        for (int i = 0; i < 4; i++)
        {
            var actualLayout = LayoutInformation.GetLayoutSlot(wrapPanel.Children[i] as FrameworkElement);
            Assert.AreEqual(expectedLayouts[i], actualLayout);
        }
    }

    // This test demonstrates executing test code both on and off the UI thread.
    // We use the ExecuteOnUIThread method to run code on the UI thread.
    [TestMethod]
    public void MultiThreadTest()
    {
        Border border = null;
        AutoResetEvent borderSizeChanged = new(false);

        ExecuteOnUIThread(() =>
        {
            Grid grid = new()
            {
                Width = 200,
            };

            border = new Border
            {
                Background = new SolidColorBrush(Colors.Green),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Child = new Rectangle
                {
                    Fill = new SolidColorBrush(Colors.Red),
                    Width = 100,
                }
            };

            border.SizeChanged += (s, e) =>
            {
                borderSizeChanged.Set();
            };

            grid.Children.Add(border);
            
            UnitTestApp.UnitTestAppWindow.AddToVisualTree(grid);
        });
        Assert.IsTrue(borderSizeChanged.WaitOne());

        ExecuteOnUIThread(() =>
        {
            Assert.AreEqual(200, border.ActualWidth);

            border.HorizontalAlignment = HorizontalAlignment.Left;
        });

        Assert.IsTrue(borderSizeChanged.WaitOne());

        ExecuteOnUIThread(() =>
        {
            Assert.AreEqual(100, border.ActualWidth);
        });
    }

    private void ExecuteOnUIThread(Action action)
    {
        AutoResetEvent done = new(false);
        DispatcherQueue dispatcherQueue = UnitTestApp.UnitTestAppWindow.DispatcherQueue;
        if (dispatcherQueue.HasThreadAccess)
        {
            action();
        }
        else
        {
            Exception exception = null;
            var success = dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex) 
                { 
                    exception = ex;
                }
                finally 
                { 
                    done.Set(); 
                }
            });
            Assert.IsTrue(success);
            Assert.IsTrue(done.WaitOne());
            if(exception != null)
            {
                Assert.Fail(exception.ToString());
            }
        }
    }

    [UITestMethod]
    public void TestFlipViewAutomationProperties()
    {
        FlipView flipView = new()
        {
            Height = 270,
            MaxWidth = 400
        };
        
        // Set the LocalizedControlType as done in the FlipViewPage
        Microsoft.UI.Xaml.Automation.AutomationProperties.SetLocalizedControlType(flipView, "FlipView");
        
        // Add some test items
        flipView.Items.Add(new Rectangle() { Fill = new SolidColorBrush(Colors.Red), Width = 100, Height = 100 });
        flipView.Items.Add(new Rectangle() { Fill = new SolidColorBrush(Colors.Blue), Width = 100, Height = 100 });
        
        UnitTestApp.UnitTestAppWindow.AddToVisualTree(flipView);
        flipView.UpdateLayout();
        
        // Verify that the LocalizedControlType is set correctly
        var localizedControlType = Microsoft.UI.Xaml.Automation.AutomationProperties.GetLocalizedControlType(flipView);
        Assert.AreEqual("FlipView", localizedControlType, "FlipView should have LocalizedControlType set to 'FlipView'");
        
        // Verify that the control is accessible
        Assert.IsNotNull(flipView, "FlipView should be created successfully");
        Assert.AreEqual(2, flipView.Items.Count, "FlipView should contain 2 items");
    }
    
    [UITestMethod]
    public void TestFlipViewPageAutomationProperties()
    {
        // Create an instance of FlipViewPage to test the actual implementation
        FlipViewPage flipViewPage = new();
        
        UnitTestApp.UnitTestAppWindow.AddToVisualTree(flipViewPage);
        flipViewPage.UpdateLayout();
        
        // Find FlipView controls in the page and verify their LocalizedControlType
        var flipViews = FindVisualChildren<FlipView>(flipViewPage);
        
        Assert.IsTrue(flipViews.Count() >= 3, "FlipViewPage should contain at least 3 FlipView controls");
        
        foreach (var flipView in flipViews)
        {
            var localizedControlType = Microsoft.UI.Xaml.Automation.AutomationProperties.GetLocalizedControlType(flipView);
            Assert.AreEqual("FlipView", localizedControlType, 
                $"FlipView should have LocalizedControlType set to 'FlipView' for accessibility compliance");
        }
    }
    
    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        ExecuteOnUIThread(() =>
        {
            UnitTestApp.UnitTestAppWindow.CleanupVisualTree();
        });
    }
}
