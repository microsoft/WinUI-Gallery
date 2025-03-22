//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace WinUIGallery.ControlPages;

public sealed partial class TeachingTipPage : Page
{
    public TeachingTipPage()
    {
        InitializeComponent();
    }

    private void TestButton1Click(object sender, RoutedEventArgs e) => TestButton1TeachingTip.IsOpen = true;

    private void TestButton2Click(object sender, RoutedEventArgs e) => TestButton2TeachingTip.IsOpen = true;

    private void TestButton3Click(object sender, RoutedEventArgs e) => TestButton3TeachingTip.IsOpen = true;
}
