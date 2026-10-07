// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewSampleItem : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private string _name;
    private string _category;
    private string? _nameError;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public int Id { get; }
    public int Value { get; }
    public string Status => Id % 2 == 0 ? "Paused" : "Active";
    public int Rating => (Id - 1) % 5 + 1;
    public string AutomationName => $"{Name}, ID {Id}, Category {Category}, Value {Value}";
    public bool HasErrors => _nameError is not null;

    public string Name
    {
        get => _name;
        set
        {
            string normalizedValue = value ?? string.Empty;
            if (SetProperty(ref _name, normalizedValue))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutomationName)));
                string? error = string.IsNullOrWhiteSpace(normalizedValue) ? "Name is required." : null;
                if (_nameError != error)
                {
                    _nameError = error;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasErrors)));
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));
                }
            }
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            if (SetProperty(ref _category, value ?? string.Empty))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutomationName)));
            }
        }
    }

    public TableViewSampleItem(int id, string name, string category, int value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        _name = name;
        _category = category;
        Value = value;
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if ((string.IsNullOrEmpty(propertyName) || propertyName == nameof(Name)) && _nameError is not null)
        {
            yield return _nameError;
        }
    }

    public static ObservableCollection<TableViewSampleItem> CreateItems(int count = 5)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        string[] categories = ["Alpha", "Beta", "Gamma"];
        int[] values = [25, 80, 50, 100, 10];
        ObservableCollection<TableViewSampleItem> items = [];

        for (int index = 0; index < count; index++)
        {
            items.Add(new TableViewSampleItem(
                index + 1,
                $"Item {index + 1}",
                categories[index % categories.Length],
                values[index % values.Length]));
        }

        return items;
    }

    private bool SetProperty(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}