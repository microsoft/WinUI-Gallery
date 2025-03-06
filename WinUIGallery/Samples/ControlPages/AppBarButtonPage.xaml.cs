//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using WinUIGallery.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;

namespace WinUIGallery.ControlPages;

public sealed partial class AppBarButtonPage : Page
{
    public AppBarButtonPage()
    {
        this.InitializeComponent();
    }

    private void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button b)
        {
            string name = b.Name;

            switch (name)
            {
                case "Button1":
                    Control1Output.Text = "You clicked: " + name;
                    UIHelper.AnnounceActionForAccessibility(Button1, Control1Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button2":
                    Control2Output.Text = "You clicked: " + name;
                    UIHelper.AnnounceActionForAccessibility(Button2, Control2Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button3":
                    Control3Output.Text = "You clicked: " + name;
                    UIHelper.AnnounceActionForAccessibility(Button3, Control3Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button4":
                    Control4Output.Text = "You clicked: " + name;
                    UIHelper.AnnounceActionForAccessibility(Button4, Control4Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
                case "Button5":
                    Control5Output.Text = "You clicked: " + name;
                    UIHelper.AnnounceActionForAccessibility(Button5, Control5Output.Text, "AppBarButtonSuccessNotificationId");
                    break;
            }
        }
    }
}
