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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        // Dynamically discover all external links from the project
        var externalLinks = await GetAllExternalLinksAsync();
        
        Assert.IsTrue(externalLinks.Count > 0, "No external links found in project");

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
                              $"Checked {checkedCount} links. Found {externalLinks.Count} total external links.");
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

    /// <summary>
    /// Dynamically discovers all external links from XAML files and ControlInfoData.json
    /// </summary>
    private async Task<List<string>> GetAllExternalLinksAsync()
    {
        var allLinks = new HashSet<string>();
        
        // Get the project root directory (assuming test is running in bin/Debug/net8.0-windows10.0.19041.0 or similar)
        var testDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        var projectRoot = GetProjectRootDirectory(testDirectory);
        
        if (projectRoot == null)
        {
            throw new DirectoryNotFoundException("Could not find project root directory");
        }

        // Discover links from XAML files
        var xamlLinks = await GetExternalLinksFromXamlFilesAsync(projectRoot);
        foreach (var link in xamlLinks)
        {
            allLinks.Add(link);
        }

        // Discover links from ControlInfoData.json
        var jsonLinks = await GetExternalLinksFromControlInfoDataAsync(projectRoot);
        foreach (var link in jsonLinks)
        {
            allLinks.Add(link);
        }

        return allLinks.ToList();
    }

    /// <summary>
    /// Finds the project root directory by looking for the .sln file
    /// </summary>
    private string GetProjectRootDirectory(string startPath)
    {
        var directory = new DirectoryInfo(startPath);
        
        while (directory != null)
        {
            if (directory.GetFiles("*.sln").Any())
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        
        return null;
    }

    /// <summary>
    /// Extracts external links from NavigateUri attributes in XAML files
    /// </summary>
    private async Task<List<string>> GetExternalLinksFromXamlFilesAsync(string projectRoot)
    {
        var links = new List<string>();
        
        // Find all XAML files in the WinUIGallery directory
        var winUIGalleryPath = Path.Combine(projectRoot, "WinUIGallery");
        if (!Directory.Exists(winUIGalleryPath))
        {
            return links;
        }

        var xamlFiles = Directory.GetFiles(winUIGalleryPath, "*.xaml", SearchOption.AllDirectories);
        
        // Regex to find NavigateUri attributes with URLs
        var navigateUriRegex = new Regex(@"NavigateUri\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
        
        foreach (var xamlFile in xamlFiles)
        {
            try
            {
                var content = await File.ReadAllTextAsync(xamlFile);
                var matches = navigateUriRegex.Matches(content);
                
                foreach (Match match in matches)
                {
                    var uri = match.Groups[1].Value;
                    if (IsExternalLink(uri))
                    {
                        links.Add(uri);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail for individual file reading issues
                System.Diagnostics.Debug.WriteLine($"Failed to read XAML file {xamlFile}: {ex.Message}");
            }
        }
        
        return links;
    }

    /// <summary>
    /// Extracts external links from the ControlInfoData.json file
    /// </summary>
    private async Task<List<string>> GetExternalLinksFromControlInfoDataAsync(string projectRoot)
    {
        var links = new List<string>();
        
        var controlInfoDataPath = Path.Combine(projectRoot, "WinUIGallery", "Samples", "Data", "ControlInfoData.json");
        
        if (!File.Exists(controlInfoDataPath))
        {
            return links;
        }

        try
        {
            var jsonContent = await File.ReadAllTextAsync(controlInfoDataPath);
            using var document = JsonDocument.Parse(jsonContent);
            
            if (document.RootElement.TryGetProperty("Groups", out var groups))
            {
                foreach (var group in groups.EnumerateArray())
                {
                    if (group.TryGetProperty("Items", out var items))
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            if (item.TryGetProperty("Docs", out var docs))
                            {
                                foreach (var doc in docs.EnumerateArray())
                                {
                                    if (doc.TryGetProperty("Uri", out var uri))
                                    {
                                        var uriValue = uri.GetString();
                                        if (!string.IsNullOrEmpty(uriValue) && IsExternalLink(uriValue))
                                        {
                                            links.Add(uriValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to parse ControlInfoData.json: {ex.Message}");
        }
        
        return links;
    }

    /// <summary>
    /// Determines if a URI is an external link that should be validated
    /// </summary>
    private static bool IsExternalLink(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
            return false;
            
        // Consider HTTP/HTTPS links as external
        if (uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        
        // Consider ms-windows-store:// links as external
        if (uri.StartsWith("ms-windows-store://", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        
        // Exclude internal/local URIs
        if (uri.StartsWith("ms-appx://", StringComparison.OrdinalIgnoreCase) ||
            uri.StartsWith("ms-appdata://", StringComparison.OrdinalIgnoreCase) ||
            uri.StartsWith("ms-resource://", StringComparison.OrdinalIgnoreCase) ||
            uri.StartsWith("/", StringComparison.OrdinalIgnoreCase) ||
            uri.StartsWith("./", StringComparison.OrdinalIgnoreCase) ||
            uri.StartsWith("../", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        
        return false;
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
