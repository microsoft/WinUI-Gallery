// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
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

        StringAssert.Matches(sdkDetails, new Regex(@"^Windows App SDK \d+\.\d+$"));
        StringAssert.StartsWith(runtimeDetails, runtimePrefix);
        Assert.IsTrue(Version.TryParse(runtimeDetails[runtimePrefix.Length..], out _));
    }
}
