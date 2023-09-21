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

        [ClassInitialize]
        public static void ClassInitializeAsync(TestContext context)
        {
            LoadText();
        }

        [TestMethod]
        public void ValidateAccessibilityWithAxe()
        {
            foreach (var controlInfoDataItems in controlInfoData.Groups)
            {
                var navViewItems = Session.FindElementByAccessibilityId(controlInfoDataItems.UniqueId);
                navViewItems.Click();

                AxeHelper.AssertNoAccessibilityErrors();
            }
        }
        
        public static void LoadText()
        {
            var uri = "ms-appx:///../../../../../../WinUIGallery/DataModel/ControlInfoData.json";
            var jsonData = File.ReadAllText(uri);

            controlInfoData = JsonSerializer.Deserialize<ControlInfoData>(jsonData);
        }

    }
}
