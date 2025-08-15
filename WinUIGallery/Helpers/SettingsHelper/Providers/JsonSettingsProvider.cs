using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WinUIGallery.Helpers;

public partial class JsonSettingsProvider : ISettingsProvider
{
    private readonly string filePath;
    private Dictionary<string, JsonElement> values = new();

    public JsonSettingsProvider(string filePath)
    {
        this.filePath = filePath;
        Load();
    }

    public bool Contains(string key) => values.ContainsKey(key);

    public object Get(string key) => values.TryGetValue(key, out var value) ? value : null;

    public void Set(string key, object value)
    {
        var json = JsonSerializer.SerializeToElement(value, SettingsJsonContext.Default.GetTypeInfo(value.GetType()));
        values[key] = json;
        Save();
    }

    public T Get<T>(string key)
    {
        if (!values.TryGetValue(key, out var jsonElement))
            return default;

        try
        {
            return (T)jsonElement.Deserialize(SettingsJsonContext.Default.GetTypeInfo(typeof(T)));
        }
        catch (Exception)
        {
            HandleCorruptedKey(key);
            return default;
        }
    }


    public void Set<T>(string key, T value)
    {
        var json = JsonSerializer.SerializeToElement(value, SettingsJsonContext.Default.GetTypeInfo(typeof(T)));
        values[key] = json;
        Save();
    }

    private void Load()
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            var json = File.ReadAllText(filePath);
            values = JsonSerializer.Deserialize(
                json,
                SettingsJsonContext.Default.DictionaryStringJsonElement
            ) ?? new();
        }
        catch (Exception)
        {
            HandleCorruptedFile();
        }
    }
    private void HandleCorruptedFile()
    {
        try
        {
            File.Delete(filePath);
        }
        catch { }

        values = new();
    }

    private void HandleCorruptedKey(string key)
    {
        values.Remove(key);
        Save();
    }


    private void Save()
    {
        var json = JsonSerializer.Serialize(
            values,
            SettingsJsonContext.Default.DictionaryStringJsonElement
        );
        File.WriteAllText(filePath, json);
    }
}
