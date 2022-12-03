//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace AppUIBasics.ControlPages
{
    public sealed partial class FlyoutPage : Page
    {
        public FlyoutPage()
        {
            this.InitializeComponent();
        }

        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            if (this.Control1.Flyout is Flyout f)
            {
                f.Hide();
            }
        }

        private FlyoutShowMode _showMode = FlyoutShowMode.Auto;
        public FlyoutShowMode ShowMode
        {
            get => _showMode;
            set
            {
                CustomFlyout.ShowMode = value;
                _showMode = value;
            }
        }

        private void FlyoutShowModeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (e.AddedItems[0].ToString())
            {
                case "Auto": ShowMode = FlyoutShowMode.Auto; break;
                case "Standard": ShowMode = FlyoutShowMode.Standard; break;
                case "Transient": ShowMode = FlyoutShowMode.Transient; break;
                default: ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway;
                    break;
            }

        }
    }
}
