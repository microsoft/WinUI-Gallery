using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThemeTransitionPage : Page
    {

        private int _itemCount = 10;
        public ThemeTransitionPage()
        {
            this.InitializeComponent();

            for (int i = 0; i < _itemCount; i++)
            {
                AddRemoveListView.Items.Add(new ListViewItem() { Content = "Item " + i });
            }

            AddItemsToContentListView();
        }

        private void ShowPopupButton_Click(object sender, RoutedEventArgs e)
        {
            ExamplePopup.IsOpen = true;
            ClosePopupButton.Focus(FocusState.Programmatic);
        }

        private void ClosePopupButton_Click(object sender, RoutedEventArgs e)
        {
            ExamplePopup.IsOpen = false;
            ShowPopupButton.Focus(FocusState.Programmatic);
        }

        private void ContentRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            AddItemsToContentListView(true);
        }

        private void AddItemsToContentListView(bool ShowDifferentContent = false)
        {
            var items = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                items.Add(ShowDifferentContent ? "Updated content " + i : "Item " + i);
            }

            ContentList.ItemsSource = items;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddRemoveListView.Items.Add(new ListViewItem() { Content = "New Item " +  _itemCount.ToString()});
            _itemCount++;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddRemoveListView.Items.Count > 0)
                AddRemoveListView.Items.RemoveAt(0);
        }

        private void RepositionButton_Click(object sender, RoutedEventArgs e)
        {
            MiddleElement.Visibility = MiddleElement.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void EntranceAddButton_Click(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToInt32((sender as Button).Tag);

            for (int i = 0; i < value; i++)
            {
                EntranceStackPanel.Children.Add(new Rectangle() { Width = 50, Height = 50, Margin = ThicknessHelper.FromUniformLength(5.0), Fill = new SolidColorBrush(Microsoft.UI.Colors.LightBlue) });
            }
        }

        private void EntranceClearButton_Click(object sender, RoutedEventArgs e)
        {
            EntranceStackPanel.Children.Clear();
        }

        private void AddDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AddButton_Click(sender, e);
            DeleteButton_Click(sender, e);
        }
    }
}
