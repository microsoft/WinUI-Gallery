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

using Axe.Windows.Automation;
using System.Diagnostics;
using System.Linq;

namespace UITests
{
	public class AxeHelper
	{
		public static IScanner AccessibilityScanner;

		internal static void InitializeAxe()
		{
			var processes = Process.GetProcessesByName("WinUIGallery.DesktopWap");
			Assert.IsTrue(processes.Length > 0);

			var config = Config.Builder.ForProcessId(processes[0].Id).Build();

			AccessibilityScanner = ScannerFactory.CreateScanner(config);
		}

		public static void AssertNoAccessibilityErrors()
		{
			var testResult = AccessibilityScanner.Scan(null).WindowScanOutputs.SelectMany(output => output.Errors);
			if (testResult.Count() != 0)
			{
				var mappedResult = testResult.Select(result => "Element " + result.Element.Properties["ControlType"] + " violated rule '" + result.Rule.Description + "'.");
				Assert.Fail("Failed with the following accessibility errors \r\n" + string.Join("\r\n", mappedResult));
			}
		}
	}
}