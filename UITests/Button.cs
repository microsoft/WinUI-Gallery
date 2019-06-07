//******************************************************************************
//
// Copyright (c) 2016 Microsoft Corporation. All rights reserved.
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

namespace UITests
{
    [TestClass]
    public class Button : Test_Base
    {
        private static WindowsElement buttonElement = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            var buttonTab = session.FindElementByName("Basic Input");
            buttonTab.Click();
            var button = session.FindElementByName("Button");
            button.Click();
            buttonElement = session.FindElementByAccessibilityId("Button1");
            Assert.IsNotNull(buttonElement);
            Thread.Sleep(3000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }

        [TestMethod]
        public void Button_Click()
        {
        
            var buttonEventOutput = session.FindElementByAccessibilityId("Control1Output");
            Assert.AreEqual(string.Empty, buttonEventOutput.Text);

            buttonElement.Click();

            Assert.AreEqual("You clicked: Button1", buttonEventOutput.Text);
        }

        [TestMethod]
        public void Displayed()
        {
            Assert.IsTrue(buttonElement.Displayed);
        }

        [TestMethod]
        public void Enabled()
        {
            var disableButtonCheckbox = session.FindElementByAccessibilityId("DisableButton1");
            Assert.IsTrue(buttonElement.Enabled);
            disableButtonCheckbox.Click();
            Assert.IsFalse(buttonElement.Enabled);
            disableButtonCheckbox.Click();
            Assert.IsTrue(buttonElement.Enabled);
        }

        [TestMethod]
        public void Name()
        {
            Assert.AreEqual("ControlType.Button", buttonElement.TagName);
        }

        [TestMethod]
        public void Size()
        {
            Assert.IsTrue(buttonElement.Size.Width > 0);
            Assert.IsTrue(buttonElement.Size.Height > 0);
        }

        [TestMethod]
        public void Text()
        {
            Assert.AreEqual("Button", buttonElement.Text);
        }
    }
}
