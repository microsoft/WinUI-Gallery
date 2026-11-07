// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinUIGallery.ControlPages;

// Raises PropertyChanged so bound cells update and, with TableViewSource.IsLiveShaping,
// the sort, filter, grouping (by Trend), and hierarchy (by ParentId) follow each new value.
public sealed partial class TableViewLiveItem(int id, int? parentId, string name, int value) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public int Id { get; } = id;
    public int? ParentId { get; } = parentId;
    public string Name { get; } = name;
    public int Value { get; private set; } = Math.Clamp(value, 0, 100);
    public int Change { get; private set; }
    public string ChangeText => Change switch
    {
        > 0 => $"▲ {Change}",
        < 0 => $"▼ {-Change}",
        _ => "–",
    };
    public string Trend => Change switch
    {
        > 0 => "Rising",
        < 0 => "Falling",
        _ => "Steady",
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Trend)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutomationName)));
    }

    public static ObservableCollection<TableViewLiveItem> CreateItems() =>
    [
        new(1, null, "Alpha", 72),
        new(2, 1, "Alpha 1", 45),
        new(3, 1, "Alpha 2", 88),
        new(4, null, "Bravo", 31),
        new(5, 4, "Bravo 1", 60),
        new(6, 4, "Bravo 2", 12),
        new(7, null, "Charlie", 54),
        new(8, 7, "Charlie 1", 27),
        new(9, 7, "Charlie 2", 66),
    ];
}
