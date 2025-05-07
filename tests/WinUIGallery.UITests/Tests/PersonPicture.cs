// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class PersonPicture : TestBase
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("PersonPicture");
    }

    [TestMethod]
    public void SwitchOptions()
    {
        GetElementByName("Profile Image").Click();
        GetElementByName("Display Name").Click();
        GetElementByName("Initials").Click();
    }
}
