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
        var search = Session.FindElementByName("Search");
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
