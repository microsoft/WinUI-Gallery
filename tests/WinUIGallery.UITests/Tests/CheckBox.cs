﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class CheckBox : TestBase
{
    private static WindowsElement checkBoxElement1 = null;
    private static WindowsElement checkBoxElement2 = null;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("CheckBox");
        checkBoxElement1 = Session.FindElementByName("Two-state");
        checkBoxElement2 = Session.FindElementByName("Three-state");
        Assert.IsNotNull(checkBoxElement2);
    }

    [TestMethod]
    public void Click()
    {
        var checkBoxEventOutput = Session.FindElementByAccessibilityId("Control2Output");
        Assert.AreEqual(string.Empty, checkBoxEventOutput.Text);

        checkBoxElement2.Click();
        Assert.AreEqual("CheckBox is checked.", checkBoxEventOutput.Text);

        checkBoxElement2.Click();
        Assert.AreEqual("CheckBox state is indeterminate.", checkBoxEventOutput.Text);

        checkBoxElement2.Click();
        Assert.AreEqual("CheckBox is unchecked.", checkBoxEventOutput.Text);
    }

    [TestMethod]
    public void Displayed()
    {
        Assert.IsTrue(checkBoxElement1.Displayed);
        Assert.IsTrue(checkBoxElement2.Displayed);
    }

    [TestMethod]
    public void Enabled()
    {
        Assert.IsTrue(checkBoxElement1.Enabled);
        Assert.IsTrue(checkBoxElement2.Enabled);
    }

    [TestMethod]
    public void Location()
    {
        Assert.IsTrue(checkBoxElement2.Location.X >= checkBoxElement1.Location.X);
        Assert.IsTrue(checkBoxElement2.Location.Y >= checkBoxElement1.Location.Y);
    }

    [TestMethod]
    public void LocationInView()
    {
        Assert.IsTrue(checkBoxElement2.LocationOnScreenOnceScrolledIntoView.X >= checkBoxElement1.LocationOnScreenOnceScrolledIntoView.X);
        Assert.IsTrue(checkBoxElement2.LocationOnScreenOnceScrolledIntoView.Y >= checkBoxElement1.LocationOnScreenOnceScrolledIntoView.Y);
    }

    [TestMethod]
    public void Name()
    {
        Assert.AreEqual("ControlType.CheckBox", checkBoxElement1.TagName);
        Assert.AreEqual("ControlType.CheckBox", checkBoxElement2.TagName);
    }

    [TestMethod]
    public void Selected()
    {
        var originalState = checkBoxElement1.Selected;
        checkBoxElement1.Click();
        Assert.AreNotEqual(originalState, checkBoxElement1.Selected);
        checkBoxElement1.Click();
        Assert.AreEqual(originalState, checkBoxElement1.Selected);
    }

    [TestMethod]
    public void Size()
    {
        Assert.IsTrue(checkBoxElement1.Size.Width > 0);
        Assert.IsTrue(checkBoxElement1.Size.Height > 0);
        Assert.IsTrue(checkBoxElement1.Size.Width <= checkBoxElement2.Size.Width);
        Assert.AreEqual(checkBoxElement1.Size.Height, checkBoxElement2.Size.Height);
    }
}
