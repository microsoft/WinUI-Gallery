// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewEditingPage : Page
{
    public ObservableCollection<TableViewEmployee> EditingEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewEditingPage()
    {
        InitializeComponent();
    }

    private void EditingTable_CellEditEnding(TableView sender, TableViewCellEditEndingEventArgs args)
    {
        string action = args.EditAction == TableViewEditAction.Commit ? "Committed" : "Canceled";
        string id = args.Item is TableViewEmployee employee ? employee.Id : string.Empty;
        EditingOutput.Text = $"{action} the edit to {args.Column?.Header} for {id}.";
    }
}
