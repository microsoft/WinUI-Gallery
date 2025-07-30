// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;

namespace WinUIGallery.Helpers;

/// <summary>
/// Provides utility methods for initializing and saving application settings.
/// </summary>
/// <remarks>The <see cref="SettingsHelper"/> class is responsible for managing the application's configuration
/// settings. It ensures that the necessary directories and configuration files are created and provides methods to
/// initialize and persist the configuration.</remarks>
public static partial class SettingsHelper
{
    public static void Init()
    {
        if (!Directory.Exists(AppConfig.RootDirectoryPath))
        {
            Directory.CreateDirectory(AppConfig.RootDirectoryPath);
        }

        if (File.Exists(AppConfig.AppConfigPath))
        {
            try
            {
                var json = File.ReadAllText(AppConfig.AppConfigPath);
                Config = (string.IsNullOrEmpty(json) ? new AppConfig() : System.Text.Json.JsonSerializer.Deserialize<AppConfig>(json, AppConfigJsonContext.Default.AppConfig)) ?? new AppConfig();
            }
            catch
            {
                Config = new AppConfig();
            }
        }
        else
        {
            Config = new AppConfig();
        }
    }

    public static void Save()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(Config, AppConfigJsonContext.Default.AppConfig);
        File.WriteAllText(AppConfig.AppConfigPath, json);
    }

    public static AppConfig Config { get; set; }
}
