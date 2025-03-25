//******************************************************************************
//
// Copyright (c) 2024 Microsoft Corporation. All rights reserved.
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
using Axe.Windows.Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;

namespace WinUIGallery.UITests;

public class AxeHelper
{
    public static IScanner AccessibilityScanner;

    internal static void InitializeAxe()
    {
        var processes = Process.GetProcessesByName("WinUIGallery");
        Assert.IsTrue(processes.Length > 0);

        var config = Config.Builder.ForProcessId(processes[0].Id).Build();

        AccessibilityScanner = ScannerFactory.CreateScanner(config);
    }

    public static void AssertNoAccessibilityErrors()
    {
        // Bug 1474: Disabling Rules NameReasonableLength and BoundingRectangleNotNull temporarily
        var testResult = AccessibilityScanner.Scan(null).WindowScanOutputs.SelectMany(output => output.Errors)
                .Where(rule => rule.Rule.ID != RuleId.NameIsInformative)
                .Where(rule => rule.Rule.ID != RuleId.NameExcludesControlType)
                .Where(rule => rule.Rule.ID != RuleId.NameExcludesLocalizedControlType)
                .Where(rule => rule.Rule.ID != RuleId.SiblingUniqueAndFocusable)
                .Where(rule => rule.Rule.ID != RuleId.NameReasonableLength)
                .Where(rule => rule.Rule.ID != RuleId.BoundingRectangleNotNull)
                .Where(rule => rule.Rule.ID != RuleId.BoundingRectangleNotNullListViewXAML)
                .Where(rule => rule.Rule.ID != RuleId.BoundingRectangleNotNullTextBlockXAML)
                .Where(rule => rule.Rule.ID != RuleId.NameNotNull)
                .Where(rule => rule.Rule.ID != RuleId.ChromiumComponentsShouldUseWebScanner);

        if (testResult.Any())
        {
            var mappedResult = testResult.Select(result =>
            "Element " + result.Element.Properties["ControlType"] + " violated rule '" + result.Rule.Description + "'.");
            Assert.Fail("Failed with the following accessibility errors \r\n" + string.Join("\r\n", mappedResult));
        }
    }
}
