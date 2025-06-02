// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.UnitTests;

public sealed partial class UnitTestAppWindow : Window
{
    public UnitTestAppWindow()
    {
        InitializeComponent();
    }

    public Grid RootGrid
    {
        get
        {
            return rootGrid;
        }
    }

    public void AddToVisualTree(UIElement element)
    {
        RootGrid.Children.Add(element);
    }

    public void CleanupVisualTree()
    {
        RootGrid.Children.Clear();
    }
}
