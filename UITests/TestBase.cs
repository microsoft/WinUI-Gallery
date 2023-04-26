using Microsoft.VisualBasic;
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
		}

		public static void TypeText(string text)
		{
			var actions = new Actions(Session);
			actions.SendKeys(text).Perform();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			Session.FindElementByName("Home");
		}
	}
}
