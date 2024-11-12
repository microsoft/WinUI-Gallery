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
using WinUIGallery.Shaders;
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
            if (SettingsPage.computeSharpAnimationState != SettingsPage.ComputeSharpAnimationState.NONE)
            {
                // Get a deferral until the shader starts rendering.
                var deferral = args.GetDeferral();

                m_dialogRect = await sender.CaptureTo(m_bitmap);

                // Calculate offset from Window root to the overlay element
                var transform = this.XamlRoot.Content.TransformToVisual(overlayPanel);
                var overlayOffset = transform.TransformPoint(new Point(0, 0));
                var dialogShaderPanel = new ShaderPanel();
                dialogShaderPanel.InitializeForShader<TwirlDismiss>();
                dialogShaderPanel.Translation = new Vector3((float)overlayOffset.X, (float)overlayOffset.Y, 0);
                dialogShaderPanel.Width = m_dialogRect.Width;
                dialogShaderPanel.Height = m_dialogRect.Height;

                // Offset from the overlay element to the dialog
                Point offset = new() { X = m_dialogRect.X, Y = m_dialogRect.Y };

                // We need to do some shenanigans because the render actually happens on a background thread,
                // which is where the event gets fired.
                var dispatcher = DispatcherQueue;
                dialogShaderPanel.FirstRender += (s, e) => dispatcher.TryEnqueue(() => deferral.Complete());

                await dialogShaderPanel.SetRenderTargetBitmapAsync(m_bitmap);

                overlayPanel.AddOverlay(dialogShaderPanel, offset);

                await Task.Delay(TimeSpan.FromSeconds(1.2f)); // sync with duration in TwirlDismiss

                overlayPanel.ClearOverlays();
            }
        }

        private RenderTargetBitmap m_bitmap = new RenderTargetBitmap();
        private RenderTargetBitmap m_fullBitmap = new RenderTargetBitmap();
        private Rect m_dialogRect = new();
    }
}
