//******************************************************************************
//
// Copyright (c) 2024 Microsoft Corporation. All rights reserved.
//
// This code is licensed under the MIT License (MIT).
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//******************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WinUIGallery.UITests;

[TestClass]
public class SessionManager
{
    private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
    private static readonly string[] WinUIGalleryAppIDs = [
        // WinUI 3 Gallery apps built in the lab
        "Microsoft.WinUI3ControlsGallery.Debug_grv3cx5qrw0gp!App",
        "Microsoft.WinUI3ControlsGallery_grv3cx5qrw0gp!App",
        // WinUI 3 Gallery apps built locally
        "Microsoft.WinUI3ControlsGallery.Debug_8wekyb3d8bbwe!App",
        "Microsoft.WinUI3ControlsGallery_8wekyb3d8bbwe!App"
    ];

    private static uint appIdIndex = 0;

    private static WindowsDriver<WindowsElement> _session;
    public static WindowsDriver<WindowsElement> Session
    {
        get
        {
            if (_session is null)
            {
                Setup(null);
            }
            return _session;
        }
    }

    public static TestContext TestContext { get; set; }

    private static string screenshotDirectory;

    [AssemblyInitialize]
    public static void Setup(TestContext context)
    {
        TestContext = context;

        string outputDirectory;

        if (context.Properties.Contains("ArtifactStagingDirectory"))
        {
            outputDirectory = context.Properties["ArtifactStagingDirectory"].ToString();
        }
        else
        {
            outputDirectory = context.TestRunResultsDirectory;
        }

        screenshotDirectory = Path.Combine(outputDirectory, "Screenshots");

        if (_session is null)
        {
            int timeoutCount = 50;

            TryInitializeSession();
            if (_session is null)
            {
                // WinAppDriver is probably not running, so lets start it!
                string winAppDriverX64Path = Path.Join(Environment.GetEnvironmentVariable("ProgramFiles"), "Windows Application Driver", "WinAppDriver.exe");
                string winAppDriverX86Path = Path.Join(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "Windows Application Driver", "WinAppDriver.exe");

                if (File.Exists(winAppDriverX64Path))
                {
                    Process.Start(winAppDriverX64Path);
                }
                else if (File.Exists(winAppDriverX86Path))
                {
                    Process.Start(winAppDriverX86Path);
                }
                else
                {
                    throw new Exception("Unable to start WinAppDriver since no suitable location was found.");
                }

                Thread.Sleep(10000);
                TryInitializeSession();
            }

            while (_session is null && timeoutCount < 1000 * 4)
            {
                TryInitializeSession();
                Thread.Sleep(timeoutCount);
                timeoutCount *= 2;
            }

            Thread.Sleep(3000);
            Assert.IsNotNull(_session);
            Assert.IsNotNull(_session.SessionId);
            AxeHelper.InitializeAxe();

            // Dismiss the disclaimer window that may pop up on the very first application launch
            // If the disclaimer is not found, this throws an exception, so lets catch that
            try
            {
                _session.FindElementByName("Disclaimer").FindElementByName("Accept").Click();
            }
            catch (OpenQA.Selenium.WebDriverException) { }

            // Wait if something is still animating in the visual tree
            _session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            _session.Manage().Window.Maximize();
        }
    }

    [AssemblyCleanup()]
    public static void TestRunTearDown()
    {
        TearDown();
    }

    public static void TearDown()
    {
        if (_session is not null)
        {
            _session.CloseApp();
            _session.Quit();
            _session = null;
        }
    }

    public static void TakeScreenshot(string fileName)
    {
        Directory.CreateDirectory(screenshotDirectory);
        _session.GetScreenshot().SaveAsFile(Path.Join(screenshotDirectory, $"{fileName}.png"));
    }

    public static void DumpTree()
    {
        Logger.LogMessage("=================");
        Logger.LogMessage("Begin visual tree");
        Logger.LogMessage("=================");

        foreach (WindowsElement element in _session.FindElementsByXPath("/*"))
        {
            DumpTreeHelper(element, 0);
        }

        Logger.LogMessage("===============");
        Logger.LogMessage("End visual tree");
        Logger.LogMessage("===============");
    }

    private static void DumpTreeHelper(WindowsElement root, int depth)
    {
        string indent = string.Empty;

        for (int i = 0; i < depth; i++)
        {
            indent += "|";

            if (i == depth - 1)
            {
                indent += "-";
            }
            else
            {
                indent += " ";
            }
        }

        if (root.Displayed && !string.IsNullOrEmpty(root.TagName))
        {
            string message;

            if (string.IsNullOrEmpty(root.Text))
            {
                message = $"{indent}{root.TagName}";
            }
            else
            {
                message = $"{indent}{root.TagName} [{root.Text}]";
            }

            Logger.LogMessage(message.Replace("{", "{{").Replace("}", "}}"));
        }

        foreach (WindowsElement child in root.FindElementsByXPath("*/*"))
        {
            DumpTreeHelper(child, root.Displayed ? depth + 1 : depth);
        }
    }

    private static void TryInitializeSession()
    {
        AppiumOptions appiumOptions = new AppiumOptions();
        appiumOptions.AddAdditionalCapability("app", WinUIGalleryAppIDs[appIdIndex]);
        appiumOptions.AddAdditionalCapability("deviceName", "WindowsPC");
        try
        {
            _session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appiumOptions);
        }
        catch (OpenQA.Selenium.WebDriverException exc)
        {
            // Use next app ID since the current one was failing
            if (exc.Message.Contains("Package was not found"))
            {
                appIdIndex++;
            }
            else
            {
                Console.WriteLine("Failed to update start driver, got exception:" + exc.Message);
            }
        }
    }
}

