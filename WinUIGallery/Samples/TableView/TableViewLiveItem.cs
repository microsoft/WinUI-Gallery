// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinUIGallery.ControlPages;

// Raises PropertyChanged so bound cells update and, with TableViewSource.IsLiveShaping,
// the sort and filter follow each new value.
public sealed partial class TableViewLiveItem(string name, int value) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; } = name;
    public int Value { get; private set; } = Math.Clamp(value, 0, 100);
    public int Change { get; private set; }
    public string ChangeText => Change switch
    {
        > 0 => $"▲ {Change}",
        < 0 => $"▼ {-Change}",
        _ => "–",
    };
    public string AutomationName => $"{Name}, value {Value}, change {Change:+0;-0;0}";

    public void Update(int delta)
    {
        int newValue = Math.Clamp(Value + delta, 0, 100);
        Change = newValue - Value;
        Value = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Change)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeText)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutomationName)));
    }

    public static ObservableCollection<TableViewLiveItem> CreateItems() =>
    [
        new("Alpha", 72),
        new("Bravo", 45),
        new("Charlie", 88),
        new("Delta", 31),
        new("Echo", 60),
        new("Foxtrot", 12),
        new("Golf", 54),
        new("Hotel", 27),
    ];
}
