using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class AxeScanAll : TestBase
{       
    public static readonly string jsonUri = "ControlInfoData.json";
    public static new WindowsDriver<WindowsElement> Session => SessionManager.Session;

    public static string[] ExclusionList =
    {
        "WebView2", // 46668961: Web contents from WebView2 are throwing null BoundingRectangle errors.
        "Icons", // https://github.com/CommunityToolkit/Windows/issues/240 External toolkit SettingsExpander does not pass Axe testing
        // https://github.com/microsoft/axe-windows/issues/662
        // AxeWindowsAutomationException: Failed to get the root element(s) of the specified process error for following pages:
        "PersonPicture",
        "MapControl",
        "TabView"
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
                testCases.Add(new object[] { sectionName, pageName });
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
        try
        {
            Logger.LogMessage($"Opening page \"{pageName}\".");

            // Click into page and check for accessibility issues.
            var page = Session.FindElementByAccessibilityId(pageName);
            page.Click();

            AxeHelper.AssertNoAccessibilityErrors();
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

                    AxeHelper.AssertNoAccessibilityErrors();
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
