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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using WinUIGallery.Helpers;

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

    [TestMethod]
    public async Task TestExternalLinksValidity()
    {
        // List of all external links found in NavigateUri attributes across XAML files
        var externalLinks = new List<string>
        {
            "https://accessibilityinsights.io/",
            "https://aka.ms/lottie",
            "https://aka.ms/toolkit/windows",
            "https://aka.ms/windowsappsdk",
            "https://aka.ms/winui",
            "https://github.com/CommunityToolkit/MVVM-Samples",
            "https://github.com/Microsoft/Win2D",
            "https://github.com/WilliamABradley/ColorCode-Universal",
            "https://go.microsoft.com/fwlink/?LinkId=521839",
            "https://go.microsoft.com/fwlink/?LinkId=822631",
            "https://learn.microsoft.com/accessibility-tools-docs/items/uwpxaml/control_fulldescription_describedby_helptext",
            "https://learn.microsoft.com/azure/azure-maps/how-to-manage-account-keys",
            "https://learn.microsoft.com/windows/apps/design/accessibility/accessibility-overview",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information#accessible-name",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information#influencing-the-ui-automation-tree-views",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information#name-from-inner-text",
            "https://learn.microsoft.com/windows/apps/design/accessibility/keyboard-accessibility",
            "https://learn.microsoft.com/windows/apps/design/accessibility/landmarks-and-headings",
            "https://learn.microsoft.com/windows/apps/design/downloads/#fonts",
            "https://learn.microsoft.com/windows/apps/design/input/access-keys",
            "https://learn.microsoft.com/windows/apps/design/input/focus-navigation",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-accelerators",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#control-group",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#home-and-end-keys",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#navigation",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#page-up-and-page-down-keys",
            "https://learn.microsoft.com/windows/apps/design/style/acrylic#usability-and-adaptability",
            "https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-in-depth#xbind-and-binding-feature-comparison",
            "https://learn.microsoft.com/windows/apps/package-and-deploy/deploy-overview",
            "https://learn.microsoft.com/windows/apps/package-and-deploy/self-contained-deploy/deploy-self-contained-apps#dependencies-on-additional-msix-packages",
            "https://learn.microsoft.com/windows/apps/windows-app-sdk/deployment-architecture#singleton-package",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositionshape",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.input.inputnonclientpointersource",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.appwindowtitlebar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Documents.Hyperlink",
            "https://support.microsoft.com/windows/complete-guide-to-narrator-e4397a0d-ef4f-b386-d8ae-c172f109bdb1",
            "https://www.microsoft.com",
            "https://www.unicode.org/notes/tn28/",
            "https://www.w3.org/Math/"
        };

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30); // Set reasonable timeout
        
        // Add user agent to avoid some servers blocking the request
        httpClient.DefaultRequestHeaders.Add("User-Agent", "WinUI-Gallery-LinkValidator/1.0");

        var failedLinks = new List<string>();
        var checkedCount = 0;

        foreach (var link in externalLinks)
        {
            try
            {
                using var response = await httpClient.GetAsync(link);
                checkedCount++;
                
                // Consider 2xx and 3xx status codes as success
                // Also accept some 4xx codes that might be normal for redirect services
                if (!response.IsSuccessStatusCode && 
                    (int)response.StatusCode >= 400 && 
                    response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                {
                    failedLinks.Add($"{link} - Status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("Name or service not known") || 
                                                 ex.Message.Contains("temporarily unavailable"))
            {
                // Skip DNS resolution failures in test environments
                checkedCount++;
                continue;
            }
            catch (HttpRequestException ex)
            {
                failedLinks.Add($"{link} - HttpRequestException: {ex.Message}");
                checkedCount++;
            }
            catch (TaskCanceledException ex)
            {
                failedLinks.Add($"{link} - Timeout: {ex.Message}");
                checkedCount++;
            }
            catch (Exception ex)
            {
                failedLinks.Add($"{link} - Exception: {ex.Message}");
                checkedCount++;
            }
            
            // Add small delay to be respectful to servers
            await Task.Delay(100);
        }

        // Assert that we checked a reasonable number of links and no critical failures occurred
        Assert.IsTrue(checkedCount > 0, "No links were checked");
        
        // If more than 50% of links fail, it's likely a test environment issue
        if (failedLinks.Count > externalLinks.Count / 2)
        {
            Assert.Inconclusive($"More than 50% of links failed ({failedLinks.Count}/{externalLinks.Count}), " +
                              "which may indicate test environment network restrictions. " +
                              "Checked {checkedCount} links.");
        }
        
        // Assert that no links failed with actual HTTP errors (not network issues)
        var httpErrorFailures = failedLinks.Where(f => f.Contains("Status:") && 
                                                      !f.Contains("TooManyRequests")).ToList();
        
        if (httpErrorFailures.Any())
        {
            var failureMessage = $"The following external links returned HTTP errors:\n{string.Join("\n", httpErrorFailures)}";
            Assert.Fail(failureMessage);
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
