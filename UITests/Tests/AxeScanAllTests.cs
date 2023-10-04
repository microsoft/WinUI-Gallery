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

namespace UITests.Tests
{
    [TestClass]
    public class AxeScanAll : TestBase
    {       
        public static readonly string xmlUri = "WinUIGalleryTestData.xml";
        public static new WindowsDriver<WindowsElement> Session => SessionManager.Session;

        private string[] ExclusionList =
        {
            "WebView2" // 46668961: Web contents from WebView2 are throwing null BoundingRectangle errors.
        };

        private static IEnumerable<object[]> TestData()
        {
            var testCases = new List<object[]>();

            // Load the XML file
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlUri);

            var nodes = xmlDoc.SelectNodes("//Row");
            foreach (XmlNode node in nodes)
            {
                var rowName = node.Attributes["Name"].Value;
                testCases.Add(new object[] { rowName });
            }

            return testCases;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        [TestMethod]
        [DynamicData(nameof(TestData), DynamicDataSourceType.Method)]
        [TestProperty("Description", "Scan all controls in the WinUIGallery for accessibility issues.")]
        public void ValidateAccessibilityWithAxe(string name)
        {

            var groupItem = Session.FindElementByAccessibilityId(name);
            groupItem.Click();

            AxeHelper.AssertNoAccessibilityErrors();
        }
    }
}
