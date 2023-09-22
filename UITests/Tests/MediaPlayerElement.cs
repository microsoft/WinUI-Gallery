//******************************************************************************
//
// Copyright (c) 2023 Microsoft Corporation. All rights reserved.
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
    public class MediaPlayerElement : TestBase
    {

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
			// Doing this manually due to keyboard layout issues surrounding y and z
			var search = Session.FindElementByName("Search");
            search.Clear();
            Thread.Sleep(1_000);
            search.SendKeys("MediaPla");
			GetElementByName("MediaPlayerElement").Click();
		}

		[TestMethod]
        public void PlayMedia()
        {
            WindowsElement play = Session.FindElementByAccessibilityId("PlayPauseButton");
            Assert.IsNotNull(play);
            Assert.IsNotNull(Session.FindElementByAccessibilityId("svPanel"));

            // Play the video
            play.Click();
            Thread.Sleep(1000);
            
            // Pause the video
            play.Click();
        }
    }
}
