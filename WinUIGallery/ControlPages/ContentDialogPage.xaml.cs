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
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

namespace WinUIGallery.ControlPages
{
    public sealed partial class ContentDialogPage : Page
    {
        private ContentDialogExample _dialog;
        private ContentDialogContent _content;
        private RenderTargetBitmap _mainRTB;
        private RenderTargetBitmap _contentDialogRTB;

        public ContentDialogPage()
        {
            this.InitializeComponent();
        }

        private async void ShowDialog_Click(object sender, RoutedEventArgs e)
        {
            _dialog = new ContentDialogExample();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            _dialog.XamlRoot = this.XamlRoot;
            _dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            _dialog.Title = "Save your work?";
            _dialog.PrimaryButtonText = "Save";
            _dialog.SecondaryButtonText = "Don't Save";
            _dialog.CloseButtonText = "Cancel";
            _dialog.DefaultButton = ContentDialogButton.Primary;
            _content = new ContentDialogContent();
            _content.Loaded += Content_Loaded;
            _dialog.Content = _content;
            _dialog.RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme;

            _mainRTB = new RenderTargetBitmap();
            UIElement root = this;
            UIElement parent = VisualTreeHelper.GetParent(root) as UIElement;
            while (parent != null)
            {
                root = parent;
                parent = VisualTreeHelper.GetParent(root) as UIElement;
            }
            await _mainRTB.RenderAsync(root);

            _dialog.Closing += Dialog_Closing;

            var result = await _dialog.ShowAsync();

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

        private void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            stackPanel.Children.Add(new TextBlock() { Text = "Main tree" });
            stackPanel.Children.Add(new Image() { Source = _mainRTB, });

            stackPanel.Children.Add(new TextBlock() { Text = "ContentDialog", Margin = new Thickness(0, 40, 0, 0) });
            stackPanel.Children.Add(new Image() { Source = _contentDialogRTB, });
        }

        private async void Content_Loaded(object sender, RoutedEventArgs e)
        {
            _contentDialogRTB = new RenderTargetBitmap();
            await _contentDialogRTB.RenderAsync(_dialog);
        }
    }
}
