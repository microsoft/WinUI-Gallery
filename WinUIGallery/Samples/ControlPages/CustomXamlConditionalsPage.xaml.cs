// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class CustomXamlConditionalsPage : Page
{
    public CustomXamlConditionalsPage()
    {
        // Feature flags must be set before InitializeComponent so the XAML
        // parser sees the desired values when it evaluates the conditions.
        // The result for each (condition, argument) pair is then cached for
        // the lifetime of the process. The defaults declared on
        // FeatureFlagCondition.FeatureFlags are used here, but the same
        // dictionary can be updated to flip the active variant, e.g.:
        //
        //     FeatureFlagCondition.FeatureFlags["NewExperience"] = false;
        //     FeatureFlagCondition.FeatureFlags["LegacyMode"] = true;

        this.InitializeComponent();
    }
}



