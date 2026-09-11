using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using AppUIBasics.Helper;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics.TabViewPages
{
    public sealed partial class TabViewWindowingSamplePage : Page
    {
        private Window _window;

        private const string DataIdentifier = "MyTabItem";
        public TabViewWindowingSamplePage()
        {
            this.InitializeComponent();

            Tabs.TabItemsChanged += Tabs_TabItemsChanged;
        }

        private void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                _window?.Close();
            }
            // If there is only one tab left, disable dragging and reordering of Tabs.
            else if (sender.TabItems.Count == 1)
            {
                sender.CanReorderTabs = false;
                sender.CanDragTabs = false;
            }
            else
            {
                sender.CanReorderTabs = true;
                sender.CanDragTabs = true;
            }
        }

        public void SetupWindow(Window window, bool addDefaultTabs)
        {
            _window = window;
            if (addDefaultTabs)
            {
                // Main Window -- add some default items
                for (int i = 0; i < 3; i++)
                {
                    Tabs.TabItems.Add(CreateNewTVI($"Item {i}", $"Page {i}"));
                }

                Tabs.SelectedIndex = 0;

            }

            window.ExtendsContentIntoTitleBar = true;
            window.AppWindow.TitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
            window.AppWindow.TitleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
            window.SetTitleBar(CustomDragRegion);
            window.SizeChanged += (_, _) => UpdateTitleBarMetrics();
            Loaded += (_, _) => UpdateTitleBarMetrics();
        }

        private void UpdateTitleBarMetrics()
        {
            var titleBar = _window.AppWindow.TitleBar;
            double scale = XamlRoot?.RasterizationScale ?? 1;
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = titleBar.RightInset / scale;
                ShellTitlebarInset.MinWidth = titleBar.LeftInset / scale;
            }
            else
            {
                CustomDragRegion.MinWidth = titleBar.LeftInset / scale;
                ShellTitlebarInset.MinWidth = titleBar.RightInset / scale;
            }

            CustomDragRegion.Height = ShellTitlebarInset.Height = titleBar.Height / scale;
        }

        public void AddTabToTabs(TabViewItem tab)
        {
            Tabs.TabItems.Add(tab);
        }

        // Create a new Window once the Tab is dragged outside.
        private void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            MoveTabToNewWindow(args.Tab);
        }

        private void MoveTabToNewWindow(TabViewItem tab)
        {
            var newPage = new TabViewWindowingSamplePage();
            var newWindow = WindowHelper.TrackWindow(new Window { Content = newPage, Title = "TabView windowing sample" });
            newPage.SetupWindow(newWindow, false);

            Tabs.TabItems.Remove(tab);
            newPage.AddTabToTabs(tab);

            newWindow.Activate();
        }

        private void Tabs_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
        {
            // We can only drag one tab at a time, so grab the first one...
            var firstItem = args.Tab;

            // ... set the drag data to the tab...
            args.Data.Properties.Add(DataIdentifier, firstItem);

            // ... and indicate that we can move it 
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void Tabs_TabStripDrop(object sender, DragEventArgs e)
        {
            // This event is called when we're dragging between different TabViews
            // It is responsible for handling the drop of the item into the second TabView

            if (e.DataView.Properties.TryGetValue(DataIdentifier, out object obj))
            {
                // Ensure that the obj property is set before continuing. 
                if (obj == null)
                {
                    return;
                }

                var destinationTabView = sender as TabView;
                var destinationItems = destinationTabView.TabItems;

                if (destinationItems != null)
                {
                    // First we need to get the position in the List to drop to
                    var index = -1;

                    // Determine which items in the list our pointer is between.
                    for (int i = 0; i < destinationTabView.TabItems.Count; i++)
                    {
                        var item = destinationTabView.ContainerFromIndex(i) as TabViewItem;

                        if (e.GetPosition(item).X - item.ActualWidth < 0)
                        {
                            index = i;
                            break;
                        }
                    }

                    // The TabView can only be in one tree at a time. Before moving it to the new TabView, remove it from the old.
                    var destinationTabViewListView = ((obj as TabViewItem).Parent as TabViewListView);
                    destinationTabViewListView.Items.Remove(obj);

                    if (index < 0)
                    {
                        // We didn't find a transition point, so we're at the end of the list
                        destinationItems.Add(obj);
                    }
                    else if (index < destinationTabView.TabItems.Count)
                    {
                        // Otherwise, insert at the provided index.
                        destinationItems.Insert(index, obj);
                    }

                    // Select the newly dragged tab
                    destinationTabView.SelectedItem = obj;
                }
            }
        }

        // This method prevents the TabView from handling things that aren't text (ie. files, images, etc.)
        private void Tabs_TabStripDragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey(DataIdentifier))
            {
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTVI("New Item", "New Item"));
        }


        private TabViewItem CreateNewTVI(string header, string dataContext)
        {
            var newTab = new TabViewItem()
            {
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = Symbol.Placeholder
                },
                Header = header,
                Content = new MyTabContentControl()
                {
                    DataContext = dataContext
                }
            };

            var contextFlyout = new MenuFlyout();
            var moveToNewWindowFlyout = new MenuFlyoutItem();

            moveToNewWindowFlyout.Text = "Move to new window";
            moveToNewWindowFlyout.Click += MoveToNewWindowFlyout_Click;
            contextFlyout.Items.Add(moveToNewWindowFlyout);

            newTab.ContextFlyout = contextFlyout;

            void MoveToNewWindowFlyout_Click(object _sender, RoutedEventArgs e)
            {
                MoveTabToNewWindow(newTab);
            }

            return newTab;
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }
    }
}
