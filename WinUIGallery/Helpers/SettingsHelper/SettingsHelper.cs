using Microsoft.UI.Xaml;
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

    public string ScratchPadXaml
    {
        get => GetOrCreateDefault<string>(string.Empty);
        set => Set(value);
    }
}
