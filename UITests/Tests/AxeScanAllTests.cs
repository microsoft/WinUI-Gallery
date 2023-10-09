using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestPlatform;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

namespace UITests.Tests
{
    [TestClass]
    public class AxeScanAll : TestBase
    {       
        public static readonly string xmlUri = "WinUIGalleryTestData.xml";
        public static new WindowsDriver<WindowsElement> Session => SessionManager.Session;

        public static string[] ExclusionList =
        {
            "WebView2", // 46668961: Web contents from WebView2 are throwing null BoundingRectangle errors.
            "Icons" // https://github.com/CommunityToolkit/Windows/issues/240 External toolkit SettingsExpander does not pass Axe testing
        };

        private static IEnumerable<object[]> TestData()
        {
            var testCases = new List<object[]>();

            // Load the XML file
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlUri);

            // Select all tables in the XML
            var tables = xmlDoc.SelectNodes("//Table");
            foreach (XmlNode tableNode in tables)
            {
                var sectionName = tableNode.Attributes["Id"].Value;

                // Select all row names within the current table
                var rows = tableNode.SelectNodes("Row");

                foreach (XmlNode rowNode in rows)
                {
                    var pageName = rowNode.Attributes["Name"].Value;

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
                // Click into page and check for accessibility issues.
                var page = Session.FindElementByAccessibilityId(pageName);
                page.Click();

                AxeHelper.AssertNoAccessibilityErrors();
            }
            catch
            {
                // If element is not found, expand tree view as it is nested.
                var section = Session.FindElementByAccessibilityId(sectionName);
                section.Click();

                // Click into page and check for accessibility issues.
                var page = Session.FindElementByAccessibilityId(pageName);
                page.Click();

                AxeHelper.AssertNoAccessibilityErrors();
            }
        }

        public static string GetCustomDynamicDataDisplayName(MethodInfo methodInfo, object[] data)
        {
            return string.Format("Validate{0}PageAccessibility", data[1]);
        }
    }
}
