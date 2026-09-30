// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewPage : Page
{
    public ObservableCollection<TableViewEmployee> SortingEmployees { get; } = TableViewEmployee.CreateSampleData();
    private readonly TableViewSource _filteringSource = TableViewSource.From(TableViewEmployee.CreateSampleData());
    private readonly TableViewSource _groupingSource = TableViewSource.From(TableViewEmployee.CreateSampleData());
    public ObservableCollection<TableViewEmployee> ResizingEmployees { get; } = TableViewEmployee.CreateSampleData();
    public ObservableCollection<TableViewEmployee> ToolTipEmployees { get; } = TableViewEmployee.CreateSampleData();
    public ObservableCollection<TableViewEmployee> EditingEmployees { get; } = TableViewEmployee.CreateSampleData();
    public ObservableCollection<TableViewEmployee> TemplateEmployees { get; } = TableViewEmployee.CreateSampleData();
    public ObservableCollection<TableViewEmployee> FrozenEmployees { get; } = TableViewEmployee.CreateSampleData();
    public ObservableCollection<TableViewEmployee> GridLinesEmployees { get; } = TableViewEmployee.CreateSampleData();
    public ObservableCollection<TableViewEmployee> EmptyStateEmployees { get; } = TableViewEmployee.CreateSampleData();
    public ObservableCollection<TableViewEmployee> SelectionEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewPage()
    {
        InitializeComponent();
        FilteringTable.ItemsSource = _filteringSource;
        GroupingTable.ItemsSource = _groupingSource;
        ApplyGrouping();
    }

    private void SortingTable_Sorted(TableView sender, TableViewSortedEventArgs args)
    {
        SortingOutput.Text = args.Direction == SortDirection.None || args.Column is null
            ? "Rows are in source order."
            : $"Sorted by {args.Column.Header}, {args.Direction}.";
    }

    private void ClearSort_Click(object sender, RoutedEventArgs e)
    {
        SortingTable.ClearSort();
    }

    private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = FilterBox.Text.Trim();
        if (text.Length == 0)
        {
            _filteringSource.ClearFilter();
            return;
        }

        _filteringSource.Filter(item => item is TableViewEmployee employee &&
            (employee.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase) ||
             employee.Role.Contains(text, StringComparison.CurrentCultureIgnoreCase) ||
             employee.Department.Contains(text, StringComparison.CurrentCultureIgnoreCase)));
    }

    private void GroupByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyGrouping();
    }

    private void ApplyGrouping()
    {
        // SelectionChanged is raised while the page is still loading.
        if (_groupingSource is null || GroupByComboBox is null)
        {
            return;
        }

        switch (GroupByComboBox.SelectedIndex)
        {
            case 1:
                _groupingSource.GroupBy(item => ((TableViewEmployee)item).Department);
                break;
            case 2:
                _groupingSource.GroupBy(item => ((TableViewEmployee)item).Location);
                break;
            default:
                _groupingSource.ClearGroupBy();
                break;
        }
    }

    private void ExpandAllGroups_Click(object sender, RoutedEventArgs e) => GroupingTable.ExpandAllGroups();

    private void CollapseAllGroups_Click(object sender, RoutedEventArgs e) => GroupingTable.CollapseAllGroups();

    private void EditingTable_CellEditEnding(TableView sender, TableViewCellEditEndingEventArgs args)
    {
        string action = args.EditAction == TableViewEditAction.Commit ? "Committed" : "Canceled";
        string id = args.Item is TableViewEmployee employee ? employee.Id : string.Empty;
        EditingOutput.Text = $"{action} the edit to {args.Column?.Header} for {id}.";
    }

    private void FreezeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        FrozenNameColumn.FrozenEdge = FreezeToggle.IsOn ? TableViewFrozenEdge.Leading : TableViewFrozenEdge.None;
    }

    private void GridLinesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridLinesTable is not null &&
            GridLinesComboBox.SelectedItem is ComboBoxItem item &&
            Enum.TryParse(item.Tag?.ToString(), out TableViewGridLinesVisibility visibility))
        {
            GridLinesTable.GridLinesVisibility = visibility;
        }
    }

    private void EmptyStateToggle_Toggled(object sender, RoutedEventArgs e)
    {
        EmptyStateEmployees.Clear();
        if (!EmptyStateToggle.IsOn)
        {
            foreach (TableViewEmployee employee in TableViewEmployee.CreateSampleData())
            {
                EmptyStateEmployees.Add(employee);
            }
        }
    }

    private void SelectionTable_SelectionChanged(TableView sender, SelectionChangedEventArgs args)
    {
        SelectionOutput.Text = sender.SelectedItem is TableViewEmployee employee
            ? $"Selected {employee.Name} (row {sender.SelectedIndex + 1})."
            : "No row selected.";
    }

    private void SelectionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectionTable is not null &&
            SelectionModeComboBox.SelectedItem is ComboBoxItem item &&
            Enum.TryParse(item.Tag?.ToString(), out TableViewSelectionMode mode))
        {
            SelectionTable.SelectionMode = mode;
        }
    }

    private void SelectFirstRow_Click(object sender, RoutedEventArgs e) => SelectionTable.Select(0);

    private void DeselectAll_Click(object sender, RoutedEventArgs e) => SelectionTable.DeselectAll();
}
