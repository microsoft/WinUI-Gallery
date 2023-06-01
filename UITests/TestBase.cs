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
using System.Linq;
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
			var headers = Session.FindElementsByName(name);

			search.SendKeys(name);
			GetElementByName(name).Click();

			WaitForPageHeader(name);
		}

		public static WindowsElement GetElementByName(string name)
		{
			for (int i = 0; i < 100; i++)
			{
				Thread.Sleep(50);
				var element = Session.FindElementByName(name);
				if (element != null)
				{
					return element;
				}
			}
			return null;
		}
		private static void WaitForPageHeader(string name)
		{
			for (int i = 0; i < 100; i++)
			{
				var header = Session.FindElementsByName(name).Where(x => x.GetProperty("AutomationId") == "PageHeader");
				if (header != null)
				{
					break;
				}
				Thread.Sleep(50);
			}
		}

		public static void TypeText(string text)
		{
			var actions = new Actions(Session);
			actions.SendKeys(text).Perform();
		}
	}
}
