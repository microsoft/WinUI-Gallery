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
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
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
}
