//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ListViewPage : ItemsPageBase
    {
        ObservableCollection<Contact> contacts1 = new ObservableCollection<Contact>();
        ObservableCollection<Contact> contacts2 = new ObservableCollection<Contact>();

        ItemsStackPanel stackPanelObj;

        int messageNumber;

        public ListViewPage()
        {
            this.InitializeComponent();
            // Add first item to inverted list so it's not empty
            AddItemToEnd();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Items = ControlInfoDataSource.Instance.Groups.Take(3).SelectMany(g => g.Items).ToList();
            BaseExample.ItemsSource = await Contact.GetContactsAsync();
            Control2.ItemsSource = await Contact.GetContactsAsync();
            contacts1 = await Contact.GetContactsAsync();

            DragDropListView.ItemsSource = contacts1;

            contacts2.Add(new Contact("John", "Doe", "ABC Printers"));
            contacts2.Add(new Contact("Jane", "Doe", "XYZ Refridgerators"));
            contacts2.Add(new Contact("Santa", "Claus", "North Pole Toy Factory Inc."));
            DragDropListView2.ItemsSource = contacts2;

            Control4.ItemsSource = AppUIBasics.ControlPages.CustomDataObject.GetDataObjects();
            ContactsCVS.Source = await Contact.GetContactsGroupedAsync();
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
            var items = new StringBuilder();
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

        private async void Target_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {

                var def = e.GetDeferral();
                var s = await e.DataView.GetTextAsync();
                var items = s.Split('\n');
                foreach (var item in items)
                {

                    // Create Contact object from string, add to existing target ListView
                    var info = item.Split(" ", 3);
                    Contact temp = new Contact(info[0], info[1], info[2]);
                    
                    // Find the insertion index:
                    Windows.Foundation.Point pos = e.GetPosition(DragDropListView2.ItemsPanelRoot);

                    // Find height of a random item from the target list view
                    ListViewItem sampleItem = (ListViewItem)DragDropListView.ContainerFromIndex(0);
                    double itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

                    // Find index based on dividing number of items by height of each item
                    int index = Math.Min(DragDropListView2.Items.Count - 1, (int)(pos.Y / itemHeight));

                    // Find the item that we want to drop
                    ListViewItem targetItem = (ListViewItem)DragDropListView2.ContainerFromIndex(index); ;

                    // Figure out if to insert above or below
                    Windows.Foundation.Point positionInItem = e.GetPosition(targetItem);
                    if (positionInItem.Y > itemHeight / 2)
                    {
                        index++;
                    }

                    // Don't go out of bounds
                    index = Math.Min(DragDropListView2.Items.Count, index);

                    // Finally, add to target listview
                    contacts2.Insert(index, temp);

                    // Go through source list and remove the items that are being moved
                    foreach (Contact contact in DragDropListView.Items)
                    {
                        if (contact.FirstName == temp.FirstName && contact.LastName == temp.LastName && contact.Company == temp.Company)
                        {
                            contacts1.Remove(contact);
                            break;
                        }
                    }
                }
                
                e.AcceptedOperation = DataPackageOperation.Move;
                def.Complete();
            }
        }

        private async void Source_Drop(object sender, DragEventArgs e)
        {

            var def = e.GetDeferral();
            var s = await e.DataView.GetTextAsync();
            var items = s.Split('\n');
            foreach (var item in items)
            {
                var info = item.Split(" ", 3);

                // Create Contact object from string, add to existing target ListView
                Contact temp = new Contact(info[0], info[1], info[2]);

                // Find the insertion index
                Windows.Foundation.Point pos = e.GetPosition(DragDropListView.ItemsPanelRoot);

                // Find height of a random item from the target list view
                ListViewItem sampleItem = (ListViewItem)DragDropListView.ContainerFromIndex(0);
                double itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

                // Find index based on dividing number of items by height of each item
                int index = Math.Min(DragDropListView.Items.Count - 1, (int)(pos.Y / itemHeight));

                // Find the item that we want to drop
                ListViewItem targetItem = (ListViewItem)DragDropListView.ContainerFromIndex(index); ;

                // Figure out if to insert above or below
                Windows.Foundation.Point positionInItem = e.GetPosition(targetItem);
                if (positionInItem.Y > itemHeight / 2)
                {
                    index++;
                }

                // Don't go out of bounds
                index = Math.Min(DragDropListView.Items.Count, index);

                // Finally, add to target listview
                contacts1.Insert(index, temp);

                // Go through source list and remove the items that are being moved
                foreach (Contact contact in DragDropListView2.Items)
                {
                    if (contact.FirstName == temp.FirstName && contact.LastName == temp.LastName && contact.Company == temp.Company)
                    {
                        contacts2.Remove(contact);
                        break;
                    }
                }
            }

            e.AcceptedOperation = DataPackageOperation.Move;
            def.Complete();
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
        // Inverted List Example
        //===================================================================================================================

        private void AddItemToEnd()
        {

            InvertedListView.Items.Add(
                new Message("Message " + ++messageNumber, DateTime.Now.ToString(), "Right")
                );
        }

        private void MessageRecieved(object sender, RoutedEventArgs e)
        {
            InvertedListView.Items.Add(
                new Message("Message " + ++messageNumber, DateTime.Now.ToString(), "Left")
                );
        }
    }

    public class Message
    {
        public string MsgText { get; private set; }
        public string MsgDateTime { get; private set; }
        public string MsgAlignment { get; set; }
        public string BgColor { get; set; }
        public Message(string text, string datetime, string align)
        {
            MsgText = text;
            MsgDateTime = datetime;
            MsgAlignment = align;

            // If recieved message, use yellow background
            if (MsgAlignment == "Left")
            {
                BgColor = "#f5d471";
            }

            // If sent message, use light gray
            else
            {
                BgColor = "#c7c6c5";
            }
        }

        public override string ToString()
        {
            return MsgDateTime + " " + MsgText;
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
        public static async Task<ObservableCollection<Contact>> GetContactsAsync()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Contacts.txt"));
            IList<string> lines = await FileIO.ReadLinesAsync(file);

            var contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < lines.Count; i += 3)
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
            return Name;
        }
        #endregion
    }

    public class GroupInfoList : List<object>
    {
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        }
        public object Key { get; set; }
    }
}
