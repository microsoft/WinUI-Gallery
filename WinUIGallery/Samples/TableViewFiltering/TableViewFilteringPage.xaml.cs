// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewFilteringPage : Page
{
    private readonly TableViewSource _filteringSource = TableViewSource.From(TableViewEmployee.CreateSampleData());

    public TableViewFilteringPage()
    {
        InitializeComponent();
        FilteringTable.ItemsSource = _filteringSource;
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
}
