using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;

namespace UITests.Tests
{
    [TestClass]
    public class AxeScanAll : TestBase
    {       
        public static readonly string jsonUri = "ms-appx:///../../../../../../WinUIGallery/DataModel/ControlInfoData.json";
        public static WindowsDriver<WindowsElement> Session => SessionManager.Session;

        public class ControlInfoData
        {
            public ObservableCollection<ControlInfoDataGroup> Groups { get; set; }
        }

        public class ControlInfoDataGroup
        {
            public string UniqueId { get; set; }
            public string Title { get; set; }
            public string Subtitle { get; set; }
            public string Description { get; set; }
            public string ImagePath { get; set; }
            public string ImageIconPath { get; set; }
            public string ApiNamespace { get; set; }
            public bool IsSpecialSection { get; set; }
            public string Folder { get; set; }
            public ObservableCollection<ControlInfoDataItem> Items { get; set; }
        }

        public class ControlInfoDataItem
        {
            public string UniqueId { get; set; }
            public string Title { get; set; }
            public string ApiNamespace { get; set; }
            public string Subtitle { get; set; }
            public string Description { get; set; }
            public string ImagePath { get; set; }
            public string ImageIconPath { get; set; }
            public string BadgeString { get; set; }
            public string Content { get; set; }
            public bool IsNew { get; set; }
            public bool IsUpdated { get; set; }
            public bool IsPreview { get; set; }
            public bool HideSourceCodeAndRelatedControls { get; set; }
            public ObservableCollection<ControlInfoDocLink> Docs { get; set; }
            public ObservableCollection<string> RelatedControls { get; set; }

            public bool IncludedInBuild { get; set; }
        }

        public class ControlInfoDocLink
        {
            public ControlInfoDocLink(string title, string uri)
            {
                this.Title = title;
                this.Uri = uri;
            }
            public string Title { get; set; }
            public string Uri { get; set; }
        }

        private static ControlInfoData controlInfoData;

        private string[] ExclusionList =
        {
            "WebView2" // 46668961: Web contents from WebView2 are throwing null BoundingRectangle errors.
        };

        [ClassInitialize]
        public static void ClassInitializeAsync(TestContext context)
        {
            ParseJson();
        }

        [TestMethod]
        [TestProperty("Description", "Scan all controls in the WinUIGallery for accessibility issues.")]
        public void ValidateAccessibilityWithAxe()
        {
            // We are using using the list of controls from ControlInfoData.json to get the Ids of each existing page.
            // Then we physically navigate to each page via NavigationView and scan for Axe issues. This also tests for run-time crashes.

            // Click through each control group in NavView and scan for Axe issues.
            foreach (var controlInfoDataGroup in controlInfoData.Groups)
            {
                var groupName = controlInfoDataGroup.UniqueId;
                var groupItem = Session.FindElementByAccessibilityId(groupName);
                groupItem.Click();

                AxeHelper.AssertNoAccessibilityErrors(groupName);

                // Click through each control in the group and scan for Axe issues.
                foreach (var controlInfoDataItem in controlInfoDataGroup.Items)
                {
                    var controlName = controlInfoDataItem.UniqueId;

                    // Skip controls that are in the exclusion list.
                    if (ExclusionList.Contains(controlName))
                    {
                        continue;
                    }

                    var controlItem = Session.FindElementByAccessibilityId(controlName);
                    controlItem.Click();

                    AxeHelper.AssertNoAccessibilityErrors(controlName);
                }
            }
        }
        
        public static void ParseJson()
        {;
            var jsonData = File.ReadAllText(jsonUri);

            controlInfoData = JsonSerializer.Deserialize<ControlInfoData>(jsonData);
        }

    }
}
