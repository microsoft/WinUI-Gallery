// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewSortingPage : Page
{
    public ObservableCollection<TableViewEmployee> SortingEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewSortingPage()
    {
        InitializeComponent();
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
}
