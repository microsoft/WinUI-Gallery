using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using AppUIBasics.SamplePages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabViewPage : Page
    {
        public TabViewPage()
        {
            this.InitializeComponent();
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            sender.Items.Add(CreateNewTab());
        }

        private TabViewItem CreateNewTab()
        {
            TabViewItem newItem = new TabViewItem();

            newItem.Header = "New Document";
            newItem.Icon = new SymbolIcon(Symbol.Placeholder);

            Frame frame = new Frame();

            newItem.Content = frame;

            frame.Navigate(typeof(SamplePage1));

            return newItem;
        }
    }
}
