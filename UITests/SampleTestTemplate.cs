using System.Threading;
using OpenQA.Selenium.Appium.Windows;
namespace UITests
{
	[TestClass]
	public class SampleTestTemplate : TestBase
	{

		private static WindowsElement element1 = null;
		private static WindowsElement element2 = null;

		public static void ClassInitialize(TestContext context)
		{
			OpenControlPage("MyControlPage");
			Thread.Sleep(1000);
			element1 = Session.FindElementByAccessibilityId("Element Locator");
			Assert.IsNotNull(element1);
			element2 = Session.FindElementByAccessibilityId("Element Locator");
			Assert.IsNotNull(element2);
		}
	}
}

