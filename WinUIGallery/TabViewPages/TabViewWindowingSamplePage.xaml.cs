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
using WinUIGallery.Helper;
using System.Threading;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;
using Windows.System;
using DispatcherQueueHandler = Microsoft.UI.Dispatching.DispatcherQueueHandler;
using System.Linq;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;

namespace WinUIGallery.TabViewPages
{
    public sealed partial class TabViewWindowingSamplePage : Page
    {
        private const string DataIdentifier = "MyTabItem";
        private Win32WindowHelper win32WindowHelper;
        private Window tabTearOutWindow = null;

        public TabViewWindowingSamplePage()
        {
            this.InitializeComponent();

            Loaded += TabViewWindowingSamplePage_Loaded;
        }

        public void SetupWindowMinSize(Window window)
        {
            win32WindowHelper = new Win32WindowHelper(window);
            win32WindowHelper.SetWindowMinMaxSize(new Win32WindowHelper.POINT() { x = 500, y = 300 });
        }

        private void TabViewWindowingSamplePage_Loaded(object sender, RoutedEventArgs e)
        {
            var currentWindow = WindowHelper.GetWindowForElement(this);
            currentWindow.ExtendsContentIntoTitleBar = true;
            currentWindow.SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;
        }

        public void LoadDemoData()
        {
            // Main Window -- add some default items
            for (int i = 0; i < 3; i++)
            {
                Tabs.TabItems.Add(new TabViewItem() { IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder }, Header = $"Item {i}", Content = new MyTabContentControl() { DataContext = $"Page {i}" } });
            }

            Tabs.SelectedIndex = 0;
        }

        public void AddTabToTabs(TabViewItem tab)
        {
            Tabs.TabItems.Add(tab);
        }

        private void Tabs_TabTearOutWindowRequested(TabView sender, TabViewTabTearOutWindowRequestedEventArgs args)
        {
            var newPage = new TabViewWindowingSamplePage();

            tabTearOutWindow = WindowHelper.CreateWindow();
            tabTearOutWindow.ExtendsContentIntoTitleBar = true;
            tabTearOutWindow.Content = newPage;
            tabTearOutWindow.AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
            newPage.SetupWindowMinSize(tabTearOutWindow);

            args.NewWindowId = tabTearOutWindow.AppWindow.Id;
        }

        private void Tabs_TabTearOutRequested(TabView sender, TabViewTabTearOutRequestedEventArgs args)
        {
            if (tabTearOutWindow == null)
            {
                return;
            }

            var newPage = (TabViewWindowingSamplePage)tabTearOutWindow.Content;

            foreach (TabViewItem tab in args.Tabs.Cast<TabViewItem>())
            {
                GetParentTabView(tab).TabItems.Remove(tab);
                newPage.AddTabToTabs(tab);
            }
        }

        private void Tabs_ExternalTornOutTabsDropping(TabView sender, TabViewExternalTornOutTabsDroppingEventArgs args)
        {
            args.AllowDrop = true;
        }

        private void Tabs_ExternalTornOutTabsDropped(TabView sender, TabViewExternalTornOutTabsDroppedEventArgs args)
        {
            int position = 0;

            foreach (TabViewItem tab in args.Tabs.Cast<TabViewItem>())
            {
                GetParentTabView(tab).TabItems.Remove(tab);
                sender.TabItems.Insert(args.DropIndex + position, tab);
                position++;
            }
        }

        private TabView GetParentTabView(TabViewItem tab)
        {
            DependencyObject current = tab;

            while (current != null)
            {
                if (current is TabView tabView)
                {
                    return tabView;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
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

        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            var tab = CreateNewTVI("New Item", "New Item");
            sender.TabItems.Add(tab);
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);

            if (sender.TabItems.Count == 0)
            {
                WindowHelper.GetWindowForElement(this).Close();
            }
        }
    }
}
