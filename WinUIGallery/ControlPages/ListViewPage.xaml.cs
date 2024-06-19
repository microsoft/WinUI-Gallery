//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUIGallery.Common;
using WinUIGallery.Data;
using WinUIGallery.Helper;

namespace WinUIGallery.ControlPages
{
    public sealed partial class ListViewPage : ItemsPageBase
    {
        ObservableCollection<Contact> contacts1 = new ObservableCollection<Contact>();
        ObservableCollection<Contact> contacts2 = new ObservableCollection<Contact>();
        ObservableCollection<Contact> contacts3 = new ObservableCollection<Contact>();
        ObservableCollection<Contact> contacts3Filtered = new ObservableCollection<Contact>();
        ObservableCollection<Contact> contacts4ContextMenu = new ObservableCollection<Contact>();

        ItemsStackPanel stackPanelObj;

        int messageNumber;

        public ListViewPage()
        {
            this.InitializeComponent();
            // Add first item to inverted list so it's not empty
            AddItemToEnd();
            BaseExample.Loaded += BaseExample_Loaded;
        }

        private void BaseExample_Loaded(object sender, RoutedEventArgs e)
        {
            // Set focus so the first item of the listview has focus
            // instead of some item which is not visible on page load
            BaseExample.Focus(FocusState.Programmatic);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Items = ControlInfoDataSource.Instance.Groups.Take(3).SelectMany(g => g.Items).ToList();
            BaseExample.ItemsSource = await Contact.GetContactsAsync();
            Control2.ItemsSource = await Contact.GetContactsAsync();
            contacts1 = await Contact.GetContactsAsync();

            DragDropListView.ItemsSource = contacts1;

            contacts2.Add(new Contact("John", "Doe", "ABC Printers"));
            contacts2.Add(new Contact("Jane", "Doe", "XYZ Refrigerators"));
            contacts2.Add(new Contact("Santa", "Claus", "North Pole Toy Factory Inc."));
            DragDropListView2.ItemsSource = contacts2;

            Control4.ItemsSource = WinUIGallery.ControlPages.CustomDataObject.GetDataObjects();
            ContactsCVS.Source = await Contact.GetContactsGroupedAsync();

            // Initialize list of contacts to be filtered
            contacts3 = await Contact.GetContactsAsync();
            contacts3Filtered = new ObservableCollection<Contact>(contacts3);

            // Initializze list of contacts for context menu sample
            contacts4ContextMenu = await Contact.GetContactsAsync();
            ContextMenuList.ItemsSource = contacts4ContextMenu;

            FilteredListView.ItemsSource = contacts3Filtered;
        }

