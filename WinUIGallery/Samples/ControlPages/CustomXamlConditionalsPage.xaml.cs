// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class CustomXamlConditionalsPage : Page
{
    // Snapshot of what FeatureFlagCondition.Evaluate returned for each
    // flag the first time the parser saw it. Used by the interactive
    // panel below to contrast the cached parse-time result against the
    // live dictionary value.
    private readonly Dictionary<string, bool> cachedAtParseTime = new();

    public CustomXamlConditionalsPage()
    {
        // Feature flags must be set before InitializeComponent so the XAML
        // parser sees the desired values when it evaluates the conditions.
        // FeatureFlagCondition.FeatureFlags["NewExperience"] = true;

        // Snapshot the values right before the parser runs. These are the
        // results that get cached by the parser for the lifetime of the
        // process.
        foreach (string flag in new[] { "NewExperience", "LegacyMode", "BetaFeature" })
        {
            cachedAtParseTime[flag] = FeatureFlagCondition.FeatureFlags[flag];
        }

        this.InitializeComponent();

        NewExperienceToggle.IsOn = FeatureFlagCondition.FeatureFlags["NewExperience"];
        LegacyModeToggle.IsOn = FeatureFlagCondition.FeatureFlags["LegacyMode"];
        BetaFeatureToggle.IsOn = FeatureFlagCondition.FeatureFlags["BetaFeature"];

        NewExperienceCached.Text = cachedAtParseTime["NewExperience"].ToString();
        LegacyModeCached.Text = cachedAtParseTime["LegacyMode"].ToString();
        BetaFeatureCached.Text = cachedAtParseTime["BetaFeature"].ToString();
    }

    private void OnFlagToggled(object sender, RoutedEventArgs e)
    {
        ToggleSwitch toggle = (ToggleSwitch)sender;
        string flag = (string)toggle.Tag;
        FeatureFlagCondition.FeatureFlags[flag] = toggle.IsOn;
    }
}

