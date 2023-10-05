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
                var tableId = tableNode.Attributes["Id"].Value;

                // Select all row names within the current table
                var rows = tableNode.SelectNodes("Row");
                var rowNames = new List<string>();

                foreach (XmlNode rowNode in rows)
                {
                    var rowName = rowNode.Attributes["Name"].Value;
                    rowNames.Add(rowName);
                }

                testCases.Add(new object[] { tableId, rowNames });
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
        public void ValidatePageAccessibilityWithAxe(string tableId, List<string> rowNames)
        {
            // Expand tree view and check for page accessibility.
            var page = Session.FindElementByAccessibilityId(tableId);
            page.Click();

            AxeHelper.AssertNoAccessibilityErrors();

            // Click into each page and check for accessibility issues.
            foreach (var rowName in rowNames)
            {
                var row = Session.FindElementByAccessibilityId(rowName);
                row.Click();

                AxeHelper.AssertNoAccessibilityErrors();
            }
        }

        public static string GetCustomDynamicDataDisplayName(MethodInfo methodInfo, object[] data)
        {
            return string.Format("Validate{0}PagesAccessibilityWithAxe", data[0]);
        }
    }
}
