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
using WinUIGallery.Helpers;
using Windows.Graphics;
using WinRT;
using System.Diagnostics;
using System.Numerics;

namespace WinUIGallery.Shaders
{
    internal static class CaptureHelper
    {
        public static async Task<CanvasRenderTarget> CaptureElementAsync(UIElement element, UIElement scaleElement = null)
        {
            Visual backingVisual = ElementCompositionPreview.GetElementVisual(element);
            var compositor = backingVisual.Compositor;
            var canvasDevice = CanvasDevice.GetSharedDevice();
            var compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

            float dpi = GetDpi(element);
            float dpiScale = dpi / 96.0f;
            if (scaleElement == null)
            {
                dpi = 96.0f;
                dpiScale = 1.0f;
            }

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

        public static Border GetDialogBox(this ContentDialog dialog)
        {
            // The dialog is actually a full window element because it darkens the screen
            // when it appears. Drill in a bit to get to the actual element that contains
            // what we think of as the "dialog".
            var child1 = VisualTreeHelper.GetChild(dialog, 0);
            var child2 = VisualTreeHelper.GetChild(child1, 0);
            var child3 = VisualTreeHelper.GetChild(child2, 0);

            var dialogBox = child3 as Border;
            return dialogBox;
        }

        public static float GetDpi(UIElement element)
        {
            var window = WindowHelper.GetWindowForElement(element);
            if (window == null)
            {
                // In the short time between the window closing and the process exiting, the window
                // could be null. Just return a default value here, we're about to exit anyways.
                return 1.0f;
            }

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            return Win32.GetDpiForWindow(hwnd);
        }
    }
}
