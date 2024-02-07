using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace WinUIGalleryUnitTests
{
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
}
