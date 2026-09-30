// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewToolTipsPage : Page
{
    public ObservableCollection<TableViewEmployee> ToolTipEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewToolTipsPage()
    {
        InitializeComponent();
    }
}
