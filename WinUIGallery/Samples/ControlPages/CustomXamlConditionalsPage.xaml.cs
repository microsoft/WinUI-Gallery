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
        // the lifetime of the process.
        FeatureFlagCondition.FeatureFlags["NewExperience"] = true;
        FeatureFlagCondition.FeatureFlags["LegacyMode"] = false;

        this.InitializeComponent();

        // Reflect the current flag values in each per-sample picker.
        string activeFlag = FeatureFlagCondition.FeatureFlags["NewExperience"]
            ? "NewExperience"
            : "LegacyMode";
        ElementsModeComboBox.SelectedItem = activeFlag;
        AttributesModeComboBox.SelectedItem = activeFlag;
        SettersModeComboBox.SelectedItem = activeFlag;
    }

    private void OnModeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender).SelectedItem is not string selected)
        {
            return;
        }

        // Update the flags so that the NEXT time this page (or any page that
        // uses the same conditions with the same arguments) is parsed, it
        // sees the new values. The conditional markup already on screen was
        // resolved at parse time and will not change.
        FeatureFlagCondition.FeatureFlags["NewExperience"] = selected == "NewExperience";
        FeatureFlagCondition.FeatureFlags["LegacyMode"] = selected == "LegacyMode";
    }
}


