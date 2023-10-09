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
            // Expand tree view.
            var page = Session.FindElementByAccessibilityId(sectionName);
            page.Click();

            // Click into page and check for accessibility issues.
            var row = Session.FindElementByAccessibilityId(pageName);
            row.Click();

            AxeHelper.AssertNoAccessibilityErrors();
        }

        public static string GetCustomDynamicDataDisplayName(MethodInfo methodInfo, object[] data)
        {
            return string.Format("Validate{0}PageAccessibility", data[1]);
        }
    }
}
