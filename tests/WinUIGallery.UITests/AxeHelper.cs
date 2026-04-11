// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Axe.Windows.Automation;
using Axe.Windows.Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WinUIGallery.UITests;

public class AxeHelper
{
    public static IScanner AccessibilityScanner;

    // Rules excluded globally due to known WinUI framework issues that affect all pages.
    // These are not fixable from app code and produce false positives.
    private static readonly HashSet<RuleId> GloballyExcludedRules =
    [
        RuleId.NameIsInformative,
        RuleId.NameExcludesControlType,
        RuleId.NameExcludesLocalizedControlType,
        RuleId.SiblingUniqueAndFocusable,
    ];

    internal static void InitializeAxe()
    {
        var processes = Process.GetProcessesByName("WinUIGallery");
        Assert.IsTrue(processes.Length > 0);

        var config = Config.Builder.ForProcessId(processes[0].Id).Build();

        AccessibilityScanner = ScannerFactory.CreateScanner(config);
    }

    /// <summary>
    /// Scans the current page for accessibility errors and asserts that none are found.
    /// </summary>
    /// <param name="pageRuleExclusions">
    /// Optional set of rule IDs to exclude for this specific page. Use this for known
    /// framework-level issues that only affect certain pages (e.g., BoundingRectangle
    /// rules for pages with off-screen or collapsed elements).
    /// </param>
    public static void AssertNoAccessibilityErrors(IEnumerable<RuleId> pageRuleExclusions = null)
    {
        HashSet<RuleId> excludedRules = new(GloballyExcludedRules);

        if (pageRuleExclusions != null)
        {
            excludedRules.UnionWith(pageRuleExclusions);
        }

        var testResult = AccessibilityScanner.Scan(null).WindowScanOutputs
            .SelectMany(output => output.Errors)
            .Where(rule => !excludedRules.Contains(rule.Rule.ID));

        if (testResult.Any())
        {
            var mappedResult = testResult.Select(result =>
            {
                string controlType = result.Element.Properties.TryGetValue("ControlType", out string ct) ? ct : "Unknown";
                string name = result.Element.Properties.TryGetValue("Name", out string n) ? n : "(no name)";
                string automationId = result.Element.Properties.TryGetValue("AutomationId", out string aid) ? aid : "(no id)";
                return $"[{result.Rule.ID}] Element '{controlType}' (Name='{name}', AutomationId='{automationId}') " +
                       $"violated rule '{result.Rule.Description}'.";
            });

            Assert.Fail("Failed with the following accessibility errors \r\n" + string.Join("\r\n", mappedResult));
        }
    }
}
