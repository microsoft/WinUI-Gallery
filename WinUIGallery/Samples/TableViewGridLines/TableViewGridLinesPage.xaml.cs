// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewGridLinesPage : Page
{
    public ObservableCollection<TableViewEmployee> GridLinesEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewGridLinesPage()
    {
        InitializeComponent();
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
}
