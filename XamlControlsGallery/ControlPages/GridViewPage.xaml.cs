//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Common;
using AppUIBasics.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using Windows.Graphics.Display;
using Windows.Media.Devices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;



namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GridViewPage : ItemsPageBase
    {
        int ActualColSpace;
        int ActualRowSpace;
        int ActualMaxItems;

        ItemsWrapGrid StyledGridIWG;
        public GridViewPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            List<CustomDataObject> tempList = CustomDataObject.GetDataObjects();
            ObservableCollection<CustomDataObject> Items = new ObservableCollection<CustomDataObject>(tempList);
            ObservableCollection<CustomDataObject> Items2 = new ObservableCollection<CustomDataObject>(tempList);
            Control0.ItemsSource = Items2;
            Control1.ItemsSource = Items;
            StyledGrid.ItemsSource = Items;

            ActualColSpace = 5;
            ActualRowSpace = 5;
            ActualMaxItems = 3;
            MaxItems.Text = ActualMaxItems.ToString();
        }

        private void ItemTemplate_Checked(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement).Tag;
            if (tag != null)
            {
                var template = tag.ToString();
                Control1.ItemTemplate = (DataTemplate)this.Resources[template];
                itemTemplate.Value = template;
            }
        }

        private void Control1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView gridView)
            {
                SelectionOutput.Text = string.Format("You have selected {0} item(s).", gridView.SelectedItems.Count);
            }
        }

        private void Control1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClickOutput.Text = "You clicked " + (e.ClickedItem as CustomDataObject).Title + ".";
        }

        private void Control0_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClickOutput.Text = "You clicked " + (e.ClickedItem as CustomDataObject).Title + ".";
        }

        private void ItemClickCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ClickOutput.Text = string.Empty;
        }

        private void FlowDirectionCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (Control1.FlowDirection == FlowDirection.LeftToRight)
            {
                Control1.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                Control1.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        private void SelectionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Control1 != null)
            {
                string colorName = e.AddedItems[0].ToString();
                switch (colorName)
                {
                    case "None":
                        Control1.SelectionMode = ListViewSelectionMode.None;
                        SelectionOutput.Text = string.Empty;
                        break;
                    case "Single":
                        Control1.SelectionMode = ListViewSelectionMode.Single;
                        break;
                    case "Multiple":
                        Control1.SelectionMode = ListViewSelectionMode.Multiple;
                        break;
                    case "Extended":
                        Control1.SelectionMode = ListViewSelectionMode.Extended;
                        break;
                }
            }
        }

        private void StyledGrid_IncreaseColSpace(object sender, RoutedEventArgs e)
        {

            ActualColSpace += 1;

            // Update text box with newly increased value
            ColSpace.Text = ActualColSpace.ToString();
            ColSpace.PlaceholderText = ActualColSpace.ToString();

            for (int i = 0; i < StyledGrid.Items.Count; i++)
            {
                var item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                Thickness NewMargin = item.Margin;
                NewMargin.Left = item.Margin.Left + 1;
                NewMargin.Right = item.Margin.Right + 1;

                item.Margin = NewMargin;
            }

            NotifyPropertyChanged();
        }

        private void StyledGrid_DecreaseColSpace(object sender, RoutedEventArgs e)
        {
            if (ActualColSpace - 1 >= 0)
            {
                ActualColSpace -= 1;

                // Update text box with newly increased value
                ColSpace.Text = ActualColSpace.ToString();
                ColSpace.PlaceholderText = ActualColSpace.ToString();

                for (int i = 0; i < StyledGrid.Items.Count; i++)
                {
                    var item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                    Thickness NewMargin = item.Margin;
                    NewMargin.Left = item.Margin.Left - 1;
                    NewMargin.Right = item.Margin.Right - 1;

                    item.Margin = NewMargin;
                }
            }

            else
            {
                ColSpace.Text = "";
                ColSpace.PlaceholderText = "0";
            }

            NotifyPropertyChanged();
        }

        private void StyledGrid_IncreaseRowSpace(object sender, RoutedEventArgs e)
        {

            ActualRowSpace += 1;

            // Update text box with newly increased value
            RowSpace.Text = ActualRowSpace.ToString();
            RowSpace.PlaceholderText = ActualRowSpace.ToString();

            for (int i = 0; i < StyledGrid.Items.Count; i++)
            {
                var item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                Thickness NewMargin = item.Margin;
                NewMargin.Top = item.Margin.Top + 1;
                NewMargin.Bottom = item.Margin.Bottom + 1;

                item.Margin = NewMargin;
            }

            NotifyPropertyChanged();
        }

        private void StyledGrid_DecreaseRowSpace(object sender, RoutedEventArgs e)
        {
            if (ActualRowSpace -1 >= 0)
            {
                ActualRowSpace -= 1;

                // Update text box with newly increased value
                RowSpace.Text = ActualRowSpace.ToString();
                RowSpace.PlaceholderText = ActualRowSpace.ToString();

                for (int i = 0; i < StyledGrid.Items.Count; i++)
                {
                    var item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                    Thickness NewMargin = item.Margin;
                    NewMargin.Top = item.Margin.Top - 1;
                    NewMargin.Bottom = item.Margin.Bottom - 1;

                    item.Margin = NewMargin;
                }
            }

            else
            {
                RowSpace.Text = "";
                RowSpace.PlaceholderText = "0";
            }

            NotifyPropertyChanged();
        }

        private void StyledGrid_ChangeRow(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Make sure that value entered is a number
                int newVal = Convert.ToInt32(RowSpace.Text.ToString());

                // If empty or negative string, reset to 0.
                if (RowSpace.Text.ToString() != "" && newVal >= 0)
                {
                    RowSpace.Text = RowSpace.Text.ToString();
                    RowSpace.PlaceholderText = RowSpace.Text.ToString();

                    for (int i = 0; i < StyledGrid.Items.Count; i++)
                    {
                        var item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                        Thickness NewMargin = item.Margin;
                        NewMargin.Top = Convert.ToInt32(RowSpace.Text);
                        NewMargin.Bottom = Convert.ToInt32(RowSpace.Text);

                        item.Margin = NewMargin;
                    }
                }
                else
                {
                    RowSpace.Text = "";
                    RowSpace.PlaceholderText = "";
                }

            }
            catch (FormatException)
            {
                RowSpace.Text = "";
                RowSpace.PlaceholderText = "0";
            }        
        }

        private void StyledGrid_ChangeCol(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Make sure that value entered is a number
                int newVal = Convert.ToInt32(ColSpace.Text.ToString());

                // If empty or negative string, reset to 0.
                if (ColSpace.Text.ToString() != "" && newVal >= 0)
                {
                    ColSpace.Text = ColSpace.Text.ToString();
                    ColSpace.PlaceholderText = ColSpace.Text.ToString();

                    for (int i = 0; i < StyledGrid.Items.Count; i++)
                    {
                        var item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                        Thickness NewMargin = item.Margin;
                        NewMargin.Left = Convert.ToInt32(ColSpace.Text);
                        NewMargin.Right = Convert.ToInt32(ColSpace.Text);

                        item.Margin = NewMargin;
                    }
                }
                else
                {
                    ColSpace.Text = "";
                    ColSpace.PlaceholderText = "";
                }

            }
            catch (FormatException)
            {
                ColSpace.Text = "";
                ColSpace.PlaceholderText = "0";
            }
        }

        private void StyledGrid_IncreaseMaxItems(object sender, RoutedEventArgs e)
        {

            if (Convert.ToInt32(MaxItems.Text.ToString()) >= 0)
            { 
                ActualMaxItems += 1;
                MaxItems.Text = ActualMaxItems.ToString();
                MaxItems.PlaceholderText = ActualMaxItems.ToString();

                StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
            }
            
        }

        private void StyledGrid_DecreaseMaxItems(object sender, RoutedEventArgs e)
        {
            
            if (Convert.ToInt32(MaxItems.Text.ToString())  >= 1)
            {
                ActualMaxItems -= 1;
                MaxItems.Text = ActualMaxItems.ToString();
                MaxItems.PlaceholderText = ActualMaxItems.ToString();

                StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
            }
            else
            {
                sendMessage();
                ActualMaxItems = 1;
                MaxItems.Text = "1";
                MaxItems.PlaceholderText = "1";
            }
        }

        private void StyledGrid_ChangeMaxItems(object sender, TextChangedEventArgs e)
        {

            try
            {
                // Make sure that value entered is a number
                int newVal = Convert.ToInt32(MaxItems.Text.ToString());

                // If empty or negative string, reset to 0.
                if (Convert.ToInt32(MaxItems.Text.ToString()) >= 1)
                {
                    MaxItems.PlaceholderText = MaxItems.Text.ToString();
                    ActualMaxItems = Convert.ToInt32(MaxItems.Text);

                    StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
                }

                else
                {
                    sendMessage();
                    ActualMaxItems = 1;
                    MaxItems.Text = "1";
                    MaxItems.PlaceholderText = "1";
                }

            }
            catch (FormatException)
            {
                MaxItems.Text = "";
                MaxItems.PlaceholderText = "1";
            }
            
        }

        private void StyledGrid_InitWrapGrid(object sender, RoutedEventArgs e)
        {
            StyledGridIWG = sender as ItemsWrapGrid;
            StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
        }

        private async void sendMessage()
        {
            var messageDialog = new MessageDialog("Value cannot be less than 1.");

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }
    }
}
