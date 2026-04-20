// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Axe.Windows.Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium.Appium.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class AxeScanAll : TestBase
{
    public static readonly string jsonUri = "ControlInfoData.json";
    public static new WindowsDriver<WindowsElement> Session => SessionManager.Session;

    // Pages that are completely excluded from Axe scanning due to AxeWindowsAutomationException
    // or other issues that prevent scanning entirely.
    public static string[] ExclusionList =
    [
        // https://github.com/microsoft/axe-windows/issues/662
        // AxeWindowsAutomationException: Failed to get the root element(s) of the specified process
        "PersonPicture",
        "TabView",
        "MediaPlayerElement",
        // WebView2 hosts Chromium content. Axe.Windows throws a NullReferenceException
        // inside DesktopElementExtensionMethods.AddLogicalSizePseudoProperty during the
        // parallel tree walk, before any rule filtering can take effect, so per-rule
        // exclusions are insufficient.
        "WebView2",
        // MapControl is internally backed by a WebView2 hosting Azure Maps. The system
        // control has no public Dispose path, so its embedded WebView2 (and its UIA
        // Pane + Chromium RootWebArea) leaks into the process tree even after the page
        // is unloaded, contaminating every subsequent Axe scan with a long
        // data:text/html;base64 Name. Skipping the page entirely prevents the WebView2
        // from ever being instantiated during the test run.
        "MapControl"
    ];

    // Per-page rule exclusions for known framework-level issues that cannot be fixed in app code.
    // Prefer adding targeted exclusions here over globally disabling rules in AxeHelper.
    private static readonly Dictionary<string, RuleId[]> PageRuleExclusions = new()
    {
        // External CommunityToolkit SettingsExpander does not pass Axe testing
        // https://github.com/CommunityToolkit/Windows/issues/240
        ["Icons"] =
        [
            RuleId.NameNotNull,
            RuleId.NameReasonableLength,
        ],
    };

    public class ControlInfoData
    {
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        [JsonProperty("UniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("Items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("UniqueId")]
        public string UniqueId { get; set; }
    }

    private static IEnumerable<object[]> TestData()
    {
        var testCases = new List<object[]>();

        string jsonContent = System.IO.File.ReadAllText(jsonUri);
        var controlInfoData = JsonConvert.DeserializeObject<ControlInfoData>(jsonContent);

        foreach (var group in controlInfoData.Groups)
        {
            var sectionName = group.UniqueId;

            // Select all row names within the current table
            var items = group.Items;

            foreach (var item in items)
            {
                var pageName = item.UniqueId;

                // Skip pages in the exclusion list.
                if (ExclusionList.Contains(pageName))
                {
                    continue;
                }
                testCases.Add([sectionName, pageName]);
            }
        }

        return testCases;
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
    }

    [TestMethod]
    [DynamicData(nameof(TestData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetCustomDynamicDataDisplayName))]
    [TestProperty("Description", "Scan pages in the WinUIGallery for accessibility issues.")]
    public void ValidatePageAccessibilityWithAxe(string sectionName, string pageName)
    {
        // Look up per-page rule exclusions for this page
        PageRuleExclusions.TryGetValue(pageName, out RuleId[] ruleExclusions);

        try
        {
            Logger.LogMessage($"Opening page \"{pageName}\".");

            // Click into page and check for accessibility issues.
            var page = Session.FindElementByAccessibilityId(pageName);
            page.Click();

            AxeHelper.AssertNoAccessibilityErrors(ruleExclusions);
        }
        catch (OpenQA.Selenium.WebDriverException exc)
        {
            if (exc.Message.Contains("element could not be located"))
            {
                try
                {
                    Logger.LogMessage($"Page not found. Opening section \"{sectionName}\" first.");

                    // If element is not found, expand tree view as it is nested.
                    var section = Session.FindElementByAccessibilityId(sectionName);
                    section.Click();

                    // wait for tree to expand
                    Thread.Sleep(1000);

                    // Click into page and check for accessibility issues.
                    var page = Session.FindElementByAccessibilityId(pageName);
                    page.Click();

                    AxeHelper.AssertNoAccessibilityErrors(ruleExclusions);
                }
                catch (OpenQA.Selenium.WebDriverException exc2)
                {
                    Logger.LogMessage($"Section \"{sectionName}\" not found either.");
                    Logger.LogMessage(exc2.Message);

                    SessionManager.DumpTree();
                    SessionManager.TakeScreenshot($"{sectionName}.{pageName}");

                    throw;
                }
            }
            else if (exc.Message.Contains("Currently selected window has been closed"))
            {
                Logger.LogMessage("Window closed. Reinitializing session.");
                SessionManager.TakeScreenshot($"{sectionName}.{pageName}");
                SessionManager.Setup(null);
            }
            else
            {
                Logger.LogMessage(exc.Message);
                SessionManager.TakeScreenshot($"{sectionName}.{pageName}");
            }

        }
    }

    public static string GetCustomDynamicDataDisplayName(MethodInfo methodInfo, object[] data)
    {
        return string.Format("Validate{0}PageAccessibility", data[1]);
    }
}
