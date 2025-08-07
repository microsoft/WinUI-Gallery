using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace WinUIGallery.Helpers;

public partial class Settings : ObservableSettings
{
    private static readonly Settings instance = new(SettingsProviderFactory.CreateProvider());
    public static Settings Current => instance;

    private Settings(ISettingsProvider provider)
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
        set => Set(value);
    }

    public List<string> Favorites
    {
        get => GetOrCreateDefault<List<string>>(new List<string>());
        set => Set(value);
    }
}
