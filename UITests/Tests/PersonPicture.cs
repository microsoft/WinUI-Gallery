using System.Threading;

namespace UITests.Tests
{
	[TestClass]
	public class PersonPicture : TestBase
	{
		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			OpenControlPage("PersonPicture");
		}

		[TestMethod]
		public void ValidateAccessibilityWithAxe()
		{
			AxeHelper.AssertNoAccessibilityErrors();
		}

		[TestMethod]
		public void SwitchOptions()
		{
			Session.FindElementByName("Profile Image").Click();
			Thread.Sleep(1_000);
			Session.FindElementByName("Display Name").Click();
			Thread.Sleep(1_000); 
			Session.FindElementByName("Initials").Click();
			Thread.Sleep(1_000);
		}
	}
}
