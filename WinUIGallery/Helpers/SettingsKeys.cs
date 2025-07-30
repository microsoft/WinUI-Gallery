// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace WinUIGallery.Helpers;

/// <summary>
/// Provides constant keys used for accessing application settings.
/// </summary>
public static partial class SettingsKeys
{
    /// <summary>
    /// The key used to store or retrieve the selected application theme.
    /// </summary>
    public const string SelectedAppTheme = "SelectedAppTheme";

    /// <summary>
    /// Represents the key used to determine whether the navigation is in "left mode."
    /// </summary>
    public const string IsLeftMode = "NavigationIsOnLeftMode";

    /// <summary>
    /// Key for the list of recently visited Pages.
    /// </summary>
    public const string RecentlyVisited = "RecentlyVisited";

    /// <summary>
    /// Key for the list of favorited Pages.
    /// </summary>
    public const string Favorites = "Favorites";

    /// <summary>
    /// Represents the key used to identify the "Show Copy Link" teaching tip of the protocol activation clipboard.
    /// </summary>
    public const string ShowCopyLinkTeachingTip = "ShowCopyLinkTeachingTip";
}
