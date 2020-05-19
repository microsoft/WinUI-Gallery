//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AppUIBasics.ControlPages
{
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

            // Get data objects and place them into an ObservableCollection
            List<CustomDataObject> tempList = CustomDataObject.GetDataObjects();
            ObservableCollection<CustomDataObject> Items = new ObservableCollection<CustomDataObject>(tempList);
            ObservableCollection<CustomDataObject> Items2 = new ObservableCollection<CustomDataObject>(tempList);
            BasicGridView.ItemsSource = Items2;
            ContentGridView.ItemsSource = Items;
            StyledGrid.ItemsSource = Items;

            ActualColSpace = 5;
            ActualRowSpace = 5;
            ActualMaxItems = 3;
            MaxItems.Text = ActualMaxItems.ToString();

            DisplayDT.Value = @"<!-- ImageTemplate: -->
<DataTemplate x:Key='ImageTemplate' x:DataType='local1: CustomDataObject'>
    <Image Stretch = 'UniformToFill' Source = '{x:Bind ImageLocation}' 
           AutomationProperties.Name = '{x:Bind Title}' Width = '190' Height = '130' 
           AutomationProperties.AccessibilityView = 'Raw'/>
</DataTemplate> ";

        }

        private void ItemTemplate_Checked(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement).Tag;
            if (tag != null)
            {
                string template = tag.ToString();
                ContentGridView.ItemTemplate = (DataTemplate)this.Resources[template];
                itemTemplate.Value = template;

                if (template == "ImageTemplate")
                {
                    DisplayDT.Value = @"<!-- ImageTemplate: -->
<DataTemplate x:Key='ImageTemplate' x:DataType='local1: CustomDataObject'>
    <Image Stretch = 'UniformToFill' Source = '{x:Bind ImageLocation}' 
           AutomationProperties.Name = '{x:Bind Title}' Width = '190' Height = '130' 
           AutomationProperties.AccessibilityView = 'Raw'/>
</DataTemplate> ";
                }

                else if (template == "IconTextTemplate")
                {
                    DisplayDT.Value = @"<!-- IconTextTemplate: -->
<DataTemplate x:Key='IconTextTemplate' x:DataType='local1:CustomDataObject'>
    <RelativePanel AutomationProperties.Name='{x:Bind Title}' Width='280' MinHeight='160'>
        <Image x:Name='image'
               Width='18'
               Margin='0,4,0,0'
               RelativePanel.AlignLeftWithPanel='True'
               RelativePanel.AlignTopWithPanel='True'
               Source='{x:Bind ImageLocation}'
               Stretch='Uniform' />
        <TextBlock x:Name='title' Style='{StaticResource BaseTextBlockStyle}' Margin='8,0,0,0' 
                   Text='{x:Bind Title}' RelativePanel.RightOf='image' RelativePanel.AlignTopWithPanel='True'/>
        <TextBlock Text='{x:Bind Description}' Style='{StaticResource CaptionTextBlockStyle}' 
                   TextWrapping='Wrap' Margin='0,4,8,0' RelativePanel.Below='title' TextTrimming='WordEllipsis'/>
    </RelativePanel>
</DataTemplate>";
                }

                else if (template == "ImageTextTemplate")
                {
                    DisplayDT.Value = @"<!-- ImageTextTemplate: -->
<DataTemplate x: Key = 'ImageTextTemplate' x: DataType = 'local1:CustomDataObject'>
    <Grid AutomationProperties.Name = '{x:Bind Title}' Width = '280'>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width = 'Auto'/>
                <ColumnDefinition Width = '*'/>
        </Grid.ColumnDefinitions>
        <Image Source = '{x:Bind ImageLocation}' Height = '100' Stretch = 'Fill' VerticalAlignment = 'Top'/>
        <StackPanel Grid.Column = '1' Margin = '8,0,0,8'>
            <TextBlock Text = '{x:Bind Title}' Style = '{ThemeResource SubtitleTextBlockStyle}' Margin = '0,0,0,8'/>
            <StackPanel Orientation = 'Horizontal'>
                <TextBlock Text = '{x:Bind Views}' Style = '{ThemeResource CaptionTextBlockStyle}'/>
                    <TextBlock Text = ' Views ' Style = '{ThemeResource CaptionTextBlockStyle}'/>
            </StackPanel>
            <StackPanel Orientation = 'Horizontal'>
                    <TextBlock Text = '{x:Bind Likes}' Style = '{ThemeResource CaptionTextBlockStyle}'/> 
                    <TextBlock Text = ' Likes' Style = '{ThemeResource CaptionTextBlockStyle}'/>
            </StackPanel>
        </StackPanel>
     </Grid>
</DataTemplate>";
                }

                else
                {
                    DisplayDT.Value = @"<!-- TextTemplate: -->
<DataTemplate x:Key='TextTemplate' x:DataType='local1: CustomDataObject'>
    <StackPanel Width = '240' Orientation = 'Horizontal'>
        <TextBlock Style = '{StaticResource TitleTextBlockStyle}' Margin = '8,0,0,0' Text = '{x:Bind Title}'/>
            </StackPanel>
</DataTemplate>";
                }
            }
        }

        private void ContentGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView gridView)
            {
                SelectionOutput.Text = string.Format("You have selected {0} item(s).", gridView.SelectedItems.Count);
            }
        }

        private void ContentGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClickOutput.Text = "You clicked " + (e.ClickedItem as CustomDataObject).Title + ".";
        }

        private void BasicGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClickOutput0.Text = "You clicked " + (e.ClickedItem as CustomDataObject).Title + ".";
        }

        private void ItemClickCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ClickOutput.Text = string.Empty;
        }

        private void FlowDirectionCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ContentGridView.FlowDirection == FlowDirection.LeftToRight)
            {
                ContentGridView.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                ContentGridView.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        private void SelectionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentGridView != null)
            {
                string colorName = e.AddedItems[0].ToString();
                switch (colorName)
                {
                    case "None":
                        ContentGridView.SelectionMode = ListViewSelectionMode.None;
                        SelectionOutput.Text = string.Empty;
                        break;
                    case "Single":
                        ContentGridView.SelectionMode = ListViewSelectionMode.Single;
                        break;
                    case "Multiple":
                        ContentGridView.SelectionMode = ListViewSelectionMode.Multiple;
                        break;
                    case "Extended":
                        ContentGridView.SelectionMode = ListViewSelectionMode.Extended;
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
            ColSpacesDec.IsEnabled = true;

            // Increase left/right margin on all GridViewItems
            for (int i = 0; i < StyledGrid.Items.Count; i++)
            {
                GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                // Create a Thickness property to bind to each item's Margin property.
                // Use existing margin and only change relevant values (left/right)
                Thickness NewMargin = item.Margin;
                NewMargin.Left = item.Margin.Left + 1;
                NewMargin.Right = item.Margin.Right + 1;

                item.Margin = NewMargin;
            }

            NotifyPropertyChanged();
        }

        private void StyledGrid_DecreaseColSpace(object sender, RoutedEventArgs e)
        {
            // Make sure new value is positive
            if (ActualColSpace > 0)
            {
                ColSpacesDec.IsEnabled = true;
                ActualColSpace -= 1;

                // Update text box with newly increased value
                ColSpace.Text = ActualColSpace.ToString();
                ColSpace.PlaceholderText = ActualColSpace.ToString();

                // Increase left/right margin on all GridViewItems
                for (int i = 0; i < StyledGrid.Items.Count; i++)
                {
                    GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                    // Create a Thickness property to bind to each item's Margin property.
                    // Use existing margin and only change relevant values (left/right)
                    Thickness NewMargin = item.Margin;
                    NewMargin.Left = item.Margin.Left - 1;
                    NewMargin.Right = item.Margin.Right - 1;

                    item.Margin = NewMargin;
                }

                // If this action caused RowSpaces to go to 0, deactivate decrease button
                if (ActualColSpace == 0)
                {
                    ColSpacesDec.IsEnabled = false;
                }
            }

            // If new value is negative, reset text box to 0 and display an error message.
            else
            {
                ColSpace.Text = "";
                ColSpace.PlaceholderText = "0";
                ColSpacesDec.IsEnabled = false;
            }

            NotifyPropertyChanged();
        }

        private void StyledGrid_IncreaseRowSpace(object sender, RoutedEventArgs e)
        {
            ActualRowSpace += 1;
            RowSpacesDec.IsEnabled = true;
            RowSpace.Text = ActualRowSpace.ToString();
            RowSpace.PlaceholderText = ActualRowSpace.ToString();

            // Increase top/bottom margin on all GridViewItems
            for (int i = 0; i < StyledGrid.Items.Count; i++)
            {
                GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                // Create a Thickness property to bind to each item's Margin property.
                // Use existing margin and only change relevant values (top/bottom)
                Thickness NewMargin = item.Margin;
                NewMargin.Top = item.Margin.Top + 1;
                NewMargin.Bottom = item.Margin.Bottom + 1;

                item.Margin = NewMargin;
            }

            NotifyPropertyChanged();
        }

        private void StyledGrid_DecreaseRowSpace(object sender, RoutedEventArgs e)
        {
            // Make sure new value is positive
            if (ActualRowSpace > 0)
            {
                ActualRowSpace -= 1;
                RowSpacesDec.IsEnabled = true;

                // Update text box with newly increased value
                RowSpace.Text = ActualRowSpace.ToString();
                RowSpace.PlaceholderText = ActualRowSpace.ToString();

                // Decrease top/bottom margin on all GridViewItems
                for (int i = 0; i < StyledGrid.Items.Count; i++)
                {
                    GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                    // Create a Thickness property to bind to each item's Margin property.
                    // Use existing margin and only change relevant values (top/bottom)
                    Thickness NewMargin = item.Margin;
                    NewMargin.Top = item.Margin.Top - 1;
                    NewMargin.Bottom = item.Margin.Bottom - 1;

                    item.Margin = NewMargin;
                }

                // If this action caused RowSpaces to go to 0, deactivate decrease button
                if (ActualRowSpace == 0)
                {
                    RowSpacesDec.IsEnabled = false;
                }
            }

            // If new value is negative, reset text box to 0 and display an error message.
            else
            {
                RowSpace.Text = "";
                RowSpace.PlaceholderText = "0";
                RowSpacesDec.IsEnabled = false;
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
                    RowSpacesDec.IsEnabled = true;
                    RowSpace.Text = RowSpace.Text.ToString();
                    RowSpace.PlaceholderText = RowSpace.Text.ToString();

                    // Create new Thickness object and update relevant parts of Margin property (top/bottom)
                    for (int i = 0; i < StyledGrid.Items.Count; i++)
                    {
                        GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

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

            // If entered value not a number, reset to 0.
            catch (FormatException)
            {
                // If clearing a text entry, reset margins to 0.
                if (RowSpace.Text == "")
                {
                    for (int i = 0; i < StyledGrid.Items.Count; i++)
                    {
                        GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                        Thickness NewMargin = item.Margin;
                        NewMargin.Top = 0;
                        NewMargin.Bottom = 0;

                        item.Margin = NewMargin;
                    }

                    RowSpacesDec.IsEnabled = false;
                }
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
                    ColSpacesDec.IsEnabled = true;

                    ColSpace.Text = ColSpace.Text.ToString();
                    ColSpace.PlaceholderText = ColSpace.Text.ToString();

                    // Create new Thickness object and update relevant parts of Margin property (top/bottom)
                    for (int i = 0; i < StyledGrid.Items.Count; i++)
                    {
                        GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

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

            // If entered value not a number, reset to 0.
            catch (FormatException)
            {
                // If clearing a text entry, reset margins to 0.
                if (ColSpace.Text == "")
                {
                    for (int i = 0; i < StyledGrid.Items.Count; i++)
                    {
                        GridViewItem item = StyledGrid.ContainerFromIndex(i) as GridViewItem;

                        Thickness NewMargin = item.Margin;
                        NewMargin.Left = 0;
                        NewMargin.Right = 0;

                        item.Margin = NewMargin;
                    }

                    ColSpacesDec.IsEnabled = false;
                }

                ColSpace.Text = "";
                ColSpace.PlaceholderText = "0";
            }
        }

        private void StyledGrid_IncreaseMaxItems(object sender, RoutedEventArgs e)
        {
            // Make sure value is positive
            if (ActualMaxItems == 1 || Convert.ToInt32(MaxItems.Text.ToString()) >= 0)
            {
                MaxItemsDec.IsEnabled = true;
                // Increment ActualMaxItems and update text box
                ActualMaxItems += 1;
                MaxItems.Text = ActualMaxItems.ToString();
                MaxItems.PlaceholderText = ActualMaxItems.ToString();

                // Set property by accessing StyledGridIWG ItemsWrapGrid object, which is defined on load (via StyledGrid_InitWrapGrid fcn)
                StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
            }

        }

        private void StyledGrid_DecreaseMaxItems(object sender, RoutedEventArgs e)
        {
            // Make sure new value is greater than 1
            if (Convert.ToInt32(MaxItems.Text.ToString()) > 1)
            {
                MaxItemsDec.IsEnabled = true;
                // Decrement ActualMaxItems and update text box
                ActualMaxItems -= 1;
                MaxItems.Text = ActualMaxItems.ToString();
                MaxItems.PlaceholderText = ActualMaxItems.ToString();

                // Set property by accessing StyledGridIWG ItemsWrapGrid object, which is defined on load (via StyledGrid_InitWrapGrid fcn)
                StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
            }

            // If value is less than 1, reset textbox to 1 and display error message.
            else
            {
                MaxItemsDec.IsEnabled = false;
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

                // Make sure number is not less than 1
                if (Convert.ToInt32(MaxItems.Text.ToString()) > 1)
                {
                    MaxItemsDec.IsEnabled = true;
                    MaxItems.PlaceholderText = MaxItems.Text.ToString();
                    ActualMaxItems = Convert.ToInt32(MaxItems.Text);


                    // Set property by accessing StyledGridIWG ItemsWrapGrid object, which is defined on load (via StyledGrid_InitWrapGrid fcn)
                    StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
                }

                // If number is less than 1, reset textbox to 1 and display error message.
                else
                {
                    MaxItemsDec.IsEnabled = false;
                    ActualMaxItems = 1;
                    MaxItems.PlaceholderText = "1";
                    MaxItems.Text = "1";
                    StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
                }

            }

            // If value not a number, reset text box to 1.
            catch (FormatException)
            {
                if (MaxItems.Text == "")
                {
                    StyledGridIWG.MaximumRowsOrColumns = 1;
                    MaxItemsDec.IsEnabled = false;
                }
                MaxItems.Text = "";
                MaxItems.PlaceholderText = "1";
                ActualMaxItems = 1;
            }

        }

        private void StyledGrid_InitWrapGrid(object sender, RoutedEventArgs e)
        {
            // Update ItemsWrapGrid object created on page load by assigning it to StyledGrid's ItemWrapGrid
            StyledGridIWG = sender as ItemsWrapGrid;

            // Now we can change StyledGrid's MaximumRowsorColumns property within its ItemsPanel>ItemsPanelTemplate>ItemsWrapGrid.
            StyledGridIWG.MaximumRowsOrColumns = ActualMaxItems;
        }
    }
}
