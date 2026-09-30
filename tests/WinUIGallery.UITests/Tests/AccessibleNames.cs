// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Axe.Windows.Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using System.Linq;
using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class AccessibleNames : TestBase
{
    private static readonly RuleId[] NameRoleRules =
    [
        RuleId.NameExcludesControlType,
        RuleId.NameExcludesLocalizedControlType,
    ];

    [TestMethod]
    [DataRow("Style")]
    [DataRow("ContentDialog")]
    [DataRow("Implicit Transitions")]
    [DataRow("MediaPlayerElement")]
    [DataRow("ProgressBar")]
    [DataRow("Slider")]
    [DataRow("ToolTip")]
    public void ControlNamesDoNotRepeatTheirControlType(string pageName)
    {
        OpenControlPage(pageName);

        AxeHelper.AssertNoAccessibilityErrors(includedRules: NameRoleRules);
    }

    [TestMethod]
    public void OffsetToolTipNameDoesNotRepeatItsControlType()
    {
        OpenControlPage("ToolTip");

        ShowToolTip(
            Session.FindElementByName("TextBlock with an offset ToolTip."),
            "Offset information.");

        AxeHelper.AssertNoAccessibilityErrors(includedRules: NameRoleRules);
    }

    [TestMethod]
    public void NonOccludingToolTipNameDoesNotRepeatItsControlType()
    {
        OpenControlPage("ToolTip");

        ShowToolTip(
            Session.FindElementByAccessibilityId("textBoxToPlace"),
            "Non-occluding information.");

        AxeHelper.AssertNoAccessibilityErrors(includedRules: NameRoleRules);
    }

    private static void ShowToolTip(WindowsElement element, string expectedName)
    {
        var actions = new Actions(Session);
        actions.MoveToElement(element).Perform();

        for (int attempt = 0; attempt < 20; attempt++)
        {
            if (Session.FindElementsByName(expectedName).Any())
            {
                return;
            }

            Thread.Sleep(100);
        }

        Assert.Fail($"ToolTip \"{expectedName}\" did not open.");
    }
}
