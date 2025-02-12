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
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

using System.Threading;

namespace WinUIGallery.UITests.Tests;

[TestClass]
public class TextBox : TestBase
{
    private static WindowsElement textBoxElement1 = null;
    private static WindowsElement textBoxElement2 = null;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        OpenControlPage("TextBox");
        textBoxElement1 = Session.FindElementByName("simple TextBox");
        textBoxElement2 = Session.FindElementByName("Enter your name:");
        Assert.IsNotNull(textBoxElement1);
        Assert.IsNotNull(textBoxElement2);
    }

    [TestMethod]
    public void Clear()
    {
        textBoxElement1.Clear();
        Assert.AreEqual(string.Empty, textBoxElement1.Text);
        textBoxElement1.SendKeys("F");
        Assert.AreEqual("F", textBoxElement1.Text);
        textBoxElement1.Clear();
        Assert.AreEqual(string.Empty, textBoxElement1.Text);
    }

    [TestMethod]
    public void Click()
    {
        // Click textBoxElement1 to set focus and arbitrarily type
        textBoxElement1.Clear();
        Assert.AreEqual(string.Empty, textBoxElement1.Text);
        textBoxElement1.Click();
        Thread.Sleep(1_000);
        TypeText("1");
        Assert.AreEqual("1", textBoxElement1.Text);

        // Click textBoxElement2 to set focus and arbitrarily type
        textBoxElement2.Clear();
        Assert.AreEqual(string.Empty, textBoxElement2.Text);
        textBoxElement2.Click();
        Thread.Sleep(1_000);
        TypeText("1");
        Assert.AreEqual("1", textBoxElement2.Text);
    }

    [TestMethod]
    public void Displayed()
    {
        Assert.IsTrue(textBoxElement1.Displayed);
        Assert.IsTrue(textBoxElement2.Displayed);
    }

    [TestMethod]
    public void Enabled()
    {
        Assert.IsTrue(textBoxElement1.Enabled);
        Assert.IsTrue(textBoxElement2.Enabled);
    }

    [TestMethod]
    public void Location()
    {
        Assert.IsTrue(textBoxElement2.Location.X >= textBoxElement1.Location.X);
        Assert.IsTrue(textBoxElement2.Location.Y >= textBoxElement1.Location.Y);
    }

    [TestMethod]
    public void LocationInView()
    {
        Assert.IsTrue(textBoxElement2.LocationOnScreenOnceScrolledIntoView.X >= textBoxElement1.LocationOnScreenOnceScrolledIntoView.X);
        Assert.IsTrue(textBoxElement2.LocationOnScreenOnceScrolledIntoView.Y >= textBoxElement1.LocationOnScreenOnceScrolledIntoView.Y);
    }

    [TestMethod]
    public void Name()
    {
        Assert.AreEqual("ControlType.Edit", textBoxElement1.TagName);
        Assert.AreEqual("ControlType.Edit", textBoxElement2.TagName);
    }

    [TestMethod]
    public void SendKeys()
    {
        textBoxElement1.Clear();
        Assert.AreEqual(string.Empty, textBoxElement1.Text);
        textBoxElement1.SendKeys("A");
        Assert.AreEqual("A", textBoxElement1.Text);

        // Use Ctrl + A to select all text and backspace to clear the box
        textBoxElement1.SendKeys(Keys.Control + "a" + Keys.Control + Keys.Backspace);
        Assert.AreEqual(string.Empty, textBoxElement1.Text);

        textBoxElement2.Clear();
        Assert.AreEqual(string.Empty, textBoxElement2.Text);
        textBoxElement2.SendKeys("E");
        Assert.AreEqual("E", textBoxElement2.Text);
    }

    [TestMethod]
    public void Text()
    {
        textBoxElement1.Clear();
        Assert.AreEqual(string.Empty, textBoxElement1.Text);
        textBoxElement1.SendKeys("A");
        Assert.AreEqual("A", textBoxElement1.Text);
    }
}
