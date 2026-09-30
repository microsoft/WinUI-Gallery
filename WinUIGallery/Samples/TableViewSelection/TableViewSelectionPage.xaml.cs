// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewSelectionPage : Page
{
    public ObservableCollection<TableViewEmployee> SelectionEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewSelectionPage()
    {
        InitializeComponent();
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
