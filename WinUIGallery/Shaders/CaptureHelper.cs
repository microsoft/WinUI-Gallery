using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Display;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using WinUIGallery.Helper;
using Windows.Graphics;
using WinRT;
using System.Diagnostics;
using System.Numerics;

namespace WinUIGallery.Shaders
{
    internal static class CaptureHelper
    {
        public static async Task CaptureTo(this Window window, RenderTargetBitmap renderTarget)
        {
            var root = GetRoot(window.Content);
            await renderTarget.RenderAsync(root);
        }
        
        public static async Task CaptureTo(this UIElement uiElement, RenderTargetBitmap renderTarget)
        {
            //var root = GetRoot(uiElement);

            //var dpi = GetDpi(uiElement);

            //await renderTarget.RenderAsync(root, (int)(uiElement.RenderSize.Width * 96.0f / dpi), (int)(uiElement.RenderSize.Height * 96.0f / dpi));
            await renderTarget.RenderAsync(uiElement);
        }

        public static async Task<Rect> CaptureTo(this ContentDialog dialog, RenderTargetBitmap renderTarget)
        {
            // The dialog is actually a full window element because it darkens the screen
            // when it appears. Drill in a bit to get to the actual element that contains
            // what we think of as the "dialog".
            var child1 = VisualTreeHelper.GetChild(dialog, 0);
            var child2 = VisualTreeHelper.GetChild(child1, 0);
            var child3 = VisualTreeHelper.GetChild(child2, 0);

            var dialogContent = child3 as Border;
            var dpi = GetDpi(dialog) / 96.0f;

            // Get transform from dialog content to the window wide dialog.
            // This will let us position things later.
            var transform = dialogContent.TransformToVisual(dialog);
            Rect originalBounds = new()
            {
                X = 0,
                Y = 0,
                Width = dialogContent.RenderSize.Width,
                Height = dialogContent.RenderSize.Height
            };

            var transformedBounds = transform.TransformBounds(originalBounds);

            await renderTarget.RenderAsync(dialogContent, (int)(transformedBounds.Width / dpi), (int)(transformedBounds.Height / dpi));

            return transformedBounds;
        }

        public static async Task<CanvasRenderTarget> CaptureTo2(this UIElement element, UIElement scaleElement)
        {
            Visual backingVisual = ElementCompositionPreview.GetElementVisual(element);
            var compositor = backingVisual.Compositor;
            var canvasDevice = CanvasDevice.GetSharedDevice();
            var compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

            float dpi = GetDpi(element);
            float dpiScale = dpi / 96.0f;

            var size = new Size(element.RenderSize.Width, element.RenderSize.Height);

            CanvasRenderTarget bitmap = new CanvasRenderTarget(canvasDevice, (float)size.Width, (float)size.Height, dpi);

            var sizePixels = new SizeInt32(
                (int)bitmap.SizeInPixels.Width,
                (int)bitmap.SizeInPixels.Height);

            var sizeDips = new SizeInt32(
                (int)bitmap.Size.Width,
                (int)bitmap.Size.Height
                );

            Visual scaleElementVisual = null;
            Vector3 originalScale = Vector3.One;

            if (scaleElement != null)
            {
                // Scale up the element to account for DPI
                scaleElementVisual = ElementCompositionPreview.GetElementVisual(scaleElement);
                originalScale = scaleElementVisual.Scale;
                var newScale = originalScale;
                newScale.X *= dpiScale;
                newScale.Y *= dpiScale;
                scaleElementVisual.Scale = newScale;
            }

            ICompositionSurface captureSurface = await compositionGraphicsDevice.CaptureAsync(
                backingVisual,
                sizePixels,
                Microsoft.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                Microsoft.Graphics.DirectX.DirectXAlphaMode.Premultiplied,
                0);

            if (scaleElement != null)
            {
                scaleElementVisual.Scale = originalScale;
            }

            CompositionDrawingSurface drawingSurface = captureSurface.As<CompositionDrawingSurface>();

            var surfaceHelper = new CompositionInterop.SurfaceHelper();
            surfaceHelper.CopySurface(drawingSurface, bitmap);

            return bitmap;
        }

        public static Border GetDialogContent(this ContentDialog dialog)
        {
            // The dialog is actually a full window element because it darkens the screen
            // when it appears. Drill in a bit to get to the actual element that contains
            // what we think of as the "dialog".
            var child1 = VisualTreeHelper.GetChild(dialog, 0);
            var child2 = VisualTreeHelper.GetChild(child1, 0);
            var child3 = VisualTreeHelper.GetChild(child2, 0);

            var dialogContent = child3 as Border;
            return dialogContent;
        }

        public static float GetDpi(UIElement element)
        {
            var window = WindowHelper.GetWindowForElement(element);
            if (window == null)
            {
                return 1.0f;
            }

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            return Win32.GetDpiForWindow(hwnd);
        }

        public static async Task SaveAsBitmapAsync(this RenderTargetBitmap bitmap, Window referenceWindow)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("Bitmap", new List<string>() { ".bmp" });
            savePicker.SuggestedFileName = "image";

            var windowNative = WinRT.Interop.WindowNative.GetWindowHandle(referenceWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, windowNative);

            var outputFile = await savePicker.PickSaveFileAsync();

            if (outputFile == null)
            {
                return;
            }

            using (var stream = await outputFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);

                uint width = (uint)bitmap.PixelWidth;
                uint height = (uint)bitmap.PixelHeight;

                encoder.BitmapTransform.ScaledWidth = width;
                encoder.BitmapTransform.ScaledHeight = height;
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;

                var buffer = await bitmap.GetPixelsAsync();

                var softwareBitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, bitmap.PixelWidth, bitmap.PixelHeight);
                encoder.SetSoftwareBitmap(softwareBitmap);

                await encoder.FlushAsync();
            }
        }

        private static UIElement GetRoot(UIElement element)
        {
            UIElement root = element;
            UIElement parent = VisualTreeHelper.GetParent(root) as UIElement;
            while (parent != null)
            {
                root = parent;
                parent = VisualTreeHelper.GetParent(root) as UIElement;
            }

            return root;
        }
    }
}
