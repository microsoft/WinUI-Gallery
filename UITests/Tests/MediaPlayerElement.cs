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
			Session.FindElementByName("MediaPlayerElement").Click();
		}

        [TestMethod]
        public void PlayMedia()
        {
            Thread.Sleep(1000);
            WindowsElement play = Session.FindElementByAccessibilityId("PlayPauseButton");
            Assert.IsNotNull(play);
            Assert.IsNotNull(Session.FindElementByAccessibilityId("svPanel"));
            Thread.Sleep(1000);

            try
            {
                play.Click();
            }
            catch
            {

            }
            Thread.Sleep(1000);
            play.Click();
        }
    }
}
