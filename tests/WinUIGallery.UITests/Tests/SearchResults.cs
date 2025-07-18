// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class SearchResults : TestBase
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
    }

    [TestMethod]
    [DataRow("a")]  // "a" should return results for all groups.
    [TestProperty("Description", "Validate the accessibility of the search results page.")]
    public void ValidateSearchResultsPageAccessibility(string searchText)
    {
        var search = Session.FindElementByName("Search controls and samples...");
        search.Clear();

        search.SendKeys(searchText);
        search.SendKeys(Keys.Enter);

        Thread.Sleep(100);

        var resultsNavView = Session.FindElementByAccessibilityId("resultsNavView");
        var resultItems = resultsNavView.FindElements(By.ClassName("Microsoft.UI.Xaml.Controls.NavigationViewItem"));

        foreach (var menuItem in resultItems)
        {
            Thread.Sleep(1000);
            menuItem.Click();
            AxeHelper.AssertNoAccessibilityErrors();
        }
    }
}
