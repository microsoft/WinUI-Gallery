// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class DatePickerPage : Page
{
    public DatePickerPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        // Set the default date to 2 months from the current date.
        Control2.Date = DateTimeOffset.Now.AddMonths(2);

        // Set the minimum year to the current year.
        Control2.MinYear = DateTimeOffset.Now;

        // Set the maximum year to 5 years in the future.
        Control2.MaxYear = DateTimeOffset.Now.AddYears(5);

    }
}
