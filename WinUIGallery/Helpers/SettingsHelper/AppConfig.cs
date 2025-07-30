using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;

namespace WinUIGallery.Helpers;

public class AppConfig
{
    internal static readonly string RootDirectoryPath = Path.Combine(GetAppDataFolderPath(), ProcessInfoHelper.ProductNameAndVersion);
    internal static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");
    private static string GetAppDataFolderPath()
    {
        // This can be replaced with Microsoft.Windows.Storage.ApplicationData.GetForUnPackaged(), once it is available in the WASDK.
        var unpackaged = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return NativeMethods.IsAppPackaged ? Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path : unpackaged;
    }

    /// <summary>
    /// Gets or sets the currently selected application theme.
    /// </summary>
    public ElementTheme SelectedAppTheme { get; set; } = ElementTheme.Default;

    /// <summary>
    /// Gets or sets a value indicating whether the navigation is in left mode.
    /// </summary>
    public bool IsLeftMode { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the teaching tip for copying links is shown.
    /// </summary>
    public bool IsShowCopyLinkTeachingTip { get; set; }

    /// <summary>
    /// Gets or sets the list of favorite items.
    /// </summary>
    public List<string> Favorites { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of recently visited items.
    /// </summary>
    public List<string> RecentlyVisited { get; set; } = new();

    /// <summary>
    /// Represents the maximum number of recently visited samples to retain.
    /// </summary>
    public int MaxRecentlyVisitedSamples { get; set; } = 7;

    /// <summary>
    /// Gets or sets the maximum number of favorite samples allowed.
    /// </summary>
    public int MaxFavoriteSamples { get; set; } = 0;
}