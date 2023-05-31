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
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace UITests
{
	public class TestBase
	{
		public static WindowsDriver<WindowsElement> Session => SessionManager.Session;

		public static void OpenControlPage(string name)
		{
			var search = Session.FindElementByName("Search");
			search.Clear();
			Thread.Sleep(1_000);
			search.SendKeys(name);
			Thread.Sleep(1_000);
			Session.FindElementByName(name).Click();
			Thread.Sleep(5_000);
		}

		public static void TypeText(string text)
		{
			var actions = new Actions(Session);
			actions.SendKeys(text).Perform();
		}
	}
}
