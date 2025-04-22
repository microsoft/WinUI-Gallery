// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class TextBlock : TestBase
{
    private static WindowsElement textBlockElement1 = null;
    private static WindowsElement textBlockElement2 = null;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("TextBlock");
        textBlockElement1 = Session.FindElementByName("I am a TextBlock.");
        Assert.IsNotNull(textBlockElement1);
        textBlockElement2 = Session.FindElementByName("I am a styled TextBlock.");
        Assert.IsNotNull(textBlockElement2);
    }

    [TestMethod]
    public void Displayed()
    {
        Assert.IsTrue(textBlockElement1.Displayed);
        Assert.IsTrue(textBlockElement2.Displayed);
    }

    [TestMethod]
    public void Enabled()
    {
        Assert.IsTrue(textBlockElement1.Enabled);
        Assert.IsTrue(textBlockElement1.Enabled);
    }

    [TestMethod]
    public void Location()
    {
        Assert.IsTrue(textBlockElement2.Location.X >= textBlockElement1.Location.X);
        Assert.IsTrue(textBlockElement2.Location.Y >= textBlockElement1.Location.Y);
    }

    [TestMethod]
    public void LocationInView()
    {
        Assert.IsTrue(textBlockElement2.LocationOnScreenOnceScrolledIntoView.X >= textBlockElement1.LocationOnScreenOnceScrolledIntoView.X);
        Assert.IsTrue(textBlockElement2.LocationOnScreenOnceScrolledIntoView.Y >= textBlockElement1.LocationOnScreenOnceScrolledIntoView.Y);
    }

    [TestMethod]
    public void Name()
    {
        Assert.AreEqual("ControlType.Text", textBlockElement1.TagName);
        Assert.AreEqual("ControlType.Text", textBlockElement2.TagName);
    }

    [TestMethod]
    public void Size()
    {
        Assert.IsTrue(textBlockElement1.Size.Width > 0);
        Assert.IsTrue(textBlockElement1.Size.Height > 0);
        Assert.IsTrue(textBlockElement2.Size.Width >= textBlockElement1.Size.Width);
        Assert.IsTrue(textBlockElement2.Size.Height >= textBlockElement1.Size.Height);
    }

    [TestMethod]
    public void Text()
    {
        Assert.AreEqual("I am a TextBlock.", textBlockElement1.Text);
        Assert.AreEqual("I am a styled TextBlock.", textBlockElement2.Text);
    }
}
