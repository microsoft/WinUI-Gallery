using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace WinUIGallery.Helpers;

public partial class ApplicationDataSettingsProvider : ISettingsProvider
{
    private readonly ApplicationDataContainer container;

    public ApplicationDataSettingsProvider(ApplicationDataContainer container)
    {
        this.container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public bool Contains(string key) => container.Values.ContainsKey(key);

    public object Get(string key) => container.Values.TryGetValue(key, out var value) ? value : null;

    public void Set(string key, object value) => container.Values[key] = value;

    public T Get<T>(string key)
    {
        if (!container.Values.TryGetValue(key, out var value))
            return default;

        if (value is T t)
            return t;

        if (value is string str && !IsSimpleType(typeof(T)))
        {
            var typeInfo = SettingsJsonContext.Default.GetTypeInfo(typeof(T));
            return (T)JsonSerializer.Deserialize(str, typeInfo);
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public void Set<T>(string key, T value)
    {
        object storedValue = IsSimpleType(typeof(T))
            ? value
            : JsonSerializer.Serialize(value, SettingsJsonContext.Default.GetTypeInfo(typeof(T)));

        container.Values[key] = storedValue;
    }

    private static readonly HashSet<Type> ExtraSimpleTypes = new()
    {
        typeof(string),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid),
        typeof(Windows.Foundation.Point),
        typeof(Windows.Foundation.Size),
        typeof(Windows.Foundation.Rect)
    };

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || ExtraSimpleTypes.Contains(type);
    }
}
