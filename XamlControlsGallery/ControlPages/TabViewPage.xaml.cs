using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using AppUIBasics.SamplePages;

namespace AppUIBasics.ControlPages
{
    public sealed partial class TabViewPage : Page
    {
        public TabViewPage()
        {
            this.InitializeComponent();

            for (int i = 0; i < 5; i++)
            {
                TabView1.Items.Add(CreateNewTab(i));
                TabView2.Items.Add(CreateNewTab(i));
            }
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            sender.Items.Add(CreateNewTab(sender.Items.Count));
        }

        private TabViewItem CreateNewTab(int index)
        {
            TabViewItem newItem = new TabViewItem();

            newItem.Header = "Document " + index;
            newItem.Icon = new SymbolIcon(Symbol.Document);

            Frame frame = new Frame();

            switch (index % 3)
            {
                case 0:
                    frame.Navigate(typeof(SamplePage1));
                    break;
                case 1:
                    frame.Navigate(typeof(SamplePage2));
                    break;
                case 2:
                    frame.Navigate(typeof(SamplePage3));
                    break;
            }

            newItem.Content = frame;

            return newItem;
        }

        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var senderTabView = args.Element as TabView;
            senderTabView.Items.Add(CreateNewTab(senderTabView.Items.Count));
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            // Only close the selected tab if it is closeable
            if (((TabViewItem)InvokedTabView.SelectedItem).IsCloseable)
            {
                InvokedTabView.Items.Remove(InvokedTabView.SelectedItem);
            }
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = InvokedTabView.Items.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < InvokedTabView.Items.Count)
            {
                InvokedTabView.SelectedIndex = tabToSelect;
            }
        }
    }
}
