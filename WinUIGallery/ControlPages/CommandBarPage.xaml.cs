//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class CommandBarPage : Page, INotifyPropertyChanged
    {
        private bool multipleButtons = false;

        public bool MultipleButtons
        {
            get
            {
                return multipleButtons;
            }
            set
            {
                multipleButtons = value;
                OnPropertyChanged("MultipleButtons");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public CommandBarPage()
        {
            this.InitializeComponent();
            AddKeyboardAccelerators();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            PrimaryCommandBar.IsOpen = true;
            PrimaryCommandBar.IsSticky = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            PrimaryCommandBar.IsOpen = false;
            PrimaryCommandBar.IsSticky = false;
        }

        private void OnElementClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var selectedFlyoutItem = sender as AppBarButton;
            SelectedOptionText.Text = "You clicked: " + (sender as AppBarButton).Label;
        }

        private void AddSecondaryCommands_Click(object sender, RoutedEventArgs e)
        {
            // Add compact button to the command bar. It provides functionality specific
            // to this page, and is removed when leaving the page.

            if (PrimaryCommandBar.SecondaryCommands.Count == 1)
            {
                var newButton = new AppBarButton
                {
                    Icon = new SymbolIcon(Symbol.Add),
                    Label = "Button 1"
                };
                newButton.KeyboardAccelerators.Add(new Windows.UI.Xaml.Input.KeyboardAccelerator()
                {
                    Key = Windows.System.VirtualKey.N,
                    Modifiers = Windows.System.VirtualKeyModifiers.Control
                });
                PrimaryCommandBar.SecondaryCommands.Add(newButton);

                newButton = new AppBarButton
                {
                    Icon = new SymbolIcon(Symbol.Delete),
                    Label = "Button 2"
                };
                PrimaryCommandBar.SecondaryCommands.Add(newButton);
                newButton.KeyboardAccelerators.Add(new Windows.UI.Xaml.Input.KeyboardAccelerator()
                {
                    Key = Windows.System.VirtualKey.Delete
                });
                PrimaryCommandBar.SecondaryCommands.Add(new AppBarSeparator());

                newButton = new AppBarButton
                {
                    Icon = new SymbolIcon(Symbol.FontDecrease),
                    Label = "Button 3"
                };
                newButton.KeyboardAccelerators.Add(new Windows.UI.Xaml.Input.KeyboardAccelerator()
                {
                    Key = Windows.System.VirtualKey.Subtract,
                    Modifiers = Windows.System.VirtualKeyModifiers.Control
                });
                PrimaryCommandBar.SecondaryCommands.Add(newButton);

                newButton = new AppBarButton
                {
                    Icon = new SymbolIcon(Symbol.FontIncrease),
                    Label = "Button 4"
                };
                newButton.KeyboardAccelerators.Add(new Windows.UI.Xaml.Input.KeyboardAccelerator()
                {
                    Key = Windows.System.VirtualKey.Add,
                    Modifiers = Windows.System.VirtualKeyModifiers.Control
                });
                PrimaryCommandBar.SecondaryCommands.Add(newButton);

            }
            MultipleButtons = true;
        }

        private void RemoveSecondaryCommands_Click(object sender, RoutedEventArgs e)
        {
            RemoveSecondaryCommands();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            RemoveSecondaryCommands();
            base.OnNavigatingFrom(e);
        }

        private void RemoveSecondaryCommands()
        {
            while (PrimaryCommandBar.SecondaryCommands.Count > 1)
            {
                PrimaryCommandBar.SecondaryCommands.RemoveAt(PrimaryCommandBar.SecondaryCommands.Count - 1);
            }
            MultipleButtons = false;
        }


        private void AddKeyboardAccelerators()
        {
            editButton.KeyboardAccelerators.Add(new Windows.UI.Xaml.Input.KeyboardAccelerator()
            {
                Key = Windows.System.VirtualKey.E,
                Modifiers = Windows.System.VirtualKeyModifiers.Control
            });

            shareButton.KeyboardAccelerators.Add(new Windows.UI.Xaml.Input.KeyboardAccelerator()
            {
                Key = Windows.System.VirtualKey.F4
            });

            addButton.KeyboardAccelerators.Add(new Windows.UI.Xaml.Input.KeyboardAccelerator()
            {
                Key = Windows.System.VirtualKey.A,
                Modifiers = Windows.System.VirtualKeyModifiers.Control
            });

        }

    }
}
