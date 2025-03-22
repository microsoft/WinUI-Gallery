using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace WinUIGallery.ControlPages;

public sealed partial class ContentDialogPage : Page
{
    public ContentDialogPage()
    {
        InitializeComponent();
    }

    private async void ShowDialog_Click(object sender, RoutedEventArgs e)
    {
        ContentDialogExample dialog = new()
        {
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Save your work?",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Don't Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = new ContentDialogContent(),
            RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            DialogResult.Text = "User saved their work";
        }
        else if (result == ContentDialogResult.Secondary)
        {
            DialogResult.Text = "User did not save their work";
        }
        else
        {
            DialogResult.Text = "User cancelled the dialog";
        }
    }
}
