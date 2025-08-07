using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUIGallery.Helpers;

public partial class ObservableSettings : INotifyPropertyChanged
{
    private readonly ISettingsProvider provider;

    public ObservableSettings(ISettingsProvider provider)
    {
        this.provider = provider;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected bool Set<T>(T value, [CallerMemberName] string propertyName = null)
    {
        if (provider.Contains(propertyName))
        {
            var currentValue = provider.Get<T>(propertyName);
            if (Equals(currentValue, value))
                return false;
        }

        provider.Set(propertyName, value);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    protected T Get<T>([CallerMemberName] string propertyName = null)
    {
        return provider.Get<T>(propertyName);
    }

    protected T GetOrCreateDefault<T>(T defaultValue, [CallerMemberName] string propertyName = null)
    {
        if (!provider.Contains(propertyName))
            Set(defaultValue, propertyName);

        return Get<T>(propertyName);
    }
}
