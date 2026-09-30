// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace WinUIGallery.ControlPages;

public sealed partial class TableViewCellTemplatesPage : Page
{
    public ObservableCollection<TableViewEmployee> TemplateEmployees { get; } = TableViewEmployee.CreateSampleData();

    public TableViewCellTemplatesPage()
    {
        InitializeComponent();
    }
}
