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
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
#if !AB_BUILD
using WinUIGallery.Shaders;
#endif // #if !AB_BUILD
using System.Numerics;

namespace WinUIGallery.ControlPages
{
    public sealed partial class ContentDialogPage : Page
    {
        public ContentDialogPage()
        {
            this.InitializeComponent();
        }

        private async void ShowDialog_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogExample dialog = new ContentDialogExample();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Save your work?";
            dialog.PrimaryButtonText = "Save";
            dialog.SecondaryButtonText = "Don't Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new ContentDialogContent();
            dialog.RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme;
            dialog.Closing += Dialog_Closing;

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

        private async void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
#if !AB_BUILD
            if (SettingsPage.computeSharpAnimationState != SettingsPage.ComputeSharpAnimationState.NONE)
            {
                // Get a deferral until the shader starts rendering.
                // This keeps the dialog open until the capture is complete.
                var deferral = args.GetDeferral();

                // Capture the dialog to our bitmap and get the dialog dimensions.
                var dialogRect = await sender.CaptureTo(m_bitmap);

                // Calculate offset from Window root to the overlay panel
                var transform = XamlRoot.Content.TransformToVisual(overlayPanel);
                var overlayOffset = transform.TransformPoint(new Point(0, 0));

                // Create our shader panel which will run "TwirlDismiss" on the dialog capture.
                var dialogShaderPanel = new ShaderPanel();
                dialogShaderPanel.InitializeForShader<TwirlDismiss>();
                dialogShaderPanel.Width = dialogRect.Width;
                dialogShaderPanel.Height = dialogRect.Height;
                dialogShaderPanel.Translation = new Vector3(
                    (float)overlayOffset.X,
                    (float)overlayOffset.Y,
                    0);

                await dialogShaderPanel.SetShaderInputAsync(m_bitmap);

                // Display the shader panel by adding it as an overlay.
                Point offset = new() { X = dialogRect.X, Y = dialogRect.Y };
                overlayPanel.AddOverlay(dialogShaderPanel, offset);

                // Close the dialog once the shader starts running, and remove the shader panel when
                // it's done.
                dialogShaderPanel.FirstRender += (s, e) => deferral.Complete();
                dialogShaderPanel.ShaderCompleted += (s, e) => overlayPanel.ClearOverlay(dialogShaderPanel);
            }
#else
            await Task.CompletedTask;
#endif // #if !AB_BUILD
        }

#if !AB_BUILD
        // The bitmap that holds the screen capture of the dialog so we can run shaders on it.
        private RenderTargetBitmap m_bitmap = new RenderTargetBitmap();
#endif // #if !AB_BUILD
    }
}

