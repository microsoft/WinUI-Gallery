﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace WinUIGallery.UITests;

public class TestBase
{
    public static WindowsDriver<WindowsElement> Session => SessionManager.Session;

    public static void OpenControlPage(string name)
    {
        var search = Session.FindElementByName("Search controls and samples...");
        search.Clear();

        search.SendKeys(name);
        GetElementByName(name).Click();

        Assert.IsNotNull(WaitForPageHeader(name), "Failed to find matching page header for page: " + name);
    }

    public static WindowsElement GetElementByName(string name)
    {
        for (int i = 0; i < 100; i++)
        {
            Thread.Sleep(50);
            var element = Session.FindElementByName(name);
            if (element != null)
            {
                return element;
            }
        }
        return null;
    }

    private static WindowsElement WaitForPageHeader(string name)
    {
        for (int i = 0; i < 100; i++)
        {
            var header = Session.FindElementByAccessibilityId("PageHeader");
            if (header != null && header.Text == name)
            {
                return header;
            }
            Thread.Sleep(50);
        }
        return null;
    }

    public static void TypeText(string text)
    {
        var actions = new Actions(Session);
        actions.SendKeys(text).Perform();
    }
}
