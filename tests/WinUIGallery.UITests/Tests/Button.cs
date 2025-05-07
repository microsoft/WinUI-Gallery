// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class Button : TestBase
{
    private static WindowsElement buttonElement = null;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("Button");
        buttonElement = Session.FindElementByAccessibilityId("Button1");
        Assert.IsNotNull(buttonElement);
    }

    [TestMethod]
    public void Button_Click()
    {

        var buttonEventOutput = Session.FindElementByAccessibilityId("Control1Output");
        Assert.AreEqual(string.Empty, buttonEventOutput.Text);

        buttonElement.Click();

        Assert.AreEqual("You clicked: Button1", buttonEventOutput.Text);
    }

    [TestMethod]
    public void Displayed()
    {
        Assert.IsTrue(buttonElement.Displayed);
    }

    [TestMethod]
    public void Enabled()
    {
        var disableButtonCheckbox = Session.FindElementByAccessibilityId("DisableButton1");
        Assert.IsTrue(buttonElement.Enabled);
        disableButtonCheckbox.Click();
        Assert.IsFalse(buttonElement.Enabled);
        disableButtonCheckbox.Click();
        Assert.IsTrue(buttonElement.Enabled);
    }

    [TestMethod]
    public void Name()
    {
        Assert.AreEqual("ControlType.Button", buttonElement.TagName);
    }

    [TestMethod]
    public void Size()
    {
        Assert.IsTrue(buttonElement.Size.Width > 0);
        Assert.IsTrue(buttonElement.Size.Height > 0);
    }

    [TestMethod]
    public void Text()
    {
        Assert.AreEqual("Standard XAML", buttonElement.Text);
    }
}