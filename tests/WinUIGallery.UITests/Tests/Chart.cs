// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public sealed class Chart : TestBase
{
    [TestMethod]
    public void ChartExposesAxesSeriesAndDataPoints()
    {
        OpenControlPage("Chart");
        WindowsElement chart = Session.FindElementByAccessibilityId("ChartBasicLine");

        Assert.AreEqual("Monthly profit line chart", chart.GetAttribute("Name"));
        Assert.AreEqual("Monthly profit increases from 18 in January to 52 in June.", chart.GetAttribute("HelpText"));

        string[] names =
        [
            "Month", "Profit", "2025",
            "January, Profit=18", "February, Profit=26", "March, Profit=33",
            "April, Profit=39", "May, Profit=47", "June, Profit=52",
        ];
        foreach (string name in names)
        {
            Assert.IsTrue(chart.FindElementsByName(name).Count > 0, $"Chart automation is missing '{name}'.");
        }
    }
}