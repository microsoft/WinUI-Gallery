// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class MediaPlayerElement : TestBase
{

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        // Doing this manually due to keyboard layout issues surrounding y and z
        var search = Session.FindElementByName("Search");
        search.Clear();
        Thread.Sleep(1_000);
        search.SendKeys("MediaPla");
        GetElementByName("MediaPlayerElement").Click();
    }

    [TestMethod]
    public void PlayMedia()
    {
        WindowsElement play = Session.FindElementByAccessibilityId("PlayPauseButton");
        Assert.IsNotNull(play);
        Assert.IsNotNull(Session.FindElementByAccessibilityId("svPanel"));

        // Play the video
        play.Click();
        Thread.Sleep(1000);

        // Pause the video
        play.Click();
    }
}
