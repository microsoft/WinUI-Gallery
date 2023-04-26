using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UITests
{
	[TestClass]
	public class SessionManager
	{
		private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
		private static string[] WinUIGalleryAppIDs = new string[]{
			"Microsoft.WinUI3ControlsGallery.Debug_s9y1p3hwd5qda!App",
			"Microsoft.WinUI3ControlsGallery_s9y1p3hwd5qda!App"
		};

		private static uint appIdIndex = 0;

		private static WindowsDriver<WindowsElement> _session;
		public static WindowsDriver<WindowsElement> Session
		{
			get
			{
				if (_session is null)
				{
					Setup(null);
				}
				return _session;
			}
		}

		[AssemblyInitialize]
		public static void Setup(TestContext _)
		{
			if (_session is null)
			{

				int timeoutCount = 50;

				tryInitializeSession();
				if (_session is null)
				{
					// WinAppDriver is probably not running, so lets start it!
					if (File.Exists(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"))
					{
						Process.Start(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe");
					}
					else if (File.Exists(@"C:\Program Files\Windows Application Driver\WinAppDriver.exe"))
					{
						Process.Start(@"C:\Program Files\Windows Application Driver\WinAppDriver.exe");
					}
					else
					{
						throw new Exception("Unable to start WinAppDriver since no suitable location was found.");
					}

					Thread.Sleep(10000);
					tryInitializeSession();
				}

				while (_session is null && timeoutCount < 1000 * 4)
				{
					tryInitializeSession();
					Thread.Sleep(timeoutCount);
					timeoutCount *= 2;
				}

				Thread.Sleep(3000);
				Assert.IsNotNull(_session);
				Assert.IsNotNull(_session.SessionId);

				// Dismiss the disclaimer window that may pop up on the very first application launch
				// If the disclaimer is not found, this throws an exception, so lets catch that
				try
				{
					_session.FindElementByName("Disclaimer").FindElementByName("Accept").Click();
				}
				catch (OpenQA.Selenium.WebDriverException) { }

				// Wait if something is still animating in the visual tree
				_session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
				_session.Manage().Window.Maximize();
			}
		}

		[AssemblyCleanup()]
		public static void TestRunTearDown()
		{
			TearDown();
		}

		public static void TearDown()
		{
			if (_session is not null)
			{
				_session.CloseApp();
				_session.Quit();
				_session = null;
			}
		}

		private static void tryInitializeSession()
		{
			AppiumOptions appiumOptions = new AppiumOptions();
			appiumOptions.AddAdditionalCapability("app", WinUIGalleryAppIDs[appIdIndex]);
			appiumOptions.AddAdditionalCapability("deviceName", "WindowsPC");
			try
			{
				_session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appiumOptions);
			}
			catch (OpenQA.Selenium.WebDriverException exc)
			{
				// Use next app ID since the current one was failing
				if (exc.Message.Contains("Package was not found"))
				{
					appIdIndex++;
				}
				else
				{
					Console.WriteLine("Failed to update start driver, got exception:" + exc.Message);
				}
			}
		}
	}
}
