// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewEmptyTemplatePage : Page
{
    public ObservableCollection<TableViewEmployee> EmptyStateEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewEmptyTemplatePage()
    {
        InitializeComponent();
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
}
