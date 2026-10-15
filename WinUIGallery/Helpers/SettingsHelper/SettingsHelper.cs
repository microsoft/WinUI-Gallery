using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;

namespace WinUIGallery.Helpers;

public partial class SettingsHelper : ObservableSettings
{
    private static readonly string[] PrivacySensitiveRegions =
    [
        "AUT",
        "BEL",
        "BGR",
        "BRA",
        "CAN",
        "HRV",
        "CYP",
        "CZE",
        "DNK",
        "EST",
        "FIN",
        "FRA",
        "DEU",
        "GRC",
        "HUN",
        "ISL",
        "IRL",
        "ITA",
        "KOR",
        "LVA",
        "LIE",
        "LTU",
        "LUX",
        "MLT",
        "NLD",
        "NOR",
        "POL",
        "PRT",
        "ROU",
        "SVK",
        "SVN",
        "ESP",
        "SWE",
        "CHE",
        "GBR",
    ];

    private static readonly SettingsHelper instance = new(SettingsProviderFactory.CreateProvider());
    private readonly string geographicRegionCode;

    public static SettingsHelper Current => instance;

    internal SettingsHelper(ISettingsProvider provider)
        : this(provider, new GeographicRegion().CodeThreeLetter)
    {
    }

    internal SettingsHelper(ISettingsProvider provider, string geographicRegionCode)
        : base(provider)
    {
        this.geographicRegionCode = geographicRegionCode;
    }

    public const int MaxRecentlyVisitedSamples = 7;

    public ElementTheme SelectedAppTheme
    {
        get => GetOrCreateDefault<ElementTheme>(ElementTheme.Default);
        set => Set(value);
    }

    public bool IsLeftMode
    {
        get => GetOrCreateDefault<bool>(true);
        set => Set(value);
    }

    public bool IsShowCopyLinkTeachingTip
    {
        get => GetOrCreateDefault<bool>(true);
        set => Set(value);
    }

    public List<string> RecentlyVisited
    {
        get => GetOrCreateDefault<List<string>>(new List<string>());
        private set => Set(value);
    }

    public List<string> Favorites
    {
        get => GetOrCreateDefault<List<string>>(new List<string>());
        private set => Set(value);
    }

    public bool IsFirstRun
    {
        get => GetOrCreateDefault<bool>(true);
        set => Set(value);
    }

    public bool IsTelemetryEnabled
    {
        get => GetOrCreateDefault<bool>(!IsPrivacySensitiveRegion);
        set => Set(value);
    }

    public bool IsTelemetryConsentDismissed
    {
        get => GetOrCreateDefault<bool>(false);
        set => Set(value);
    }

    internal bool IsPrivacySensitiveRegion => PrivacySensitiveRegions.Contains(
        geographicRegionCode,
        StringComparer.OrdinalIgnoreCase);

    internal bool IsTelemetryConsentRequired => IsPrivacySensitiveRegion && !IsTelemetryConsentDismissed;

    internal bool IsTelemetryAllowed => IsTelemetryEnabled && !IsTelemetryConsentRequired;

    public void UpdateFavorites(Action<List<string>> updater)
    {
        var list = Favorites;
        updater(list);
        Favorites = list;
        _ = JumpListHelper.UpdateJumpListAsync();
    }
    public void UpdateRecentlyVisited(Action<List<string>> updater)
    {
        var list = RecentlyVisited;
        updater(list);
        RecentlyVisited = list;
        _ = JumpListHelper.UpdateJumpListAsync();
    }
}
