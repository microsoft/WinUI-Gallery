// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class TeachingTipPage : Page
{
    public TeachingTipPage()
    {
        this.InitializeComponent();
    }

    private void TestButton1Click(object sender, RoutedEventArgs e)
    {
        TestButton1TeachingTip.IsOpen = true;
    }

    private void TestButton2Click(object sender, RoutedEventArgs e)
    {
        TestButton2TeachingTip.IsOpen = true;
    }

    private void TestButton3Click(object sender, RoutedEventArgs e)
    {
        TestButton3TeachingTip.IsOpen = true;
    }
}
