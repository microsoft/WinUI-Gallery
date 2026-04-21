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
        // FeatureFlagCondition.FeatureFlags["NewExperience"] = true;

        this.InitializeComponent();
    }
}
