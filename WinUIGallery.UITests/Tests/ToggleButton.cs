//******************************************************************************
//
// Copyright (c) 2024 Microsoft Corporation. All rights reserved.
//
// This code is licensed under the MIT License (MIT).
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//******************************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class ToggleButton : TestBase
{
    private static WindowsElement toggleButtonElement = null;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("ToggleButton");
        toggleButtonElement = Session.FindElementByAccessibilityId("Toggle1");
        Assert.IsNotNull(toggleButtonElement);
    }

    [TestMethod]
    public void Click()
    {
        var buttonEventOutput = Session.FindElementByAccessibilityId("Control1Output");
        Assert.AreEqual("Off", buttonEventOutput.Text);

        toggleButtonElement.Click();
        Assert.AreEqual("On", buttonEventOutput.Text);
        toggleButtonElement.Click();
        Assert.AreEqual("Off", buttonEventOutput.Text);
    }

    [TestMethod]
    public void Displayed()
    {
        Assert.IsTrue(toggleButtonElement.Displayed);
    }

    [TestMethod]
    public void Enabled()
    {
        var disableButtonCheckbox = Session.FindElementByAccessibilityId("DisableToggle1");
        Assert.IsTrue(toggleButtonElement.Enabled);

        disableButtonCheckbox.Click();
        Assert.IsFalse(toggleButtonElement.Enabled);

        disableButtonCheckbox.Click();
        Assert.IsTrue(toggleButtonElement.Enabled);
    }

    [TestMethod]
    public void Name()
    {
        Assert.AreEqual("ControlType.Button", toggleButtonElement.TagName);
    }

    [TestMethod]
    public void Selected()
    {
        toggleButtonElement.Click();
        Assert.IsTrue(toggleButtonElement.Selected);

        toggleButtonElement.Click();
        Assert.IsFalse(toggleButtonElement.Selected);
    }

    [TestMethod]
    public void Size()
    {
        Assert.IsTrue(toggleButtonElement.Size.Width > 0);
        Assert.IsTrue(toggleButtonElement.Size.Height > 0);
    }

    [TestMethod]
    public void Text()
    {
        Assert.AreEqual("ToggleButton", toggleButtonElement.Text);
    }
}
