// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class DatePicker : TestBase
{
    private static WindowsElement datePickerElement1 = null;
    private static WindowsElement datePickerElement2 = null;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("DatePicker");
        datePickerElement1 = Session.FindElementByName("Pick a date");
        Assert.IsNotNull(datePickerElement1);
        datePickerElement2 = Session.FindElementByAccessibilityId("Control2");
        Assert.IsNotNull(datePickerElement2);
    }

    [TestMethod]
    public void Click()
    {
        // Click datePickerElement1 to show the picker and simply dismiss it
        datePickerElement1.Click();
        var datePickerFlyout = Session.FindElementByAccessibilityId("FlyoutButton");
        Assert.IsNotNull(datePickerFlyout);
        Assert.IsTrue(datePickerFlyout.Displayed);
        Session.FindElementByAccessibilityId("DatePickerFlyoutPresenter").FindElementByAccessibilityId("DismissButton").Click();
        Thread.Sleep(1000);

        // Click datePickerElement1 to show the picker and set the year to 2000
        datePickerElement1.Click();
        datePickerFlyout = Session.FindElementByAccessibilityId("DatePickerFlyoutPresenter");
        Assert.IsNotNull(datePickerFlyout);
        var yearLoopingSelector = datePickerFlyout.FindElementByAccessibilityId("YearLoopingSelector");
        Assert.IsNotNull(yearLoopingSelector);
        var currentYear = yearLoopingSelector.Text;
        yearLoopingSelector.FindElementByName("2000").Click();
        Thread.Sleep(1000);
        Assert.AreNotEqual(currentYear, yearLoopingSelector.Text);
        datePickerFlyout.FindElementByAccessibilityId("AcceptButton").Click();
    }

    [TestMethod]
    public void Displayed()
    {
        Assert.IsTrue(datePickerElement1.Displayed);
        Assert.IsTrue(datePickerElement2.Displayed);
    }

    [TestMethod]
    public void Enabled()
    {
        Assert.IsTrue(datePickerElement1.Enabled);
        Assert.IsTrue(datePickerElement2.Enabled);
    }

    [TestMethod]
    public void Location()
    {
        Assert.IsTrue(datePickerElement2.Location.X >= datePickerElement1.Location.X);
        Assert.IsTrue(datePickerElement2.Location.Y >= datePickerElement1.Location.Y);
    }

    [TestMethod]
    public void LocationInView()
    {
        Assert.IsTrue(datePickerElement2.LocationOnScreenOnceScrolledIntoView.X >= datePickerElement1.LocationOnScreenOnceScrolledIntoView.X);
        Assert.IsTrue(datePickerElement2.LocationOnScreenOnceScrolledIntoView.Y >= datePickerElement1.LocationOnScreenOnceScrolledIntoView.Y);
    }

    [TestMethod]
    public void Name()
    {
        Assert.AreEqual("ControlType.Group", datePickerElement1.TagName);
        Assert.AreEqual("ControlType.Group", datePickerElement2.TagName);
    }

    [TestMethod]
    public void Size()
    {
        Assert.IsTrue(datePickerElement1.Size.Width > 0);
        Assert.IsTrue(datePickerElement1.Size.Height > 0);
        Assert.IsTrue(datePickerElement2.Size.Width >= datePickerElement1.Size.Width);
        Assert.IsTrue(datePickerElement2.Size.Height <= datePickerElement1.Size.Height);
    }
}
