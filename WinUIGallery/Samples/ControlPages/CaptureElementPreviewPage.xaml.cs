// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.System;
using WinUIGallery.Helpers;

namespace WinUIGallery.ControlPages;

public sealed partial class CaptureElementPreviewPage : Page, INotifyPropertyChanged
{
    public CaptureElementPreviewPage()
    {
        this.InitializeComponent();
        this.Loaded += CaptureElementPreviewPage_Loaded;
        this.Unloaded += this.CaptureElementPreviewPage_Unloaded;
    }

    private async void CaptureElementPreviewPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await StartCaptureElement();
        }
        catch (UnauthorizedAccessException)
        {
            var dialog = new ContentDialog()
            {
                Title = "Camera access denied",
                Content = "Please enable camera access in the privacy settings.",
                PrimaryButtonText = "Privacy Settings",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
            };

            if (await dialog.ShowAsync() is ContentDialogResult.Primary)
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-webcam"));
            }
        }
        catch (Exception ex)
        {
            var dialog = new ContentDialog()
            {
                Title = "Error",
                Content = ex.Message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot,
            };

            await dialog.ShowAsync();
        }

        // Move the ScrollViewer from the captureContainer under an ExpandToFillContainer.
        // This will allow the snapshots column to use all available height without
        // influencing the height.
        var expandToFillContainer = new ExpandToFillContainer();
        var sv = captureContainer.Children[0];
        captureContainer.Children.Remove(sv);
        captureContainer.Children.Add(expandToFillContainer);
        expandToFillContainer.Children.Add(sv);
    }

    private void CaptureElementPreviewPage_Unloaded(object sender, RoutedEventArgs e)
    {
        // Needs to run as task to unblock UI thread
        if (mediaCapture != null)
        {
            new Task(mediaCapture.Dispose).Start();
        }
    }

    private MediaFrameSourceGroup mediaFrameSourceGroup;
    private MediaCapture mediaCapture;

    private async Task StartCaptureElement()
    {
        var groups = await MediaFrameSourceGroup.FindAllAsync();
        if (groups.Count == 0)
        {
            frameSourceName.Text = "No camera devices found.";
            return;
        }
        mediaFrameSourceGroup = groups.First();

        frameSourceName.Text = "Viewing: " + mediaFrameSourceGroup.DisplayName;
        mediaCapture = new MediaCapture();
        var mediaCaptureInitializationSettings = new MediaCaptureInitializationSettings()
        {
            SourceGroup = this.mediaFrameSourceGroup,
            SharingMode = MediaCaptureSharingMode.SharedReadOnly,
            StreamingCaptureMode = StreamingCaptureMode.Video,
            MemoryPreference = MediaCaptureMemoryPreference.Cpu
        };
        await mediaCapture.InitializeAsync(mediaCaptureInitializationSettings);

        // Set the MediaPlayerElement's Source property to the MediaSource for the mediaCapture.
        var frameSource = mediaCapture.FrameSources[this.mediaFrameSourceGroup.SourceInfos[0].Id];
        captureElement.Source = Windows.Media.Core.MediaSource.CreateFromMediaFrameSource(frameSource);
    }

    public string MirrorTextReplacement = ""; // starts not mirrored, so no text in that case

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    private void MirrorToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (mirrorSwitch.IsOn)
        {
            captureElement.RenderTransform = new ScaleTransform() { ScaleX = -1 };
            captureElement.RenderTransformOrigin = new Point(0.5, 0.5);
            MirrorTextReplacement =
                "\n" +
                "        // Mirror the preview\n" +
                "        captureElement.RenderTransform = new ScaleTransform() { ScaleX = -1 };\n" +
                "        captureElement.RenderTransformOrigin = new Point(0.5, 0.5);\n";
        }
        else
        {
            captureElement.RenderTransform = null;
            MirrorTextReplacement = "";
        }
        OnPropertyChanged("MirrorTextReplacement");
    }

    async private void CapturePhoto_Click(object sender, RoutedEventArgs e)
    {
        // Capture a photo to a stream
        var imgFormat = ImageEncodingProperties.CreateJpeg();
        var stream = new InMemoryRandomAccessStream();
        await mediaCapture.CapturePhotoToStreamAsync(imgFormat, stream);
        stream.Seek(0);

        // Show the photo in an Image element
        BitmapImage bmpImage = new BitmapImage();
        await bmpImage.SetSourceAsync(stream);
        var image = new Image() { Source = bmpImage };
        snapshots.Children.Insert(0, image);

        capturedText.Visibility = Visibility.Visible;

        UIHelper.AnnounceActionForAccessibility(captureButton, "Photo successfully captured.", "CameraPreviewSampleCaptureNotificationId");
    }
}

partial class ExpandToFillContainer : Grid
{
    protected override Size MeasureOverride(Size availableSize)
    {
        // Measure with the minimum height so it will just expand to whatever space is available.
        var desiredSize = base.MeasureOverride(new Size(availableSize.Width, 100));
        return desiredSize;
    }
}
