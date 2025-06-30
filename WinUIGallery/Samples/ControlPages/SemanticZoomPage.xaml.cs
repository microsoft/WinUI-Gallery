// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using WinUIGallery.Helpers;
using WinUIGallery.Models;

namespace WinUIGallery.ControlPages;

public sealed partial class SemanticZoomPage : Page
{
    private IEnumerable<ControlInfoDataGroup> _groups;

    public SemanticZoomPage()
    {
        this.InitializeComponent();
    }
    public IEnumerable<ControlInfoDataGroup> Groups
    {
        get { return this._groups; }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        _groups = ControlInfoDataSource.Instance.Groups;
    }

    private void List_GotFocus(object sender, RoutedEventArgs e)
    {
        Control1.StartBringIntoView();
    }
}
