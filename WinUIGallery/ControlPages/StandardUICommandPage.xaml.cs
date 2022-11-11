using System;
using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ICommand = System.Windows.Input.ICommand;

namespace AppUIBasics.ControlPages
{
    public class ListItemData
    {
        public string Text { get; set; }
        public ICommand Command { get; set; }
    }

    public sealed partial class StandardUICommandPage : Page
    {
        ObservableCollection<ListItemData> collection = new ObservableCollection<ListItemData>();

        public StandardUICommandPage()
        {
            this.InitializeComponent();
        }

        private void DeleteCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter != null)
            {
                foreach (var i in collection)
                {
                    if (i.Text == (args.Parameter as string))
                    {
                        collection.Remove(i);
                        return;
                    }
                }
            }
            if (ListViewRight.SelectedIndex != -1)
            {
                collection.RemoveAt(ListViewRight.SelectedIndex);
            }
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.ItemsSource = collection;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewRight.SelectedIndex != -1)
            {
                var item = collection[ListViewRight.SelectedIndex];
            }
        }

        private void ListViewSwipeContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
            }
        }

        private void ListViewSwipeContainer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
        }

        private void ControlExample_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                var deleteCommand = new StandardUICommand(StandardUICommandKind.Delete);
                deleteCommand.ExecuteRequested += DeleteCommand_ExecuteRequested;

                DeleteFlyoutItem.Command = deleteCommand;

                for (var i = 0; i < 15; i++)
                {
                    collection.Add(new ListItemData { Text = "List item " + i.ToString(), Command = deleteCommand });
                }
            }
            else
            {
                for (var i = 0; i < 15; i++)
                {
                    collection.Add(new ListItemData { Text = "List item " + i.ToString(), Command = null });
                }
            }
        }

        private void ListViewRight_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            MenuFlyout flyout = new MenuFlyout();
            ListItemData data = (ListItemData)args.Item;
            MenuFlyoutItem item = new MenuFlyoutItem() { Command = data.Command };
            flyout.Opened += delegate (object element, object e) {
                MenuFlyout flyoutElement = element as MenuFlyout;
                ListViewItem elementToHighlight = flyoutElement.Target as ListViewItem;
                elementToHighlight.IsSelected = true;
            };
            flyout.Items.Add(item);
            args.ItemContainer.ContextFlyout = flyout;
        }
    }
}
