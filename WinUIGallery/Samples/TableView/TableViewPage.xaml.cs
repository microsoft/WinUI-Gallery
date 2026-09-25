// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewPage : Page, INotifyPropertyChanged
{
    private InventoryRecord _selectedInventoryItem;
    private StaffingAllocation? _editingStaffingAllocation;
    private string _inventorySortStatus = "Rows are in source order.";
    private bool _isValidationMessageOpen;
    private string _validationMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<InventoryRecord> Inventory { get; } =
    [
        new("AST-1042", "Handheld barcode scanner", "Available", "\uE73E", "Seattle", "A-14-03", 42, 12, "Sep 18, 2026", "No action required."),
        new("AST-1048", "Rugged tablet", "Low stock", "\uE814", "Seattle", "B-02-11", 7, 10, "Sep 20, 2026", "Replenish three units from the Portland warehouse."),
        new("AST-1091", "Thermal label printer", "In service", "\uE90F", "Portland", "C-08-01", 18, 6, "Sep 12, 2026", "Maintenance is scheduled for October 4."),
        new("AST-1103", "RFID reader", "Available", "\uE73E", "Austin", "D-01-07", 25, 8, "Sep 22, 2026", "No action required."),
        new("AST-1120", "Mobile workstation", "Transfer", "\uE8AB", "Austin", "D-04-02", 4, 5, "Sep 17, 2026", "Confirm the incoming transfer from Seattle."),
        new("AST-1137", "Industrial scale", "Inspection", "\uE9D9", "Chicago", "E-11-04", 11, 4, "Sep 21, 2026", "Complete the annual safety inspection."),
        new("AST-1164", "Packing station display", "Available", "\uE73E", "Chicago", "F-03-09", 31, 10, "Sep 19, 2026", "No action required."),
        new("AST-1195", "Forklift telemetry unit", "Low stock", "\uE814", "Portland", "G-07-06", 3, 6, "Sep 16, 2026", "Create a replenishment purchase order."),
    ];

    public ObservableCollection<StaffingAllocation> StaffingRows { get; } =
    [
        new("E-0142", "Amina Yusuf", "Program manager", 80, 148),
        new("E-0188", "Leo Martins", "Software engineer", 100, 192),
        new("E-0214", "Priya Shah", "Data analyst", 65, 121),
        new("E-0277", "Marek Nowak", "UX designer", 50, 96),
        new("E-0319", "Sofia Rossi", "Finance partner", 25, 62),
    ];

    public ObservableCollection<PerformanceMetric> PerformanceRows { get; } = [];

    public InventoryRecord SelectedInventoryItem
    {
        get => _selectedInventoryItem;
        private set => SetProperty(ref _selectedInventoryItem, value);
    }

    public bool IsValidationMessageOpen
    {
        get => _isValidationMessageOpen;
        private set => SetProperty(ref _isValidationMessageOpen, value);
    }

    public string InventorySortStatus
    {
        get => _inventorySortStatus;
        private set => SetProperty(ref _inventorySortStatus, value);
    }

    public string ValidationMessage
    {
        get => _validationMessage;
        private set => SetProperty(ref _validationMessage, value);
    }

    public TableViewPage()
    {
        _selectedInventoryItem = Inventory[0];
        InitializeComponent();
        PopulatePerformanceRows();
        Loaded += TableViewPage_Loaded;
    }

    private void TableViewPage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= TableViewPage_Loaded;
        InventoryTable.Select(0);
    }

    private void InventoryTable_SelectionChanged(TableView sender, SelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is InventoryRecord selectedItem)
        {
            SelectedInventoryItem = selectedItem;
        }
    }

    private void InventoryTable_Sorting(TableView sender, TableViewSortingEventArgs args)
    {
        string columnName = args.Column?.Header?.ToString() ?? "all columns";
        InventorySortStatus = args.Direction == SortDirection.None
            ? $"Clearing the sort for {columnName}."
            : $"Sorting {columnName} {args.Direction.ToString().ToLowerInvariant()}.";
    }

    private void InventoryTable_Sorted(TableView sender, TableViewSortedEventArgs args)
    {
        if (args.Direction == SortDirection.None)
        {
            InventorySortStatus = "Rows are in source order.";
            return;
        }

        string columnName = args.Column?.Header?.ToString() ?? "the selected column";
        InventorySortStatus = $"Sorted by {columnName}, {args.Direction.ToString().ToLowerInvariant()}.";
    }

    private void StaffingTable_CellEditEnding(TableView sender, TableViewCellEditEndingEventArgs args)
    {
        if (args.EditAction == TableViewEditAction.Cancel)
        {
            ClearValidationMessage();
            QueueStaffingEditCleanup(sender);
            return;
        }

        ClearValidationMessage();

        if (args.Item is StaffingAllocation allocation &&
            args.Column == AllocationColumn &&
            allocation.EmployeeId == "E-0319")
        {
            args.Cancel = true;
            ValidationMessage = "Finance has approved this allocation, so it cannot be changed from the staffing plan.";
            IsValidationMessageOpen = true;
        }

        QueueStaffingEditCleanup(sender);
    }

    private void StaffingTable_BeginningEdit(TableView sender, TableViewBeginningEditEventArgs args)
    {
        if (args.Item is StaffingAllocation allocation)
        {
            SetEditingStaffingAllocation(allocation);
            ClearValidationMessage();
        }
    }

    private void StaffingAllocation_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (sender is not StaffingAllocation allocation)
        {
            return;
        }

        foreach (object error in allocation.GetErrors(e.PropertyName))
        {
            ValidationMessage = error.ToString() + " Correct the value or press Esc to cancel the edit.";
            IsValidationMessageOpen = true;
            break;
        }
    }

    private void QueueStaffingEditCleanup(TableView table)
    {
        if (!table.DispatcherQueue.TryEnqueue(() =>
            {
                if (!table.IsEditing)
                {
                    SetEditingStaffingAllocation(null);
                    ClearValidationMessage();
                }
            }))
        {
            SetEditingStaffingAllocation(null);
        }
    }

    private void StaffingTable_Unloaded(object sender, RoutedEventArgs e)
    {
        SetEditingStaffingAllocation(null);
    }

    private void SetEditingStaffingAllocation(StaffingAllocation? allocation)
    {
        if (_editingStaffingAllocation == allocation)
        {
            return;
        }

        if (_editingStaffingAllocation is not null)
        {
            _editingStaffingAllocation.ErrorsChanged -= StaffingAllocation_ErrorsChanged;
        }

        _editingStaffingAllocation = allocation;

        if (_editingStaffingAllocation is not null)
        {
            _editingStaffingAllocation.ErrorsChanged += StaffingAllocation_ErrorsChanged;
        }
    }

    private void ClearValidationMessage()
    {
        ValidationMessage = string.Empty;
        IsValidationMessageOpen = false;
    }

    private void DensityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox &&
            comboBox.SelectedItem is ComboBoxItem selectedItem &&
            Enum.TryParse(selectedItem.Tag?.ToString(), out TableViewDensity density))
        {
            VirtualizedTable.Density = density;
        }
    }

    private void GridLinesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox &&
            comboBox.SelectedItem is ComboBoxItem selectedItem &&
            Enum.TryParse(selectedItem.Tag?.ToString(), out TableViewGridLinesVisibility visibility))
        {
            VirtualizedTable.GridLinesVisibility = visibility;
        }
    }

    private void EmptyStateToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        if (toggleSwitch.IsOn)
        {
            PerformanceRows.Clear();
        }
        else if (PerformanceRows.Count == 0)
        {
            PopulatePerformanceRows();
        }
    }

    private void PopulatePerformanceRows()
    {
        string[] regions = ["East US", "West US", "North Europe", "Southeast Asia"];
        string[] serviceNames = ["Order API", "Inventory sync", "Billing worker", "Identity gateway", "Reporting pipeline"];

        for (int index = 1; index <= 1000; index++)
        {
            int requestRate = 850 + ((index * 37) % 4200);
            int latency = 42 + ((index * 13) % 180);
            double availability = 99.50 + ((index * 7) % 50) / 100.0;

            PerformanceRows.Add(new PerformanceMetric(
                $"SVC-{index:0000}",
                serviceNames[index % serviceNames.Length],
                regions[index % regions.Length],
                requestRate.ToString("N0"),
                $"{latency} ms",
                $"{availability:F2}%"));
        }
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed class InventoryRecord
{
    public string AssetId { get; }
    public string ItemName { get; }
    public string Status { get; }
    public string StatusGlyph { get; }
    public string Warehouse { get; }
    public string Location { get; }
    public int Quantity { get; }
    public int ReorderPoint { get; }
    public string LastAudit { get; }
    public string NextAction { get; }

    public InventoryRecord(
        string assetId,
        string itemName,
        string status,
        string statusGlyph,
        string warehouse,
        string location,
        int quantity,
        int reorderPoint,
        string lastAudit,
        string nextAction)
    {
        AssetId = assetId;
        ItemName = itemName;
        Status = status;
        StatusGlyph = statusGlyph;
        Warehouse = warehouse;
        Location = location;
        Quantity = quantity;
        ReorderPoint = reorderPoint;
        LastAudit = lastAudit;
        NextAction = nextAction;
    }
}

public sealed class StaffingAllocation : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private string _employeeName;
    private string _role;
    private string _allocationPercent;
    private string _budgetThousands;
    private readonly Dictionary<string, List<string>> _errors = [];

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errors.Count > 0;

    public string EmployeeId { get; }

    public string EmployeeName
    {
        get => _employeeName;
        set
        {
            if (SetProperty(ref _employeeName, value))
            {
                SetError(nameof(EmployeeName), string.IsNullOrWhiteSpace(value) ? "Employee name is required." : null);
            }
        }
    }

    public string Role
    {
        get => _role;
        set
        {
            if (SetProperty(ref _role, value))
            {
                SetError(nameof(Role), string.IsNullOrWhiteSpace(value) ? "Role is required." : null);
            }
        }
    }

    public string AllocationPercent
    {
        get => _allocationPercent;
        set
        {
            string normalizedValue = value ?? string.Empty;
            if (SetProperty(ref _allocationPercent, normalizedValue))
            {
                string? error = !int.TryParse(normalizedValue, out int allocation)
                    ? "Allocation must be a whole number."
                    : allocation < 0 || allocation > 100
                        ? "Allocation must be from 0 through 100 percent."
                        : null;
                SetError(
                    nameof(AllocationPercent),
                    error);
            }
        }
    }

    public string BudgetThousands
    {
        get => _budgetThousands;
        set
        {
            string normalizedValue = value ?? string.Empty;
            if (SetProperty(ref _budgetThousands, normalizedValue))
            {
                string? error = !int.TryParse(normalizedValue, out int budget)
                    ? "Budget must be a whole number."
                    : budget < 0
                        ? "Budget must be zero or greater."
                        : null;
                SetError(nameof(BudgetThousands), error);
            }
        }
    }

    public StaffingAllocation(
        string employeeId,
        string employeeName,
        string role,
        int allocationPercent,
        int budgetThousands)
    {
        EmployeeId = employeeId;
        _employeeName = employeeName;
        _role = role;
        _allocationPercent = allocationPercent.ToString();
        _budgetThousands = budgetThousands.ToString();
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            foreach (List<string> propertyErrors in _errors.Values)
            {
                foreach (string error in propertyErrors)
                {
                    yield return error;
                }
            }
        }
        else if (_errors.TryGetValue(propertyName, out List<string>? errors))
        {
            foreach (string error in errors)
            {
                yield return error;
            }
        }
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    private void SetError(string propertyName, string? error)
    {
        string? existingError = _errors.TryGetValue(propertyName, out List<string>? errors)
            ? errors[0]
            : null;
        if (existingError == error)
        {
            return;
        }

        if (error is null)
        {
            _errors.Remove(propertyName);
        }
        else
        {
            _errors[propertyName] = [error];
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}

public sealed class PerformanceMetric
{
    public string ServiceId { get; }
    public string ServiceName { get; }
    public string Region { get; }
    public string RequestsPerMinute { get; }
    public string P95Latency { get; }
    public string Availability { get; }

    public PerformanceMetric(
        string serviceId,
        string serviceName,
        string region,
        string requestsPerMinute,
        string p95Latency,
        string availability)
    {
        ServiceId = serviceId;
        ServiceName = serviceName;
        Region = region;
        RequestsPerMinute = requestsPerMinute;
        P95Latency = p95Latency;
        Availability = availability;
    }
}
