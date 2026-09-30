// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewPage : Page
{
    private readonly TableViewSource _filteringGroupingSource = TableViewSource.From(TableViewSampleItem.CreateItems(12));
    private readonly List<ItemsRepeater> _rowRepeaters = [];
    private TableViewSampleItem? _editingItem;

    public ObservableCollection<TableViewSampleItem> BasicItems { get; } = TableViewSampleItem.CreateItems();
    public ObservableCollection<TableViewSampleItem> CustomColumnItems { get; } = TableViewSampleItem.CreateItems();
    public ObservableCollection<TableViewSampleItem> EditingItems { get; } = TableViewSampleItem.CreateItems();
    public ObservableCollection<TableViewSampleItem> PresentationItems { get; } = TableViewSampleItem.CreateItems(1000);

    public TableViewPage()
    {
        InitializeComponent();
        ApplyRowBanding();
        FilteringGroupingTable.ItemsSource = _filteringGroupingSource;
        ApplyGrouping();

        TableView[] tables = [BasicTable, FilteringGroupingTable, CustomColumnsTable, EditingTable, PresentationTable];
        foreach (TableView table in tables)
        {
            table.Loaded += TableView_Loaded;
        }

        Unloaded += TableViewPage_Unloaded;
    }

    private void TableView_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TableView table)
        {
            return;
        }

        ItemsRepeater? rowsRepeater = table.GetDescendantsOfType<ItemsRepeater>()
            .FirstOrDefault(element => element.Name == "PART_RowsRepeater");
        if (rowsRepeater is null)
        {
            return;
        }

        if (!_rowRepeaters.Contains(rowsRepeater))
        {
            _rowRepeaters.Add(rowsRepeater);
            rowsRepeater.ElementPrepared += RowsRepeater_ElementPrepared;
        }

        foreach (TableViewRow row in rowsRepeater.GetDescendantsOfType<TableViewRow>())
        {
            BindRowAutomationName(row);
        }
    }

    private static void RowsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if (args.Element is TableViewRow row)
        {
            BindRowAutomationName(row);
        }
    }

    private static void BindRowAutomationName(TableViewRow row)
    {
        row.SetBinding(
            AutomationProperties.NameProperty,
            new Binding
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(nameof(TableViewSampleItem.AutomationName)),
            });
    }

    private void TableViewPage_Unloaded(object sender, RoutedEventArgs e)
    {
        foreach (ItemsRepeater rowsRepeater in _rowRepeaters)
        {
            rowsRepeater.ElementPrepared -= RowsRepeater_ElementPrepared;
        }

        _rowRepeaters.Clear();
    }

    private void BasicTable_SelectionChanged(TableView sender, SelectionChangedEventArgs args)
    {
        SelectionStatusText.Text = sender.SelectedItem is TableViewSampleItem item
            ? $"Selected: {item.Name} (ID {item.Id})."
            : "No row selected.";
    }

    private void SelectionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectionModeComboBox.SelectedItem is ComboBoxItem { Tag: string selectionMode })
        {
            BasicTable.SelectionMode = Enum.Parse<TableViewSelectionMode>(selectionMode);
        }
    }

    private void BasicTable_Sorted(TableView sender, TableViewSortedEventArgs args)
    {
        SortStatusText.Text = args.Direction == SortDirection.None
            ? "Rows are in source order."
            : $"Sorted by {args.Column.Header}, {args.Direction.ToString().ToLowerInvariant()}.";
    }

    private void FreezeFirstColumnToggle_Toggled(object sender, RoutedEventArgs e)
    {
        BasicIdColumn.FrozenEdge = FreezeFirstColumnToggle.IsOn
            ? TableViewFrozenEdge.Leading
            : TableViewFrozenEdge.None;
    }

    private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = FilterBox.Text.Trim();
        if (text.Length == 0)
        {
            _filteringGroupingSource.ClearFilter();
            return;
        }

        _filteringGroupingSource.Filter(item => item is TableViewSampleItem sampleItem &&
            (sampleItem.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase) ||
             sampleItem.Category.Contains(text, StringComparison.CurrentCultureIgnoreCase)));
    }

    private void GroupByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyGrouping();
    }

    private void ApplyGrouping()
    {
        // SelectionChanged can run before the named ComboBox is initialized.
        if (GroupByComboBox is null)
        {
            return;
        }

        if (GroupByComboBox.SelectedIndex == 1)
        {
            _filteringGroupingSource.GroupBy(item => ((TableViewSampleItem)item).Category);
        }
        else
        {
            _filteringGroupingSource.ClearGroupBy();
        }
    }

    private void ExpandAllGroups_Click(object sender, RoutedEventArgs e)
    {
        FilteringGroupingTable.ExpandAllGroups();
    }

    private void CollapseAllGroups_Click(object sender, RoutedEventArgs e)
    {
        FilteringGroupingTable.CollapseAllGroups();
    }

    private void EditingTable_BeginningEdit(TableView sender, TableViewBeginningEditEventArgs args)
    {
        if (args.Item is TableViewSampleItem item)
        {
            SetEditingItem(item);
            ClearValidationMessage();
        }
    }

    private void EditingTable_CellEditEnding(TableView sender, TableViewCellEditEndingEventArgs args)
    {
        ClearValidationMessage();
        QueueEditCleanup(sender);
    }

    private void EditingItem_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (sender is not TableViewSampleItem item)
        {
            return;
        }

        foreach (object error in item.GetErrors(e.PropertyName))
        {
            ValidationInfoBar.Message = error + " Enter a name or press Esc to cancel the edit.";
            ValidationInfoBar.IsOpen = true;
            break;
        }
    }

    private void QueueEditCleanup(TableView table)
    {
        // CellEditEnding runs before the control finishes committing or rolling back the value.
        if (!table.DispatcherQueue.TryEnqueue(() =>
            {
                if (!table.IsEditing)
                {
                    SetEditingItem(null);
                    ClearValidationMessage();
                }
            }))
        {
            SetEditingItem(null);
        }
    }

    private void EditingTable_Unloaded(object sender, RoutedEventArgs e)
    {
        SetEditingItem(null);
    }

    private void SetEditingItem(TableViewSampleItem? item)
    {
        if (_editingItem == item)
        {
            return;
        }

        if (_editingItem is not null)
        {
            _editingItem.ErrorsChanged -= EditingItem_ErrorsChanged;
        }

        _editingItem = item;

        if (_editingItem is not null)
        {
            _editingItem.ErrorsChanged += EditingItem_ErrorsChanged;
        }
    }

    private void ClearValidationMessage()
    {
        ValidationInfoBar.IsOpen = false;
        ValidationInfoBar.Message = string.Empty;
    }

    private void DensityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DensityComboBox.SelectedItem is ComboBoxItem { Tag: string density })
        {
            PresentationTable.Density = Enum.Parse<TableViewDensity>(density);
        }
    }

    private void GridLinesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridLinesComboBox.SelectedItem is ComboBoxItem { Tag: string visibility })
        {
            PresentationTable.GridLinesVisibility = Enum.Parse<TableViewGridLinesVisibility>(visibility);
        }
    }

    private void RowBandingToggle_Toggled(object sender, RoutedEventArgs e)
    {
        ApplyRowBanding();
    }

    private void ApplyRowBanding()
    {
        if (PresentationTable is null || RowBandingToggle is null)
        {
            return;
        }

        PresentationTable.AlternatingRowBackground = RowBandingToggle.IsOn
            ? (Brush)Application.Current.Resources["SubtleFillColorSecondaryBrush"]
            : null;
        RowBandingToggle.Tag = RowBandingToggle.IsOn
            ? "ThemeResource SubtleFillColorSecondaryBrush"
            : "x:Null";
    }

    private void EmptyStateToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (EmptyStateToggle.IsOn)
        {
            PresentationItems.Clear();
        }
        else if (PresentationItems.Count == 0)
        {
            foreach (TableViewSampleItem item in TableViewSampleItem.CreateItems(1000))
            {
                PresentationItems.Add(item);
            }
        }
    }
}