        //===================================================================================================================
        // Selection Modes Example
        //===================================================================================================================
        private void SelectionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Control2 != null)
            {
                string selectionMode = e.AddedItems[0].ToString();
                switch (selectionMode)
                {
                    case "None":
                        Control2.SelectionMode = ListViewSelectionMode.None;
                        break;
                    case "Single":
                        Control2.SelectionMode = ListViewSelectionMode.Single;
                        break;
                    case "Multiple":
                        Control2.SelectionMode = ListViewSelectionMode.Multiple;
                        break;
                    case "Extended":
                        Control2.SelectionMode = ListViewSelectionMode.Extended;
                        break;
                }
            }
        }

        //===================================================================================================================
        // Drag/Drop Example
        //===================================================================================================================

        private void Source_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // Prepare a string with one dragged item per line
            StringBuilder items = new StringBuilder();
            foreach (Contact item in e.Items)
            {
                if (items.Length > 0) { items.AppendLine(); }
                if (item.ToString() != null)
                {
                    // Append name from contact object onto data string
                    items.Append(item.ToString() + " " + item.Company);
                }
            }
            // Set the content of the DataPackage
            e.Data.SetText(items.ToString());

            e.Data.RequestedOperation = DataPackageOperation.Move;

        }

        private void Target_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void Source_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private async void ListView_Drop(object sender, DragEventArgs e)
        {
            ListView target = (ListView)sender;

            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                DragOperationDeferral def = e.GetDeferral();
                string s = await e.DataView.GetTextAsync();
                string[] items = s.Split('\n');
                foreach (string item in items)
                {

                    // Create Contact object from string, add to existing target ListView
                    string[] info = item.Split(" ", 3);
                    Contact temp = new Contact(info[0], info[1], info[2]);

                    // Find the insertion index:
                    Windows.Foundation.Point pos = e.GetPosition(target.ItemsPanelRoot);

                    // If the target ListView has items in it, use the height of the first item
                    //      to find the insertion index.
                    int index = 0;
                    if (target.Items.Count != 0)
                    {
                        // Get a reference to the first item in the ListView
                        ListViewItem sampleItem = (ListViewItem)target.ContainerFromIndex(0);

                        // Adjust itemHeight for margins
                        double itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

                        // Find index based on dividing number of items by height of each item
                        index = Math.Min(target.Items.Count - 1, (int)(pos.Y / itemHeight));

                        // Find the item being dropped on top of.
                        ListViewItem targetItem = (ListViewItem)target.ContainerFromIndex(index);

                        // If the drop position is more than half-way down the item being dropped on
                        //      top of, increment the insertion index so the dropped item is inserted
                        //      below instead of above the item being dropped on top of.
                        Windows.Foundation.Point positionInItem = e.GetPosition(targetItem);
                        if (positionInItem.Y > itemHeight / 2)
                        {
                            index++;
                        }

                        // Don't go out of bounds
                        index = Math.Min(target.Items.Count, index);
                    }
                    // Only other case is if the target ListView has no items (the dropped item will be
                    //      the first). In that case, the insertion index will remain zero.

                    // Find correct source list
                    if (target.Name == "DragDropListView")
                    {
                        // Find the ItemsSource for the target ListView and insert
                        contacts1.Insert(index, temp);
                        //Go through source list and remove the items that are being moved
                        foreach (Contact contact in DragDropListView2.Items)
                        {
                            if (contact.FirstName == temp.FirstName && contact.LastName == temp.LastName && contact.Company == temp.Company)
                            {
                                contacts2.Remove(contact);
                                break;
                            }
                        }
                    }
                    else if (target.Name == "DragDropListView2")
                    {
                        contacts2.Insert(index, temp);
                        foreach (Contact contact in DragDropListView.Items)
                        {
                            if (contact.FirstName == temp.FirstName && contact.LastName == temp.LastName && contact.Company == temp.Company)
                            {
                                contacts1.Remove(contact);
                                break;
                            }
                        }
                    }
                }

                e.AcceptedOperation = DataPackageOperation.Move;
                def.Complete();
            }
        }

        private void Target_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items.Count == 1)
            {
                // Prepare ListViewItem to be moved
                Contact tmp = (Contact)e.Items[0];

                e.Data.SetText(tmp.FirstName + " " + tmp.LastName + " " + tmp.Company);
                e.Data.RequestedOperation = DataPackageOperation.Move;
            }
        }

        private void Target_DragEnter(object sender, DragEventArgs e)
        {
            // We don't want to show the Move icon
            e.DragUIOverride.IsGlyphVisible = false;
        }

        //===================================================================================================================
        // Grouped Headers Example
        //===================================================================================================================
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (StickySwitch != null)
            {
                if (StickySwitch.IsOn == true)
                {
                    stackPanelObj.AreStickyGroupHeadersEnabled = true;
                }
                else
                {
                    stackPanelObj.AreStickyGroupHeadersEnabled = false;
                }
            }
        }

        private void StackPanel_loaded(object sender, RoutedEventArgs e)
        {
            stackPanelObj = sender as ItemsStackPanel;
        }

        //===================================================================================================================
        // Filtered List Example
        //===================================================================================================================
        private void Remove_NonMatching(IEnumerable<Contact> filteredData)
        {
            for (int i = contacts3Filtered.Count - 1; i >= 0; i--)
            {
                var item = contacts3Filtered[i];
                // If contact is not in the filtered argument list, remove it from the ListView's source.
                if (!filteredData.Contains(item))
                {
                    contacts3Filtered.Remove(item);
                }
            }
        }

        private void AddBack_Contacts(IEnumerable<Contact> filteredData)
        // When a user hits backspace, more contacts may need to be added back into the list
        {
            foreach (var item in filteredData)
            {
                // If item in filtered list is not currently in ListView's source collection, add it back in
                if (!contacts3Filtered.Contains(item))
                {
                    contacts3Filtered.Add(item);
                }
            }
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs args)
        {
            // Linq query that selects only items that return True after being passed through Filter function
            var filtered = contacts3.Where(contact => Filter(contact));
            Remove_NonMatching(filtered);
            AddBack_Contacts(filtered);

            UIHelper.AnnounceActionForAccessibility(FilteredListView, $"Found {filtered.Count()} contacts", "ContactListViewFilteredActivityId");
        }

        private bool Filter(Contact contact)
        {
            // When the text in any filter is changed, contact list is ran through all three filters to make sure
            // they can properly interact with each other (i.e. they can all be applied at the same time).

            return contact.FirstName.Contains(FilterByFirstName.Text, StringComparison.InvariantCultureIgnoreCase) &&
                   contact.LastName.Contains(FilterByLastName.Text, StringComparison.InvariantCultureIgnoreCase) &&
                   contact.Company.Contains(FilterByCompany.Text, StringComparison.InvariantCultureIgnoreCase);
        }

        //===================================================================================================================
        // Inverted List Example
        //===================================================================================================================

        private void AddItemToEnd()
        {
            InvertedListView.Items.Add(
                new Message("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Right)
                );
        }

        private void MessageReceived(object sender, RoutedEventArgs e)
        {
            InvertedListView.Items.Add(
                new Message("Message " + ++messageNumber, DateTime.Now, HorizontalAlignment.Left)
                );
        }

        //===================================================================================================================
        // ListView with Images Sample
        //===================================================================================================================

        private void TextBlock_IsTextTrimmedChanged(TextBlock sender, IsTextTrimmedChangedEventArgs args)
        {
            var textBlock = sender as TextBlock;
            var text = textBlock.IsTextTrimmed ? textBlock.Text : string.Empty;

            ToolTipService.SetToolTip(textBlock, text);
        }


        //===================================================================================================================
        // ListView with context menu
        //===================================================================================================================

        private void ContactDeleteMenuyItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var contact = item as Contact;
            contacts4ContextMenu.Remove(contact);
        }
    }

    public class Message
    {
        public string MsgText { get; private set; }
        public DateTime MsgDateTime { get; private set; }
        public HorizontalAlignment MsgAlignment { get; set; }
        public Message(string text, DateTime dateTime, HorizontalAlignment align)
        {
            MsgText = text;
            MsgDateTime = dateTime;
            MsgAlignment = align;
        }

        public override string ToString()
        {
            return MsgDateTime.ToString() + " " + MsgText;
        }
    }

    public class Contact
    {
        #region Properties
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Company { get; private set; }
        public string Name => FirstName + " " + LastName;
        #endregion

        public Contact(string firstName, string lastName, string company)
        {
            FirstName = firstName;
            LastName = lastName;
            Company = company;
        }

        #region Public Methods
        public async static Task<ObservableCollection<Contact>> GetContactsAsync()
        {
            IList<string> lines = await FileLoader.LoadLines("Assets/Contacts.txt");

            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < lines.Count - 2; i += 3)
            {
                contacts.Add(new Contact(lines[i], lines[i + 1], lines[i + 2]));
            }

            return contacts;
        }

        public static async Task<ObservableCollection<GroupInfoList>> GetContactsGroupedAsync()
        {
            var query = from item in await GetContactsAsync()
                        group item by item.LastName.Substring(0, 1).ToUpper() into g
                        orderby g.Key
                        select new GroupInfoList(g) { Key = g.Key };

            return new ObservableCollection<GroupInfoList>(query);
        }

        public override string ToString()
        {
            return $"{Name}, {Company}";
        }
        #endregion
    }

    public class GroupInfoList : List<object>
    {
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        }
        public object Key { get; set; }

        public override string ToString()
        {
            return "Group " + Key.ToString();
        }
    }
}
