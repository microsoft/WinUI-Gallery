//******************************************************************************
//
// Copyright (c) 2017 Microsoft Corporation. All rights reserved.
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
    public class ProgressBar : Test_Base
    {
        private static WindowsElement progressBarElement = null;
        private static WindowsElement clickAndHoldButton = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            //NavigateTo("Progress controls", "ProgressBar");
            var buttonTab = session.FindElementByName("Status and Info");
            buttonTab.Click();
            var button = session.FindElementByName("ProgressBar");
            button.Click();
            progressBarElement = session.FindElementByAccessibilityId("ProgressBar2");
            Assert.IsNotNull(progressBarElement);
            // Numberbox is a spinner, thus "Increase" is the button we need
            clickAndHoldButton = session.FindElementByName("Increase");
            Assert.IsNotNull(clickAndHoldButton);
            Thread.Sleep(3000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }

        [TestMethod]
        public void Displayed()
        {
            Assert.IsTrue(progressBarElement.Displayed);
        }

        [TestMethod]
        public void Enabled()
        {
            Assert.IsTrue(progressBarElement.Enabled);
        }

        [TestMethod]
        public void Location()
        {
            Assert.IsTrue(clickAndHoldButton.Location.X >= progressBarElement.Location.X);
            Assert.IsTrue(clickAndHoldButton.Location.Y <= progressBarElement.Location.Y);
        }

        [TestMethod]
        public void LocationInView()
        {
            Assert.IsTrue(clickAndHoldButton.LocationOnScreenOnceScrolledIntoView.X >= progressBarElement.LocationOnScreenOnceScrolledIntoView.X);
            Assert.IsTrue(clickAndHoldButton.LocationOnScreenOnceScrolledIntoView.Y <= progressBarElement.LocationOnScreenOnceScrolledIntoView.Y);
        }

        [TestMethod]
        public void Name()
        {
            Assert.AreEqual("ControlType.ProgressBar", progressBarElement.TagName);
        }

        [TestMethod]
        public void Size()
        {
            Assert.IsTrue(progressBarElement.Size.Width > 0);
            Assert.IsTrue(progressBarElement.Size.Height > 0);
        }

        [TestMethod]
        public void Text()
        {
            var originalValue = int.Parse(progressBarElement.Text);
            Assert.IsTrue(originalValue >= 0);
            clickAndHoldButton.Click();
            Assert.AreEqual(originalValue + 1, int.Parse(progressBarElement.Text));
            clickAndHoldButton.Click();
            Assert.AreEqual(originalValue + 2, int.Parse(progressBarElement.Text));
        }
    }
}
