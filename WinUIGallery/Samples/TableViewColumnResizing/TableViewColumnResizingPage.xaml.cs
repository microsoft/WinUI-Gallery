// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewColumnResizingPage : Page
{
    public ObservableCollection<TableViewEmployee> ResizingEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewColumnResizingPage()
    {
        InitializeComponent();
    }
}
