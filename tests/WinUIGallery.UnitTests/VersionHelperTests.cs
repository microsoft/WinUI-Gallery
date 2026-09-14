// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAppSDK;
using System;
using WinUIGallery.Helpers;

namespace WinUIGallery.UnitTests;

[TestClass]
public class VersionHelperTests
{
    [TestMethod]
    public void VersionDetailsAreAvailable()
    {
        string sdkDetails = VersionHelper.WinAppSdkDetails;
        string runtimeDetails = VersionHelper.WinAppSdkRuntimeDetails;
        string runtimePrefix = $"{sdkDetails}, Windows App Runtime ";

        string expectedSdkDetails = $"Windows App SDK {Release.Major}.{Release.Minor}";
        if (!string.IsNullOrEmpty(Release.Channel))
        {
            expectedSdkDetails += $" ({Release.Channel})";
        }

        Assert.AreEqual(expectedSdkDetails, sdkDetails);
        StringAssert.StartsWith(runtimeDetails, runtimePrefix);
        Assert.IsTrue(Version.TryParse(runtimeDetails[runtimePrefix.Length..], out _));
    }
}
