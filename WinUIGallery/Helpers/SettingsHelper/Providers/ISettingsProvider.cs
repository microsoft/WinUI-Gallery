// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace WinUIGallery.Helpers;

public interface ISettingsProvider
{
    bool Contains(string key);
    object? Get(string key);
    void Set(string key, object value);

    T? Get<T>(string key);
    void Set<T>(string key, T value);
}
