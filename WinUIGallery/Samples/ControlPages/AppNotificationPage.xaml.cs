// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;

namespace WinUIGallery.ControlPages;

public sealed partial class AppNotificationPage : Page
{
    private IReadOnlyList<AppNotificationSoundEvent> appNotificationSoundEvents = new List<AppNotificationSoundEvent>
    {
        AppNotificationSoundEvent.Default,
        AppNotificationSoundEvent.IM,
        AppNotificationSoundEvent.Reminder,
        AppNotificationSoundEvent.SMS,
        AppNotificationSoundEvent.Alarm,
        AppNotificationSoundEvent.Call
    };
    private AppNotificationSoundEvent selectedAppNotificationSoundEvent = AppNotificationSoundEvent.Default;

    public AppNotificationPage()
    {
        this.InitializeComponent();
    }

    private void ShowInformationalNotificationWithLogoButton_Click(object sender, RoutedEventArgs e)
    {
        AppNotification notification = new AppNotificationBuilder()
            .AddText("Control Highlight: PersonPicture")
            .AddText("Use the PersonPicture control to display user avatars with initials or images.")
            .SetAppLogoOverride(new Uri("ms-appx:///Assets/ControlImages/PersonPicture.png"), AppNotificationImageCrop.Circle)
            .SetAudioEvent(selectedAppNotificationSoundEvent)
            .SetTimeStamp(DateTime.Now)
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }

    private void ShowVisualNotificationWithHeroImageButton_Click(object sender, RoutedEventArgs e)
    {
        AppNotification notification = new AppNotificationBuilder()
            .AddText("Harbor Scene with Boats")
            .AddText("A quiet harbor with boats gently anchored in view.")
            .SetHeroImage(new Uri("ms-appx:///Assets/SampleMedia/LandscapeImage5.jpg"))
            .SetAttributionText("WinUI gallery assets")
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }

    private void ShowNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        AppNotification notification = new AppNotificationBuilder()
            .AddText("Welcome to WinUI 3 Gallery")
            .AddText("Explore interactive samples and discover the power of modern Windows UI.")
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }

    private void ShowNotificationWithControlsButton_Click(object sender, RoutedEventArgs e)
    {
        AppNotification notification = new AppNotificationBuilder()
            .AddText("Survey")
            .AddText("Please select your satisfaction level and leave a comment.")
            .AddComboBox(new AppNotificationComboBox("satisfaction")
                .AddItem("1", "Very Bad")
                .AddItem("2", "Bad")
                .AddItem("3", "Neutral")
                .AddItem("4", "Good")
                .AddItem("5", "Excellent")
                .SetSelectedItem("3"))
            .AddTextBox("comment", "Leave a comment here...", "")
            .AddButton(new AppNotificationButton("Submit")
                .AddArgument("action", "submit_survey"))
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }

    private void ShowNotificationWithProgressBarButton_Click(object sender, RoutedEventArgs e)
    {
        AppNotification notification = new AppNotificationBuilder()
            .AddText("Progress Bar Example")
            .AddText("This is a sample notification showing how to use a progress bar.")
            .AddProgressBar(new AppNotificationProgressBar()
            {
                Title = "Demo Progress",
                Value = 0.6, // 60%
                ValueStringOverride = "60%",
                Status = "In progress..."
            })
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }
}
