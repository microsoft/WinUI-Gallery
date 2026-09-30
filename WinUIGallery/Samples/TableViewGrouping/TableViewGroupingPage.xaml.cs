// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewGroupingPage : Page
{
    private readonly TableViewSource _groupingSource = TableViewSource.From(TableViewEmployee.CreateSampleData());

    public TableViewGroupingPage()
    {
        InitializeComponent();
        GroupingTable.ItemsSource = _groupingSource;
        ApplyGrouping();
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
}
