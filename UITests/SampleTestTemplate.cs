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
    public class SampleTestTemplate : Test_Base
    {
        
    private static WindowsElement element1 = null;
    private static WindowsElement element2 = null;

        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
            var buttonTab = session.FindElementByName("Basic Input");
            buttonTab.Click();
            var button = session.FindElementByName("RadioButton");
            button.Click();
            Thread.Sleep(1000);
            element1 = session.FindElementByAccessibilityId("Element Locator");
            Assert.IsNotNull(element1);
            element2 = session.FindElementByAccessibilityId("Element Locator");
            Assert.IsNotNull(element2);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }

        [TestMethod]
        public void Test1()
        {
          //  Assert.AreEqual("Option 1", element1.Text);
          //  Assert.AreEqual("Option 2", element2.Text);
        }

        [TestMethod]
        public void Test2()
        {
           // Assert.AreEqual("Option 1", element1.Text);
           // Assert.AreEqual("Option 2", element2.Text);
        }
    }




}

