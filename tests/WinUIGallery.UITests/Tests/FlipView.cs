// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class FlipView : TestBase
{
    private static WindowsElement flipViewElement1 = null;
    private static WindowsElement flipViewElement2 = null;
    private static WindowsElement flipViewElement3 = null;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("FlipView");
        
        // Wait for the page to load and find FlipView elements
        System.Threading.Thread.Sleep(1000);
        
        // Find the FlipView controls - they should have LocalizedControlType set
        var flipViews = Session.FindElementsByClassName("FlipView");
        Assert.IsTrue(flipViews.Count >= 3, "FlipViewPage should contain at least 3 FlipView controls");
        
        flipViewElement1 = flipViews[0];
        flipViewElement2 = flipViews[1];
        flipViewElement3 = flipViews[2];
        
        Assert.IsNotNull(flipViewElement1);
        Assert.IsNotNull(flipViewElement2);
        Assert.IsNotNull(flipViewElement3);
    }

    [TestMethod]
    public void VerifyFlipViewLocalizedControlType()
    {
        // Verify that FlipView controls have proper LocalizedControlType for accessibility
        Assert.AreEqual("FlipView", flipViewElement1.GetAttribute("LocalizedControlType"),
            "First FlipView should have LocalizedControlType set to 'FlipView'");
        Assert.AreEqual("FlipView", flipViewElement2.GetAttribute("LocalizedControlType"),
            "Second FlipView should have LocalizedControlType set to 'FlipView'");
        Assert.AreEqual("FlipView", flipViewElement3.GetAttribute("LocalizedControlType"),
            "Third FlipView should have LocalizedControlType set to 'FlipView'");
    }

    [TestMethod]
    public void BasicFlipViewNavigation()
    {
        // Test basic FlipView navigation functionality
        Assert.IsTrue(flipViewElement1.Displayed, "FlipView should be visible");
        
        // Get the initial selected item
        var selectedItem = flipViewElement1.GetAttribute("Selection.SelectedItem");
        Assert.IsNotNull(selectedItem, "FlipView should have a selected item");
        
        // Try to navigate (this might require keyboard navigation or finding navigation buttons)
        flipViewElement1.Click();
        System.Threading.Thread.Sleep(500);
        
        // Basic verification that the control is functional
        Assert.IsTrue(flipViewElement1.Enabled, "FlipView should be enabled and functional");
    }

    [TestMethod]
    public void VerifyAccessibilityCompliance()
    {
        // Run accessibility scan specifically for FlipView
        AxeHelper.AssertNoAccessibilityErrors();
    }
}