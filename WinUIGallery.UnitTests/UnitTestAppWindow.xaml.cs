using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.UnitTests;

public sealed partial class UnitTestAppWindow : Window
{
    public UnitTestAppWindow()
    {
        this.InitializeComponent();
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
        this.RootGrid.Children.Add(element);
    }

    public void CleanupVisualTree()
    {
        this.RootGrid.Children.Clear();
    }
}
