// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Tabular;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewFrozenColumnsPage : Page
{
    public ObservableCollection<TableViewEmployee> FrozenEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewFrozenColumnsPage()
    {
        InitializeComponent();
    }

    private void FreezeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        FrozenNameColumn.FrozenEdge = FreezeToggle.IsOn ? TableViewFrozenEdge.Leading : TableViewFrozenEdge.None;
    }
}
