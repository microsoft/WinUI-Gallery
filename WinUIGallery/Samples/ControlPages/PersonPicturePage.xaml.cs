// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class PersonPicturePage : Page
{
    public PersonPicturePage()
    {
        this.InitializeComponent();
    }

    private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProfileImageRadio.IsChecked == true)
        {
            personPicture.ProfilePicture = new BitmapImage(new Uri("https://learn.microsoft.com/windows/uwp/contacts-and-calendar/images/shoulder-tap-static-payload.png"));
            personPicture.DisplayName = null;
            personPicture.Initials = null;
        }
        else if (DisplayNameRadio.IsChecked == true)
        {
            personPicture.ProfilePicture = null;
            personPicture.DisplayName = "Jane Doe";
            personPicture.Initials = null;
        }
        else if (InitialsRadio.IsChecked == true)
        {
            personPicture.ProfilePicture = null;
            personPicture.DisplayName = null;
            personPicture.Initials = "SB";
        }
    }
}
