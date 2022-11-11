using System;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;
using AppUIBasics.Helper;
using System.Threading;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;
using Windows.System;
using DispatcherQueueHandler = Microsoft.UI.Dispatching.DispatcherQueueHandler;

namespace AppUIBasics.TabViewPages
{
    public sealed partial class TabViewWindowingSamplePage : Page
    {
        private const string DataIdentifier = "MyTabItem";
        public TabViewWindowingSamplePage()
        {
            this.InitializeComponent();

            Tabs.TabItemsChanged += Tabs_TabItemsChanged;

            Loaded += TabViewWindowingSamplePage_Loaded;
        }

        private void TabViewWindowingSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            var currentWindow = WindowHelper.GetWindowForElement(this);
            currentWindow.ExtendsContentIntoTitleBar = true;
            currentWindow.SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;
        }

        private void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                WindowHelper.GetWindowForElement(this).Close();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetupWindow();
        }

        void SetupWindow()
        {

            // Main Window -- add some default items
            for (int i = 0; i < 3; i++)
            {
                Tabs.TabItems.Add(new TabViewItem() { IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder }, Header = $"Item {i}", Content = new MyTabContentControl() { DataContext = $"Page {i}" } });
            }

            Tabs.SelectedIndex = 0;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // To ensure that the tabs in the titlebar are not occluded by shell
            // content, we must ensure that we account for left and right overlays.
            // In LTR layouts, the right inset includes the caption buttons and the
            // drag region, which is flipped in RTL.

            // The SystemOverlayLeftInset and SystemOverlayRightInset values are
            // in terms of physical left and right. Therefore, we need to flip
            // then when our flow direction is RTL.
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
                ShellTitleBarInset.MinWidth = sender.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
                ShellTitleBarInset.MinWidth = sender.SystemOverlayRightInset;
            }

            // Ensure that the height of the custom regions are the same as the titlebar.
            CustomDragRegion.Height = ShellTitleBarInset.Height = sender.Height;
        }

        public void AddTabToTabs(TabViewItem tab)
        {
            Tabs.TabItems.Add(tab);
        }

        // Create a new Window once the Tab is dragged outside.
        private void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            var newPage = new TabViewWindowingSamplePage();

            Tabs.TabItems.Remove(args.Tab);
            newPage.AddTabToTabs(args.Tab);

            var newWindow = WindowHelper.CreateWindow();
            newWindow.ExtendsContentIntoTitleBar = true;
            newWindow.Content = newPage;

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

        private async void Tabs_TabStripDrop(object sender, DragEventArgs e)
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

                    // The TabViewItem can only be in one tree at a time. Before moving it to the new TabView, remove it from the old.
                    // Note that this call can happen on a different thread if moving across windows. So make sure you call methods on
                    // the same thread as where the UI Elements were created.

                    object header = null;
                    object dataContext = null;
                    var element = (obj as UIElement);

                    var taskCompletionSource = new TaskCompletionSource();

                    element.DispatcherQueue.TryEnqueue(
                        Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                        new DispatcherQueueHandler(() =>
                        {
                            var tabItem = obj as TabViewItem;
                            var destinationTabViewListView = (tabItem.Parent as TabViewListView);
                            destinationTabViewListView.Items.Remove(obj);
                            header = tabItem.Header;
                            dataContext = (tabItem.Content as MyTabContentControl).DataContext;

                            taskCompletionSource.SetResult();
                        }));

                    await taskCompletionSource.Task;

                    var insertedItem = CreateNewTVI(header.ToString(), dataContext.ToString());
                    if (index < 0)
                    {
                        // We didn't find a transition point, so we're at the end of the list
                        destinationItems.Add(insertedItem);
                    }
                    else if (index < destinationTabView.TabItems.Count)
                    {
                        // Otherwise, insert at the provided index.
                        destinationItems.Insert(index, insertedItem);
                    }

                    // Select the newly dragged tab
                    destinationTabView.SelectedItem = insertedItem;
                }
            }
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

            return newTab;
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
            var tab = CreateNewTVI("New Item", "New Item");
            sender.TabItems.Add(tab);
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }
    }
}
