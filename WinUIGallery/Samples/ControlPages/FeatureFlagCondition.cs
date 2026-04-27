// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace WinUIGallery.ControlPages;

// Sample implementation of IXamlCondition (introduced in Windows App SDK 2.0).
// A custom XAML condition is evaluated at XAML parse time and lets you
// conditionally include markup based on application-specific state such as
// feature flags, device capabilities, or configuration.
//
// Important: Evaluate is called by the XAML parser. The result for a given
// (condition, argument) pair is cached for the lifetime of the process, so
// changing the underlying flag at runtime will NOT re-evaluate already-parsed
// XAML. Set the flags before the page is loaded.
public sealed partial class FeatureFlagCondition : DependencyObject, IXamlCondition
{
    public static IDictionary<string, bool> FeatureFlags { get; } = new Dictionary<string, bool>
    {
        ["NewExperience"] = true,
        ["LegacyMode"] = false,
    };

    public bool Evaluate(string argument)
    {
        return FeatureFlags.TryGetValue(argument, out bool enabled) && enabled;
    }
}
