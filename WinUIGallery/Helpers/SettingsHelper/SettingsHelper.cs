using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace WinUIGallery.Helpers;

public partial class SettingsHelper : ObservableSettings
{
    private static readonly SettingsHelper instance = new(SettingsProviderFactory.CreateProvider());
    public static SettingsHelper Current => instance;

    private SettingsHelper(ISettingsProvider provider)
        : base(provider)
    {
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

    public void UpdateFavorites(Action<List<string>> updater)
    {
        var list = Favorites;
        updater(list);
        Favorites = list;
    }
    public void UpdateRecentlyVisited(Action<List<string>> updater)
    {
        var list = RecentlyVisited;
        updater(list);
        RecentlyVisited = list;
    }
}
