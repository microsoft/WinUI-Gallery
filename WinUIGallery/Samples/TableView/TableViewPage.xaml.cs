// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewPage : Page
{
    private TableViewSampleItem? _editingItem;

    public ObservableCollection<TableViewSampleItem> BasicItems { get; } = TableViewSampleItem.CreateItems();
    public ObservableCollection<TableViewSampleItem> CustomColumnItems { get; } = TableViewSampleItem.CreateItems();
    public ObservableCollection<TableViewSampleItem> EditingItems { get; } = TableViewSampleItem.CreateItems();
    public ObservableCollection<TableViewSampleItem> PresentationItems { get; } = TableViewSampleItem.CreateItems(1000);

    public TableViewPage()
    {
        InitializeComponent();
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