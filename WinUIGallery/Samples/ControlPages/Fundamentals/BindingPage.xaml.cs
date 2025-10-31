// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

namespace WinUIGallery.ControlPages;

public sealed partial class BindingPage : Page
{
    public string GreetingMessage { get; set; } = "Hello, WinUI 3!";

    public ExampleViewModel ViewModel { get; set; }

    public BindingPage()
    {
        this.InitializeComponent();

        ViewModel = new ExampleViewModel
        {
            Title = "Welcome to WinUI 3",
            Description = "This is an example of binding to a view model.",
            NullString = null
        };
        DataContext = ViewModel;
    }

    public string FormatDate(DateTimeOffset? date)
    {
        if (date.HasValue)
        {
            return "Selected date is: " + date.Value.ToString("dddd, MMMM d, yyyy");
        }
        else
        {
            return "No date selected";
        }
    }
}

public partial class ExampleViewModel : INotifyPropertyChanged
{
    private string _title;
    private string _description;
    private string _nullString;

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public string NullString
    {
        get => _nullString;
        set
        {
            if (_nullString != value)
            {
                _nullString = value;
                OnPropertyChanged(nameof(_nullString));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
