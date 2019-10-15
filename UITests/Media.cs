using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace UITests
{
    [TestClass]
    public class Media : Test_Base
    {

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            var buttonTab = session.FindElementByName("Media");
            buttonTab.Click();
            var button = session.FindElementByName("MediaPlayerElement");
            button.Click();
            var mediaElements = session.FindElementsByClassName("MediaPlayerElement");
  
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }

        [TestMethod]
        public void PlayMedia()
        {
            Thread.Sleep(1000);
            WindowsElement play = session.FindElementByAccessibilityId("PlayPauseButton");
            Assert.IsNotNull(play);
            Assert.IsNotNull(session.FindElementByAccessibilityId("svPanel"));
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

    [TestClass]
    public class PersonPicture : Test_Base
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            //NavigateTo("Selection and picker controls", "RadioButton");

            var buttonTab = session.FindElementByName("Media");
            buttonTab.Click();
            var button = session.FindElementByName("PersonPicture");
            button.Click();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }

    }
    
}
