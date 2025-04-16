//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Samples.SamplePages;

namespace WinUIGallery.ControlPages;

public sealed partial class TitleBarPage : Page
{
    public TitleBarPage()
    {
        this.InitializeComponent();
    }

    private void CreateTitleBarWindowClick(object sender, RoutedEventArgs e)
    {
        TitleBarWindow titleBarWindow = new TitleBarWindow();
        titleBarWindow.Activate();
    }
}