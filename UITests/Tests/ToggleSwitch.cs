﻿//******************************************************************************
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

namespace UITests.Tests
{
	[TestClass]
    public class ToggleSwitch : TestBase
    {
        private static WindowsElement toggleSwitchElement = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
			OpenControlPage("ToggleSwitch");
			toggleSwitchElement = Session.FindElementByAccessibilityId("ToggleSwitch2");
            Assert.IsNotNull(toggleSwitchElement);
        }

		[TestMethod]
        public void Click()
        {
            var originalState = toggleSwitchElement.Selected;
            toggleSwitchElement.Click();
            Assert.AreNotEqual(originalState, toggleSwitchElement.Selected);

            Thread.Sleep(1000);
            toggleSwitchElement.Click();
            Assert.AreEqual(originalState, toggleSwitchElement.Selected);
        }

        [TestMethod]
        public void Displayed()
        {
            Assert.IsTrue(toggleSwitchElement.Displayed);
        }

        [TestMethod]
        public void Enabled()
        {
            Assert.IsTrue(toggleSwitchElement.Enabled);
        }

        [TestMethod]
        public void Name()
        {
            Assert.AreEqual("ControlType.Button", toggleSwitchElement.TagName);
        }

        [TestMethod]
        public void Selected()
        {
            var originalState = toggleSwitchElement.Selected;
            toggleSwitchElement.Click();
            Assert.AreNotEqual(originalState, toggleSwitchElement.Selected);

            Thread.Sleep(1000);
            toggleSwitchElement.Click();
            Assert.AreEqual(originalState, toggleSwitchElement.Selected);
        }

        [TestMethod]
        public void Size()
        {
            Assert.IsTrue(toggleSwitchElement.Size.Width > 0);
            Assert.IsTrue(toggleSwitchElement.Size.Height > 0);
        }

        [TestMethod]
        public void Text()
        {
            Assert.AreEqual("Toggle work Working", toggleSwitchElement.Text);
        }
    }
}
