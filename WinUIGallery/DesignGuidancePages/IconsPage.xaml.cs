using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using AppUIBasics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIGallery.DesktopWap.DataModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.DesktopWap.DesignGuidancePages
{
    public sealed partial class IconsPage : ItemsPageBase
    {
        public ObservableCollection<IconData> FilteredItems = new ObservableCollection<IconData>();


        public IconData SelectedItem
        {
            get { return (IconData)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(IconData), typeof(IconsPage), new PropertyMetadata(null));



        public IconsPage()
        {
            // Fill filtered items
            IconsDataSource.Icons.ForEach(item => FilteredItems.Add(item));
            this.InitializeComponent();
            IconsRepeater.Loaded += ItemsGridView_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
            args.NavigationRootPage.NavigationView.Header = string.Empty;
        }

        private void ItemsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            // Delegate loading of icons, so we have smooth navigating to this page
            // and not unnecessarily block UI Thread
            Task.Run(delegate ()
            {
                _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.High, () =>
                {
                    IconsRepeater.ItemsSource = FilteredItems;
                    SelectedItem = FilteredItems[0];
                });
            });
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            Filter((sender as TextBox).Text);
        }
        
        public void Filter(string search)
        {
            string[] filter = search?.Split(" ");

            FilteredItems.Clear();

            foreach (var item in IconsDataSource.Icons)
            {
                var fitsFilter = filter.All(entry => item.Code.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase)
                        || item.Name.Contains(entry, System.StringComparison.CurrentCultureIgnoreCase));

                if (fitsFilter)
                {
                    FilteredItems.Add(item);
                }
            }
            SelectedItem = FilteredItems[0];
        }

        private void Icons_TemplatePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var itemIndex = IconsRepeater.GetElementIndex(sender as UIElement);
            SelectedItem = FilteredItems[itemIndex != -1 ? itemIndex : 0];
        }
    }
}
